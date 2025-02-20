namespace MenuQ.Models
{
    public class AreaViewModel
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public List<TableViewModel> Tables { get; set; }
    }

    public class TableViewModel
    {
        public int TableNumber { get; set; }
    }

}
