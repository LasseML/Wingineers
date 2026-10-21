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
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Wingineers API call failed");
                model.Error = "Orders couldn't be loaded right now. Please try again later.";
            }

            return model;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
