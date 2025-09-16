using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBRS.Models
{
    public class Buses
    {
        [Key]
        public Guid BusId { get; set; }

        [Required]
        [StringLength(50)]
        public string? BusNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string? BusName { get; set; }

        [Required]
        [StringLength(50)]
        public string? BusType { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerSeat { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Bus image is required")]
        public string BusImage { get; set; }

        [ForeignKey("routes")]
        public Guid Route_id { get; set; }
        public Routes routes { get; set; }

        public Prices price { get; set; }
    }
}
