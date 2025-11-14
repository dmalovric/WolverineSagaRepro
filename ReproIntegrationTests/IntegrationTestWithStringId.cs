using FastEndpoints.Testing;
using Wolverine.Tracking;
using WolverineSagaIssue.StringId;

namespace ReproIntegrationTests;

[Collection(TestCollection.Name)]
public  class IntegrationTestWithStringId(Fixture _fixture) : TestBase
{
    [Fact]
    public async Task WorkingTest()
    {
        // Arrange
        string orderPlacedId = Guid.NewGuid().ToString();
        StringIdOrderPlacedMessage orderPlacedMessage = new(
            Id: orderPlacedId,
            Description: "Order With String Id"
        );

        // Act
        var trackedSession = await _fixture.Host.InvokeMessageAndWaitAsync(orderPlacedMessage, Fixture.MessagingTimeoutInMilliseconds);

        // Assert
        var receivedMessage = trackedSession.Sent.SingleMessage<StringIdOrderProcessedMessage>();
        Assert.Equivalent(receivedMessage.Id, orderPlacedId);
    }
}
