using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public  class Connection : DbContext
    {
        //public Connection(DbContextOptions<Connection> options)
        //    : base(options)
        //{
        //}

        private readonly string _connectionString;

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<Resultado> Resultado { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Orders>().ToTable("Orders");
                modelBuilder.Entity<Resultado>().ToTable("Resultado");
            }
        }
    }


