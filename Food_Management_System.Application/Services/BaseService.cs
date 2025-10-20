using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services
{
    public class BaseService<T>: IBaseService<T> where T:BaseEntity,new()
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<T> repository;

        public BaseService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            repository = unitOfWork.Repository<T>();
        }

        public async Task<IEnumerable<T>?> GetAll() {
            return await repository.GetAll();
        }
        public async Task<T?> GetById(int id) { 
            return await repository.GetById(id); 
        }

        public async Task<T> Create(T entity)
        {
            await repository.Add(entity);
            await unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task Update(T entity)
        {
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await repository.GetById(id);
            if (entity is null) return;
            repository.Remove(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
