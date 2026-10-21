namespace Wingineers.Models
{
    public sealed class OverviewViewModel
    {
        public List<MonthlyOrderCount> Months { get; set; } = [];
        public int TotalOrders { get; set; }
        public int UnreadableDateCount { get; set; }
        public string? Error { get; set; }
    }

    public sealed class MonthlyOrderCount
    {
        public DateOnly Month { get; init; }
        public int OrderCount { get; init; }
    }
}
