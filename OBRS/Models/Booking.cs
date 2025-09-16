using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBRS.Models
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }   // Primary Key (GUID)

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string PassengerName { get; set; }  // Passenger full name

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string PassengerCNIC { get; set; }  // Passenger CNIC

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string PassengerPhone { get; set; }  // Passenger contact number

        [Required]
        public DateTime TravelDate { get; set; }  // Date of travel

        [Required]
        [Column(TypeName = "int")]
        public int SeatNumber { get; set; }  // Reserved Seat Number

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; }  // "Booked", "Cancelled", "Completed"

        [Required]
        public DateTime BookingDate { get; set; }  // When booking was made

        // ✅ Only FK string Id (no navigation, no foreign key attribute)
        public string UserId { get; set; }

        [ForeignKey("bus")]
        public Guid Bus_id { get; set; }
        public Buses bus { get; set; }
    }
}
