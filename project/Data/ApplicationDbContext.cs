using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       public DbSet<project.Models.names>? names { get; set; }
        public DbSet<project.Models.Registercs>? Registercs { get; set; }
        public DbSet<project.Models.validation>? validation { get; set; }
        public DbSet<project.Models.account>? account { get; set; }
        public DbSet<project.Models.confirmedselect> confirmedselect { get; set; }
        public DbSet<project.Models.confirmed_report> confirmed_report { get; set; }
       
    }
}