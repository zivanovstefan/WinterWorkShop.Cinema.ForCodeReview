using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("movieTags")]
    public class MovieTag
    {
		[Key, Column("movieId")]
		public Guid MovieId { get; set; }
		[Key, Column("tagId")]
		public int TagId { get; set; }

		public virtual Movie Movie { get; set; }
		public virtual Tag Tag { get; set; }
	}
}
