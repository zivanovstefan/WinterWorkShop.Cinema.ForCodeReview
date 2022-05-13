using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("tag")]
    public class Tag
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("tagName")]
        public string Name { get; set; }
        public virtual ICollection<MovieTag> MovieTags { get; set; }
    }
}
