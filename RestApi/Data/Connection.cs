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

        public Connection()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection_ = "Server=tcp:pruebahackathon.database.windows.net,1433;Initial Catalog=PruebasAlgoritmo;Persist Security Info=False;User ID=adminPrueba;Password=hackathon2021.*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            optionsBuilder.UseSqlServer(connection_);
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


