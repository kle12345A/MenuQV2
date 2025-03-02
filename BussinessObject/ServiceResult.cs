using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ServiceResult<T> CreateSuccess(T data, string message = "Operation completed successfully")
        {
            return new ServiceResult<T> { Success = true, Message = message, Data = data };
        }

        public static ServiceResult<T> CreateError(string message)
        {
            return new ServiceResult<T> { Success = false, Message = message };
        }
    }
}
