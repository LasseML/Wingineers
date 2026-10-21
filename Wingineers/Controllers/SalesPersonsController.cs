using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using Wingineers.Models;
using Wingineers.Services.WingineersApi;
using Wingineers.Services.WingineersApi.Models;

namespace Wingineers.Controllers
{
    public class HomeController(WingineersApiService wingineersApi, ILogger<HomeController> logger) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            SalesPersonsViewModel model = await GetSalesPersonsAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            SalesPersonsViewModel all = await GetSalesPersonsAsync(cancellationToken);

            if (all.Error is not null)
            {
                return View(new SalesPersonDetailsViewModel { Error = all.Error });
            }

            SalesPerson? salesPerson = all.SalesPersons.FirstOrDefault(s => s.Id == id);
            if (salesPerson is null)
            {
                return NotFound();
            }

            return View(new SalesPersonDetailsViewModel { SalesPerson = salesPerson });
        }

        public async Task<IActionResult> Overview(CancellationToken cancellationToken)
        {
            var model = new OverviewViewModel();

            try
            {
                List<OrderLineDto>? orderLines = await wingineersApi.GetOrderLinesAsync(cancellationToken);

                if (orderLines is null)
                {
                    logger.LogWarning("Wingineers API orderLines returned null");
                    model.Error = "Orders couldn't be loaded right now. Please try again later.";
                    return View(model);
                }

                model.TotalOrders = orderLines.Count;

                // First day of the month for every order with a readable date
                var orderMonths = new List<DateOnly>();
                foreach (OrderLineDto line in orderLines)
                {
                    if (DateTime.TryParse(line.OrderDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        orderMonths.Add(new DateOnly(date.Year, date.Month, 1));
                    }
                    else
                    {
                        model.UnreadableDateCount++;
                    }
                }

                if (model.UnreadableDateCount > 0)
                {
                    logger.LogWarning("{Count} order lines had an unreadable OrderDate", model.UnreadableDateCount);
                }

                if (orderMonths.Count > 0)
                {
                    Dictionary<DateOnly, int> countsByMonth = orderMonths
                        .GroupBy(m => m)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Walk every month from first to last, including months with 0 orders
                    for (DateOnly month = orderMonths.Min(); month <= orderMonths.Max(); month = month.AddMonths(1))
                    {
                        model.Months.Add(new MonthlyOrderCount
                        {
                            Month = month,
                            OrderCount = countsByMonth.GetValueOrDefault(month)
                        });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Wingineers API call failed");
                model.Error = "Orders couldn't be loaded right now. Please try again later.";
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<SalesPersonsViewModel> GetSalesPersonsAsync(CancellationToken cancellationToken)
        {
            SalesPersonsViewModel model = new();

            try
            {
                Task<List<OrderLineDto>?> orderLinesTask = wingineersApi.GetOrderLinesAsync(cancellationToken);
                Task<List<SalesPersonDto>?> salesPersonsTask = wingineersApi.GetSalesPersonsAsync(cancellationToken);

                await Task.WhenAll(orderLinesTask, salesPersonsTask);

                if (salesPersonsTask.Result is null)
                {
                    logger.LogWarning("Wingineers API salesPersons returned null");
                    model.Error = "Orders couldn't be loaded right now. Please try again later.";
                }
                else
                {
                    if (orderLinesTask.Result is null)
                    {
                        logger.LogWarning("Wingineers API orderLines returned null");
                    }

                    ILookup<int, OrderLineDto> orderLinesBySalesPerson =
                        (orderLinesTask.Result ?? []).ToLookup(o => o.SalesPersonId);

                    model.SalesPersons = salesPersonsTask.Result
                        .Select(x => new SalesPerson
                        {
                            Id = x.Id,
                            Name = x.Name ?? "",
                            HireDate = x.HireDate ?? "",
                            Address = x.Address ?? "",
                            City = x.City ?? "",
                            ZipCode = x.ZipCode ?? "",
                            SalesPersonOrderLines = orderLinesBySalesPerson[x.Id]
                                .Select(o => new SalesPersonOrderLine
                                {
                                    Id = o.Id,
                                    OrderName = o.OrderName ?? "",
                                    OrderPrice = o.OrderPrice,
                                    OrderDate = o.OrderDate ?? ""
                                })
                                .ToList()
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wingineers API call failed");
                model.Error = "Orders couldn't be loaded right now. Please try again later.";
            }

            return model;
        }
    }
}
