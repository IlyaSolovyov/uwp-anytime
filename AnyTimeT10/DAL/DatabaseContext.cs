using AnyTimeT10.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyTimeT10.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<DailyTask> DailyTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Anytime.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyTask>()
                .Property(b => b.Completed)
                .HasDefaultValue(0);
        }


    }

    
}
