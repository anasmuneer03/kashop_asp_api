using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> GetAllAsync(string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>(); 

            if (includes != null) 
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);  //(join) .Include(c => c.Translations)
                }
            }
            return await query.ToListAsync();
        }

        public async Task<bool> DeleteAsync(T entity) 
        {
            _context.Remove(entity);
            var affected = await _context.SaveChangesAsync();
            return affected > 0 ;
        }

        public async Task<T?> GetOne(Expression<Func<T,bool>> filter, string[]? includes = null) 
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);  //(join) .Include(c => c.Translations)
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }
    }
}
