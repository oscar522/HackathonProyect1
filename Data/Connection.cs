using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public partial class Connection : DbContext
    {
        public Connection(DbContextOptions<Connection> options)
            : base(options)
        {
        }
        public DbSet<Orders> Orders { get; set; }


            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Orders>().ToTable("Orders");
            }
        }
    }


