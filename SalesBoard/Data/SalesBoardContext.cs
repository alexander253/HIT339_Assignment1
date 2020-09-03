using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesBoard.Models;

namespace SalesBoard.Data


{
    public class SalesBoardContext : DbContext
    {
        public SalesBoardContext(DbContextOptions<SalesBoardContext> options)
            : base(options)
        {
        }

        public DbSet<Items> Items { get; set; }
        public DbSet<SalesBoard.Models.Sales> Sales { get; set; }
    }
    }