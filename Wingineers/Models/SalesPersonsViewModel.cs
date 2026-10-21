namespace Wingineers.Models
{
    public sealed class SalesPersonsViewModel
    {
        public List<SalesPerson> SalesPersons { get; set; } = [];
        public string? Error { get; set; }
    }
    public sealed class SalesPersonDetailsViewModel
    {
        public SalesPerson? SalesPerson { get; set; }
        public string? Error { get; set; }
    }
    public sealed class SalesPerson
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public DateTime? HireDate { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? ZipCode { get; init; }
        public List<SalesPersonOrderLine> SalesPersonOrderLines { get; init; } = [];
    }
    public sealed class SalesPersonOrderLine
    {
        public int Id { get; init; }
        public string? OrderName { get; init; }
        public decimal OrderPrice { get; init; }
        public DateTime? OrderDate { get; init; }
    }
}
