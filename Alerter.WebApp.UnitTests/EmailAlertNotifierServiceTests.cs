using Alerter.WebApp.Infrastructure.AlertNotifying;
using AutoFixture;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Alerter.WebApp.UnitTests
{
    public class EmailAlertNotifierServiceTests
    {
        private Fixture fixture;

        public EmailAlertNotifierServiceTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task SendNotificationAsync_WhenItCalledWithoutParameters_ThenItThrowsArgumentException()
        {
            //arrange
            var mockClient = new Mock<ISmtpClient>();
            var mockOptions = new Mock<IOptions<SmtpSettings>>();
            var mockLogger = new Mock<ILogger<EmailAlertNotifierService>>();

            var emailAlertNotifierService = new EmailAlertNotifierService(
                mockOptions.Object, mockClient.Object, mockLogger.Object);

            bool isCatchWorked = false;

            //act
            try
            {
                await emailAlertNotifierService.SendNotificationAsync(new Dictionary<string, string>());
            }
            catch (ArgumentException exc)
            {
                Assert.Contains("not enough arguments", exc.Message);
                isCatchWorked = true;
            }

            //assert
            Assert.True(isCatchWorked);
        }

        [Fact]
        public async Task SendNotificationAsync_WhenItCalledWithCorrectParameters_ThenItCallsSmtpClientSendAsync()
        {
            //arrange
            var mockClient = new Mock<ISmtpClient>();
            var mockOptions = new Mock<IOptions<SmtpSettings>>();
            var mockLogger = new Mock<ILogger<EmailAlertNotifierService>>();

            mockClient.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), default, null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockOptions.Setup(x => x.Value).Returns(new SmtpSettings
            {
                MailSender = "test",
                MailSenderName = "test",
                SmtpServer = "test",
                Port = 12312,
                Password = "test",
                Username = "test"
            });

            var emailAlertNotifierService = new EmailAlertNotifierService(
                mockOptions.Object, mockClient.Object, mockLogger.Object);

            bool isCatchWorked = false;

            //act
            try
            {
                await emailAlertNotifierService.SendNotificationAsync(new Dictionary<string, string> {
                    {"email", "test" },
                    {"subject", "test" },
                    {"message", "test" }
                });
            }
            catch (Exception exc)
            {
                isCatchWorked = true;
            }

            //assert
            Assert.True(!isCatchWorked);
            mockClient.Verify(x => x.SendAsync(
                It.Is<MimeMessage>(y => y.Subject.Contains("test")), default, null));
        }

        [Fact]
        public async Task SendNotificationAsync_WhenSmtpClientSendAsyncThrowsException_ThenItCatchAndLogException()
        {
            //arrange
            var mockClient = new Mock<ISmtpClient>();
            var mockOptions = new Mock<IOptions<SmtpSettings>>();
            var mockLogger = new Mock<ILogger<EmailAlertNotifierService>>();

            mockClient.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), default, null))
                .Throws(new Exception("client"))
                .Verifiable();

            mockOptions.Setup(x => x.Value).Returns(new SmtpSettings
            {
                MailSender = "test",
                MailSenderName = "test",
                SmtpServer = "test",
                Port = 12312,
                Password = "test",
                Username = "test"
            });

            mockLogger.Setup(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            var emailAlertNotifierService = new EmailAlertNotifierService(
                mockOptions.Object, mockClient.Object, mockLogger.Object);

            bool isCatchWorked = false;

            //act
            try
            {
                await emailAlertNotifierService.SendNotificationAsync(new Dictionary<string, string> {
                    {"email", "test" },
                    {"subject", "test" },
                    {"message", "test" }
                });
            }
            catch (Exception exc)
            {
                isCatchWorked = true;
            }

            //assert
            Assert.True(!isCatchWorked);

            mockLogger.Verify(x =>
            x.Log(
                LogLevel.Error,
                It.Is<EventId>(y => y.Id == 2000),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
