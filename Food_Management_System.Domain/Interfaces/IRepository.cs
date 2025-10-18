using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Domain.Interfaces
{
    public interface IRepository<T> where T:BaseEntity
    {
        Task<IEnumerable<T>?> GetAll();
        Task<T?> GetById(int id);
        Task Add(T entity);
        void Update(T entity);
        void Remove(T entity);

    }
}
