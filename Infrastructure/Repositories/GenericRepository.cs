using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CacheTech _cacheTech=CacheTech.Memory;

        private readonly string _cacheKey=$"{typeof(T)}";
        private readonly Func<CacheTech,ICacheService> _cacheService;
        private readonly ApplicationDbContext _context;

        public GenericRepository(Func<CacheTech,ICacheService> cacheService,ApplicationDbContext context)
        {
            this._context=context;
            _cacheService=cacheService;
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            BackgroundJob.Enqueue(()=> RefreshCache());
        }

        public async Task DeleteAsync(T entity)
        {
             _context.Set<T>().Remove(entity);
             await _context.SaveChangesAsync();
             BackgroundJob.Enqueue(()=> RefreshCache());

        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if(!_cacheService(_cacheTech).TryGet(_cacheKey,out IReadOnlyList<T> cacheList))
            {
                cacheList= await _context.Set<T>().ToListAsync();

                _cacheService(_cacheTech).Set(_cacheKey,cacheList);
            }
            return cacheList;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State=EntityState.Modified;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            BackgroundJob.Enqueue(()=> RefreshCache());
        }

        public async Task RefreshCache()
        {
            Debug.WriteLine("CacheKey : "+_cacheKey);
            _cacheService(_cacheTech).Remove(_cacheKey);
            // var cacheList= await _context.Set<T>().ToListAsync();
            // _cacheService(_cacheTech).Set(_cacheKey,cacheList);

        }
    }
}