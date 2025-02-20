namespace MenuQ.Models
{
    public class QrCodeDetails
    {
        public int AreaId { get; set; }
        public int TableId { get; set; }
        public string TopText { get; set; }
        public string BottomText { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public string TableName { get; set; }
        public string Url { get; set; } // Link dẫn đến trang đặt món
    }

}
