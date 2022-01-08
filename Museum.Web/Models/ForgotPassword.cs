using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Не указан Login")]
        public string Login { get; set; }
    }
}
