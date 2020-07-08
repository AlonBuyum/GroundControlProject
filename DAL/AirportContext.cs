using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DAL
{
    public class AirportContext : DbContext
    {
        public AirportContext( DbContextOptions options) : base(options) { }

       public DbSet<StateDO> StateDOs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder); 
        }
    }
}
