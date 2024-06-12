using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iakademi38_proje.Models
{
	public class Order
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public string? OrderGroupGUID { get; set; } //20231104121625 DateTimeNow

        public int UserID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }
    }
}
