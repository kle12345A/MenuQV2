using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BussinessObject.DTOs
{

    public class LoginCustomerDto
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[1-9][0-9]{8,9})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        public string Username { get; set; }
    }

}
