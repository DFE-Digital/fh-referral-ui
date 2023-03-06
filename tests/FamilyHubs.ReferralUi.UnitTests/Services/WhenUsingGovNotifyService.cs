using FamilyHubs.ReferralUi.Ui.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Notify.Interfaces;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingGovNotifyService
{
    [Fact]
    public async Task ThenSendAnEmailNotification()
    {
        //Arrange
        IOptions<GovNotifySetting> notifyoptions = Options.Create<GovNotifySetting>(new GovNotifySetting
        {
                APIKey = "ApiKey",
                TemplateId = "TemplateId"
        });

        Mock<IAsyncNotificationClient> mockAsyncNotificationClient = new Mock<IAsyncNotificationClient>();
       
        int setCallback = 0;
        mockAsyncNotificationClient.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>(), It.IsAny<string>()))
        .Callback(() => setCallback++);
        GovNotifySender govNotifySender = new GovNotifySender(notifyoptions, mockAsyncNotificationClient.Object);

        //Act
        await govNotifySender.SendEmailAsync("someone@email.com", "Title", "Some Message");

        //Assert
        setCallback.Should().Be(1);


    }
}
