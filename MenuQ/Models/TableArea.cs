namespace MenuQ.Models
{
    public class TableArea
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int TableCount { get; set; }
        public List<int> Tables { get; set; } = new List<int>();
        public List<string> TableNumbers { get; set; }
    }

}
