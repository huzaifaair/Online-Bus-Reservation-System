using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBRS.Models
{
    public class Prices
    {
        [Key]
        public Guid PriceId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseFare { get; set; }   // e.g., 1000.00

        [Column(TypeName = "decimal(10,2)")]
        public decimal AdditionalCharges { get; set; } = 0;  // e.g., Toll, Tax

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; } = 0; // Optional discounts

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalFare { get; set; }    // (BaseFare + AdditionalCharges - Discount)

        public bool IsActive { get; set; } = true;

        [ForeignKey("bus")]
        public Guid Bus_id { get; set; }
        public Buses bus { get; set; }
    }
}
