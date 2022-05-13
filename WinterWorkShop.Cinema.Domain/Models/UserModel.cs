using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class UserModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.USER_PROPERTIE_FIRSTNAME_NOT_VALID)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = Messages.USER_PROPERTIE_LASTNAME_VALID)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = Messages.USER_PROPERTIE_USERNAME_NOT_VALID)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
