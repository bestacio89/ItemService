using Franz.Common.Messaging.Messages;
using ItemService.Consumer.Processing;

namespace ItemService.Consumer.Tests;

public sealed class ConsumerContractTests
{
    [Fact]
    public void GenericProcessorImplementsTheDeclaredGenericContract()
    {
        var contract = typeof(IGenericMessageProcessor<>);
        var implementedContracts = typeof(GenericMessageProcessor<>).GetInterfaces();

        Assert.Contains(implementedContracts, implemented =>
            implemented.IsGenericType &&
            implemented.GetGenericTypeDefinition() == contract);
    }

    [Fact]
    public void ProcessorAcceptsTheInstalledFranzMessageContract()
    {
        var process = typeof(GenericMessageProcessor<>).GetMethod("Process");

        Assert.NotNull(process);
        Assert.Equal(typeof(void), process!.ReturnType);
        Assert.Equal(new[] { typeof(Message) }, process.GetParameters().Select(parameter => parameter.ParameterType));
    }

    [Fact]
    public void ProcessorIsGenericOverAnEntityContract()
    {
        var processor = typeof(GenericMessageProcessor<>);
        var parameter = Assert.Single(processor.GetGenericArguments());

        Assert.Contains(parameter.GetGenericParameterConstraints(), constraint =>
            constraint.Name == "IEntity" &&
            constraint.Namespace == "Franz.Common.Business.Domain");
    }
}
