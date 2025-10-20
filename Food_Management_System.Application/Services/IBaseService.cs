using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services
{
    public interface IBaseService<T> where T:BaseEntity
    {
        Task<IEnumerable<T>?> GetAll();
        Task<T?> GetById(int id);
        Task<T> Create(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
