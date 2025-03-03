using ExamProjectOne.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ExamProjectOne.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
                .HasOne(a => a.Coach)
                .WithMany()
                .HasForeignKey(a => a.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
            // Many to one: appointment -> customer
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
            // Many to one: appointment -> gymhall
            builder.Entity<Appointment>()
                .HasOne(a => a.GymHall)
                .WithMany()
                .HasForeignKey(a => a.GymHallId)
                .OnDelete(DeleteBehavior.SetNull);


            // one to many: group training -> customer
            builder.Entity<GroupTraining>()
                .HasOne(gt => gt.Customer)
                .WithMany()
                .HasForeignKey(gt => gt.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
            // one to one: group training -> schedule
            builder.Entity<GroupTraining>()
                .HasOne(gt => gt.Schedule)
                .WithOne()
                .HasForeignKey<GroupTraining>(gt => gt.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);


            // one to one: schedule -> coach
            builder.Entity<Schedule>()
                .HasOne(s => s.Coach)
                .WithOne()
                .HasForeignKey<Schedule>(s => s.CoachId)
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
    }
}
