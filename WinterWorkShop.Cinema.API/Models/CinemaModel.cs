using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CinemaModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.USER_PROPERTIE_USERNAME_NOT_VALID)]
        public string Name { get; set; }
    }
}
