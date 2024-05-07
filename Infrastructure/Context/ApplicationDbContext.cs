using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            ChangeTracker.QueryTrackingBehavior=QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Customer> Customers { get;}
    }
}