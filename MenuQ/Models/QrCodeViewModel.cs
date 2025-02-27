namespace MenuQ.Models
{
    public class QrCodeViewModel
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int TableCount { get; set; }
        public string TopText { get; set; }
        public string BottomText { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public List<QrCodeDetails> QrCodes { get; set; } = new List<QrCodeDetails>();
    }
}
