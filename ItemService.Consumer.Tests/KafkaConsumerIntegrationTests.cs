using Franz.Common.Messaging.Messages;
using Franz.Common.Business.Domain;
using ItemService.Consumer.Processing;
using ItemService.Domain.Items.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

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

        var constraint = Assert.Single(parameter.GetGenericParameterConstraints());

        Assert.Equal(typeof(IEntity<>), constraint.GetGenericTypeDefinition());
        Assert.Equal(typeof(Guid), Assert.Single(constraint.GetGenericArguments()));
    }

    [Fact]
    public void Process_HandlesJsonNullWithoutThrowing()
    {
        var processor = new GenericMessageProcessor<ItemEntity>(
            NullLogger<GenericMessageProcessor<ItemEntity>>.Instance);

        processor.Process(new Message("null"));
    }

    [Fact]
    public void Process_RethrowsMalformedJsonForRetryOrDeadLetterHandling()
    {
        var processor = new GenericMessageProcessor<ItemEntity>(
            NullLogger<GenericMessageProcessor<ItemEntity>>.Instance);

        Assert.Throws<JsonSerializationException>(() => processor.Process(new Message("{")));
    }
}
