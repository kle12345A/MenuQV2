namespace MenuQ.Models
{
    public class OrderHisto
    {
        public int RequestID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
