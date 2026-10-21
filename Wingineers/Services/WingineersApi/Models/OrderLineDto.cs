namespace Wingineers.Services.WingineersApi.Models
{
    public sealed class OrderLineDto
    {
        public int Id { get; set; }
        public string? OrderName { get; set; }
        public decimal OrderPrice { get; set; }
        public string? OrderDate { get; set; }
        public int SalesPersonId { get; set; }
    }
}
