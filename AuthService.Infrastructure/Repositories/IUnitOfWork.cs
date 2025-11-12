using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database (delegates to DbContext.SaveChangesAsync).
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Convenience: commit and dispose the given transaction.
        /// </summary>
        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Convenience: rollback and dispose the given transaction.
        /// </summary>
        Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    }
}
