using Franz.Common.Business.Domain;
using Franz.Common.DependencyInjection;
using Franz.Common.Messaging;
using Franz.Common.Messaging.Messages;

namespace ItemService.Consumer.Processing
{
  public interface IGenericMessageProcessor<T> where T : IEntity
  {
    void Process(Message message);
  }
}



