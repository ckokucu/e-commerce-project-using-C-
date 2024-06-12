using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iakademi38_proje.Models
{
	public class Status
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[DisplayName("STATÜ ID")]
        public int StatusID { get; set; }

		[StringLength(100)]
		[Required]
        public string? StatusName { get; set; }

        public bool Active { get; set; }
    }
}
