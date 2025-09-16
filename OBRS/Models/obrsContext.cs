using Microsoft.EntityFrameworkCore;

namespace OBRS.Models
{
    public class obrsContext : DbContext {
        public obrsContext(DbContextOptions<obrsContext> options) : base(options)
        {
        }
        public DbSet<Buses> tbl_bus { get; set; }
        public DbSet<Routes> tbl_route { get; set; }
        public DbSet<Prices> tbl_price{ get; set; }
        public DbSet<Employee> tbl_employees { get; set; }
        public DbSet<Booking> tbl_bookings { get; set; }
        public DbSet<Contact> tbl_contact { get; set; }

    }
}
