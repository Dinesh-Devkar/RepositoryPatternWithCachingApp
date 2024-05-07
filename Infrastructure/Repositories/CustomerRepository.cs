using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class CustomerRepository:GenericRepository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CustomerRepository(Func<CacheTech,ICacheService> cacheService,ApplicationDbContext context):base(cacheService,context)
        {
            _dbContext=context;
        }

        public async Task AddCustomerRangeAsync()
        {
            List<Customer> customers=new List<Customer>();
            for (int i = 0; i <= 2000; i++)
            {
                customers.Add(new Customer()
                {
                    FirstName=$"Test FirstName - {i}",
                    LastName=$"Test LastName - {i}",
                    Email=$"Test Address - {i}",
                    Address=$"Test Address - {i}",
                    ContactNo=$"Test ContactNo - {i}",
                    DateOfBirth=DateTime.Now
                });
            }

            await _dbContext.Set<Customer>().AddRangeAsync(customers);
            await _dbContext.SaveChangesAsync();
        }
    }
}