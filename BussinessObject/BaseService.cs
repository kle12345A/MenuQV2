using DataAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IBaseRepository<T> _baseRepository;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = _unitOfWork.BaseRepository<T>();
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _baseRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _baseRepository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _baseRepository.FindAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _baseRepository.GetByIdAsync(id);
        }

        public IQueryable<T> GetQuery()
        {
            return _baseRepository.GetQuery();
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return _baseRepository.GetQuery(where);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _baseRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
