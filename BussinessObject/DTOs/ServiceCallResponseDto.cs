using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTOs
{
    public class ServiceCallResponseDto
    {
        public int customerId {  get; set; }
        public int tableId { get; set; }
        public string CustomService { get; set; }
        public List<ServiceReason> ListService { get; set; } = new List<ServiceReason>();
       public List<string> ListReson { get; set; } = new List<string>();
    }
}
