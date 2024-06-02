using Application;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext dbContext
         )
        {
            _dbContext = dbContext;
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public void BeginTransactionLocking()
        {
            _transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        }
        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }
        public void ClearTrack()
        {
            _dbContext.ChangeTracker.Clear();
        }
    }
}
