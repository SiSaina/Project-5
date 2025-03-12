using ExamProjectOne.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ExamProjectOne.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole { Id = "3", Name = "Coach", NormalizedName = "COACH" },
                new IdentityRole { Id = "4", Name = "Senior coach", NormalizedName = "SENIOR COACH"},
                new IdentityRole { Id = "5", Name = "Supervisor", NormalizedName = "SUPERVISOR" },
                new IdentityRole { Id = "6", Name = "Senior supervisor", NormalizedName = "SENIOR SUPERVISOR" }
            );

            // one to one: coach -> user
            builder.Entity<Coach>()
                .HasOne(s => s.User)
                .WithOne(u => u.Coach)
                .HasForeignKey<Coach>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // one to one: customer -> user
            builder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // One-to-One: Supervisor -> User
            builder.Entity<Supervisor>()
                .HasOne(s => s.User)
                .WithOne(u => u.Supervisor)
                .HasForeignKey<Supervisor>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Many to one: appointment -> coach
            builder.Entity<Appointment>()
                .HasOne(a => a.Schedule)
                .WithMany()
                .HasForeignKey(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);
            // Many to one: appointment -> customer
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // one to one: group training -> schedule
            builder.Entity<GroupTraining>()
                .HasOne(gt => gt.Schedule)
                .WithOne()
                .HasForeignKey<GroupTraining>(gt => gt.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);
            // many to many: group training customer -> group training
            builder.Entity<GroupTrainingCustomer>()
                .HasOne(gtc => gtc.GroupTraining)
                .WithMany(gt => gt.GroupTrainingCustomers)
                .HasForeignKey(gtc => gtc.GroupTrainingId)
                .OnDelete(DeleteBehavior.Cascade);
            // many to many: group training customer -> customer
            builder.Entity<GroupTrainingCustomer>()
                .HasOne(gtc => gtc.Customer)
                .WithMany(c => c.GroupTrainingCustomers)
                .HasForeignKey(gtc => gtc.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // one to one: schedule -> coach
            builder.Entity<Schedule>()
                .HasOne(s => s.Coach)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
            // one to one: schedule -> gym hall
            builder.Entity<Schedule>()
                .HasOne(s => s.GymHall)
                .WithMany()
                .HasForeignKey(s => s.GymHallId)
                .OnDelete(DeleteBehavior.Cascade);

        }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<GymHall> GymHalls { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<GroupTraining> GroupTrainings { get; set; }
        public DbSet<GroupTrainingCustomer> GroupTrainingCustomers { get; set; }

    }
}
