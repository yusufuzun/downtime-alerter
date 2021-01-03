using Alerter.WebApp.AlertHttpClient;
using Alerter.WebApp.JobManagement;
using AutoFixture;
using Hangfire;
using Hangfire.Common;
using MediatR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Alerter.WebApp.UnitTests
{
    public class AlertJobSchedulingManagerTests
    {
        private Fixture fixture;

        public AlertJobSchedulingManagerTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task AddOrUpdateAsync_WhenItCalled_ThenItCallsHangfireRecurringJobManager()
        {
            //arrange
            int id = 5;

            var mockManager = new Mock<IRecurringJobManager>();
            var mockClient = new Mock<IStatusCheckClient>();
            var mockMediator = new Mock<IMediator>();

            mockManager.Setup(x => x.AddOrUpdate(
                "RC:Alert:" + id.ToString(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()))
                .Verifiable();

            var alertJobSchedulingManager = new AlertJobSchedulingManager(
                mockManager.Object, mockClient.Object, mockMediator.Object);

            //act
            await alertJobSchedulingManager.AddOrUpdateAsync(id, It.IsAny<string>(), It.IsAny<int>());

            //assert
            mockManager.Verify(x => x.AddOrUpdate(
                "RC:Alert:" + id.ToString(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()), Times.Once);
        }


        [Fact]
        public async Task RemoveAsync_WhenItCalled_ThenItCallsHangfireRecurringJobManager()
        {
            //arrange
            int id = 5;

            var mockManager = new Mock<IRecurringJobManager>();
            var mockClient = new Mock<IStatusCheckClient>();
            var mockMediator = new Mock<IMediator>();

            mockManager.Setup(x => x.RemoveIfExists("RC:Alert:" + id.ToString()))
                .Verifiable();

            var alertJobSchedulingManager = new AlertJobSchedulingManager(
                mockManager.Object, mockClient.Object, mockMediator.Object);

            //act
            await alertJobSchedulingManager.RemoveAsync(id);

            //assert
            mockManager.Verify(x => x.RemoveIfExists("RC:Alert:" + id.ToString()), Times.Once);
        }
    }
}
