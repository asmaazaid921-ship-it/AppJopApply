using AppJopApply.Application.Interfaces;
using AppJopApply.Infrastruction.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Infrastruction.Repositries
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public IQueryable<T> Get()
        {
            var entities = _dbSet.AsQueryable();
            return entities;
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}