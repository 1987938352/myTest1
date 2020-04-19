using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class UserRegisterModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PassWord { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string verfiyCode { get; set; }
    }
}
