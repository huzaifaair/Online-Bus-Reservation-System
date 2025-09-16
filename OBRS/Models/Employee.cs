using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBRS.Models
{
    public class Employee
    {
        [Key]
        public Guid EmployeeId { get; set; }   // Primary Key

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string FullName { get; set; }  // Name of Driver/Conductor

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string CNIC { get; set; }      // National ID (Unique)

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }  // Contact Number

        [Column(TypeName = "varchar(200)")]
        public string Address { get; set; }   // Home Address

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Role { get; set; }      // "Driver" or "Conductor"

        [Column(TypeName = "varchar(50)")]
        public string? LicenseNumber { get; set; }  // Only for Drivers, optional for Conductors

        [Required]
        public DateTime HireDate { get; set; }  // Date of Joining

        [Required]
        public bool IsActive { get; set; }     // Active / Inactive

        [Required(ErrorMessage = "Bus image is required")]
        public string EmployeeImage { get; set; }

        [ForeignKey("bus")]
        public Guid Bus_id { get; set; }
        public Buses bus { get; set; }
    }
}
