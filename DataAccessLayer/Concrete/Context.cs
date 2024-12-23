using EntityLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-HJO4S3H\\SQLEXPRESS;database=DbSistemAnalizi1;integrated security=true;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.BarberService)
                .WithMany(bs => bs.Appointments)
                .HasForeignKey(a => a.BarberServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<BarberService> BarberServices { get; set; }
        public DbSet<BarberShop> BarberShops { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }
        public DbSet<Approval> Approvals { get; set; }
    }
}
