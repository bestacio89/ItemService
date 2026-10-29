using ItemService.Consumer.Processing;

namespace ItemService.Consumer.Tests;

public sealed class ConsumerContractTests
{
    [Fact]
    public void GenericMessageProcessor_ImplementsItsDeclaredContract()
    {
        Assert.True(
            typeof(IGenericMessageProcessor<>).IsAssignableFrom(typeof(GenericMessageProcessor<>)));
    }
}
