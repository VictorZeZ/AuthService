using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace AuthService.Tests.Mocks
{
    /// <summary>
    /// Provides preconfigured mock for IUnitOfWork.
    /// </summary>
    public static class UnitOfWorkMock
    {
        public static Mock<IUnitOfWork> CreateDefault()
        {
            var mock = new Mock<IUnitOfWork>();
            var fakeTransaction = new Mock<IDbContextTransaction>().Object;

            mock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeTransaction);

            mock.Setup(u => u.CommitTransactionAsync(
                It.IsAny<IDbContextTransaction>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }
    }
}
