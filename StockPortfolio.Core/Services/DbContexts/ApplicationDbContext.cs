using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Domain;

namespace StockPortfolio.Core.Services.DbContexts;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Security> Securities { get; set; }
    //public DbSet<SharePriceHistory> SharePriceHistories { get; set; }
    //public DbSet<IntradayPriceHistory> IntradayPriceHistories { get; set; }
}
