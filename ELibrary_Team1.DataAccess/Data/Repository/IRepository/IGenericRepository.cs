﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ELibrary_Team1.DataAccess.Data.Repository
{
    public interface IGenericRepository<T> where T: class
    {
    // Add
        void Add(T entity);
        T AddRange(T entites);
        //Task<T> AddAsync(T entity);
        //Task<T> AddRangeAsync(T entites);

    // Remove
        void Remove(T entity);
        //T RemoveRage(T entites);

        // Get
        T Get(int id);
        IEnumerable<T> GetAll();
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        T GetById(int id);
        T GetById(string id);
        //Task<T> GetByIdAsync(int id);
        //Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        // Find
        //T Find(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Update
        void Update(T entity);
        //Task<T> UpdateAsync(T entity);
    }
}
