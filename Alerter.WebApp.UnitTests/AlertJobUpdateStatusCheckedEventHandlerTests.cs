using Alerter.WebApp.Hubs;
using Alerter.WebApp.Infrastructure;
using Alerter.WebApp.Infrastructure.AlertJobDomain;
using Alerter.WebApp.Infrastructure.AlertNotifying;
using Alerter.WebApp.Infrastructure.EventHandlers;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Alerter.WebApp.UnitTests
{
    public class AlertJobUpdateStatusCheckedEventHandlerTests
    {
        private Fixture fixture;

        public AlertJobUpdateStatusCheckedEventHandlerTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_WhenAlertJobNotExists_ThenItDoesNotCallSendNotificationAsync()
        {
            //arrange
            int id = fixture.Create<int>();

            var mockAlertJobService = new Mock<IAlertJobService>();
            var mockHubContext = new Mock<IHubContext<AlertHub>>();

            mockAlertJobService.Setup(x => x.GetAlertJobDetailsAsync(id))
                .Returns(Task.FromResult<AlertJob>(null))
                .Verifiable();

            var eventHandler = new AlertJobUpdateStatusCheckedEventHandler(
                mockAlertJobService.Object, mockHubContext.Object);

            //act
            await eventHandler.Handle(new Data.Events.StatusCheckedEvent(id, true),
                It.IsAny<CancellationToken>());

            //assert
            mockAlertJobService.Verify(x => x.GetAlertJobDetailsAsync(id), Times.Once);
            mockHubContext.Verify(x => x.Clients.User(It.IsAny<string>()).SendCoreAsync(
                "ReceiveStatus", It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenAlertJobExists_ThenItCallsHubContextReceiveStatusFunction()
        {
            //arrange
            int id = fixture.Create<int>();
            Guid userId = fixture.Create<Guid>();

            var mockAlertJobService = new Mock<IAlertJobService>();
            var mockHubContext = new Mock<IHubContext<AlertHub>>();

            mockAlertJobService.Setup(x => x.GetAlertJobDetailsAsync(id))
                .Returns(Task.FromResult(new AlertJob()
                {
                    Id = id,
                    UserId = userId
                }))
                .Verifiable();

            mockAlertJobService.Setup(x => x.UpdateAlertJobAsync(userId, It.IsAny<AlertJob>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockHubContext.Setup(x => x.Clients.User(userId.ToString()).SendCoreAsync(
                    "ReceiveStatus", It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var eventHandler = new AlertJobUpdateStatusCheckedEventHandler(
                mockAlertJobService.Object, mockHubContext.Object);

            //act
            await eventHandler.Handle(new Data.Events.StatusCheckedEvent(id, true),
                It.IsAny<CancellationToken>());

            //assert
            mockAlertJobService.Verify(x => x.GetAlertJobDetailsAsync(id), Times.Once);
            mockAlertJobService.Verify(x =>
                x.UpdateAlertJobAsync(userId, It.Is<AlertJob>(y => y.CurrentStatus == "OK")), Times.Once);

            mockHubContext.Verify(x => x.Clients.User(It.IsAny<string>()).SendCoreAsync(
                "ReceiveStatus", It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
