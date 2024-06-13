using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Process = Domain.Entities.Process;

namespace Infrastructures
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<CourseTag> CourseTag { get; set; }
        public DbSet<Chapter> Chapter { get; set; }
        public DbSet<Lesson> Lesson { get; set; }
        public DbSet<Learner> Learner { get; set; }
        public DbSet<Mentor> Mentor { get; set; }
        public DbSet<Course> Course { get; set; }

        public DbSet<Identication> Identication { get; set; }
        public DbSet<BankInformation> BankInformation { get; set; }
        public DbSet<ConnectionType> ConnectionType { get; set; }
        public DbSet<Connection> Connection { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderTransaction> OrderTransaction { get; set; }
        public DbSet<Process> Process { get; set; }
        public DbSet<Requirement> Requirement { get; set; }
        public DbSet<WalletHistory> WalletHistory { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<WithdrawTransaction> WithdrawTransaction { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var cascadeFKs = builder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            base.OnModelCreating(builder);
        }



        /*        private string GetConnectionString()
                {
                    IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json", true, true).Build();
                    var strConn = config["ConnectionStrings:Development"];
                    return strConn;
                }*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }
    }
}
