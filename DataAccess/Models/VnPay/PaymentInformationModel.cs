namespace DataAccess.Models.VnPay;

     public class PaymentInformationModel
    {
        public string OrderType { get; set; }

        public int OrderId { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }