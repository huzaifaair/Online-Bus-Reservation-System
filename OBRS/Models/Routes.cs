using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBRS.Models
{
    public class Routes
    {
        [Key]
        public Guid RouteId { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string RouteName { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string StartLocation { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Destination { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Distance must be greater than 0")]
        public int DistanceInKm { get; set; }

        [Required]
        public TimeSpan EstimatedDuration { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
