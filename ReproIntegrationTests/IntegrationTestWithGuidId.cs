using FastEndpoints.Testing;
using Wolverine.Tracking;
using WolverineSagaIssue.GuidId;

namespace ReproIntegrationTests;

[Collection(TestCollection.Name)]
public  class IntegrationTestWithGuidId(Fixture _fixture) : TestBase
{
    [Fact]
    public async Task ReproductionTest()
    {
        // Arrange
        Guid orderPlacedId = Guid.NewGuid();
        GuidIdOrderPlacedMessage orderPlacedMessage = new(
            Id: orderPlacedId,
            Description: "Order With Guid Id"
        );

        // Act
        var trackedSession = await _fixture.Host.InvokeMessageAndWaitAsync(orderPlacedMessage, Fixture.MessagingTimeoutInMilliseconds);

        // Assert
        var receivedMessage = trackedSession.Sent.SingleMessage<GuidIdOrderProcessedMessage>();
        Assert.Equivalent(receivedMessage.Id, orderPlacedId);
    }
}
