using Alerter.WebApp.AlertHttpClient;
using Alerter.WebApp.Infrastructure;
using Alerter.WebApp.Infrastructure.AlertJobDomain;
using Alerter.WebApp.Infrastructure.AlertNotifying;
using Alerter.WebApp.Infrastructure.EventHandlers;
using Alerter.WebApp.JobManagement;
using AutoFixture;
using Hangfire;
using Hangfire.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Alerter.WebApp.UnitTests
{
    public class AlerterEmailSendStatusCheckedEventHandlerTests
    {
        private Fixture fixture;

        public AlerterEmailSendStatusCheckedEventHandlerTests()
        {
            fixture = new Fixture();
        }
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        [Fact]
        public async Task Handle_WhenAlertJobNotExists_ThenItDoesNotCallSendNotificationAsync()
        {
            //arrange
            int id = 5;

            var mockAlertJobService = new Mock<IAlertJobService>();
            var mockNotifierService = new Mock<IAlertNotifierService>();
            var mockUserManager = MockUserManager<IdentityUser>();

            mockAlertJobService.Setup(x => x.GetAlertJobDetailsAsync(id))
                .Returns(Task.FromResult<AlertJob>(null))
                .Verifiable();

            var eventHandler = new AlerterEmailSendStatusCheckedEventHandler(
                mockAlertJobService.Object, mockNotifierService.Object, mockUserManager.Object);

            //act
            await eventHandler.Handle(new Data.Events.StatusCheckedEvent(id, true),
                It.IsAny<CancellationToken>());

            //assert
            mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            mockNotifierService.Verify(x => x.SendNotificationAsync(It.IsAny<Dictionary<string, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenAlertJobExists_ThenItCallsSendNotificationAsync()
        {
            //arrange
            int id = fixture.Create<int>();
            Guid userId = fixture.Create<Guid>();
            var email = fixture.Create<string>();

            var mockAlertJobService = new Mock<IAlertJobService>();
            var mockNotifierService = new Mock<IAlertNotifierService>();
            var mockUserManager = MockUserManager<IdentityUser>();

            mockAlertJobService.Setup(x => x.GetAlertJobDetailsAsync(id))
                .Returns(Task.FromResult(new AlertJob()
                {
                    Id = id,
                    UserId = userId
                }))
                .Verifiable();

            mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .Returns(Task.FromResult(new IdentityUser { Email = email }))
                .Verifiable();

            var eventHandler = new AlerterEmailSendStatusCheckedEventHandler(
                mockAlertJobService.Object, mockNotifierService.Object, mockUserManager.Object);

            //act
            await eventHandler.Handle(new Data.Events.StatusCheckedEvent(id, true),
                It.IsAny<CancellationToken>());

            //assert
            mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            mockNotifierService.Verify(x => x.SendNotificationAsync(It.Is<Dictionary<string, string>>(
                y => y["email"] == email)), Times.Once);
        }

    }
}
