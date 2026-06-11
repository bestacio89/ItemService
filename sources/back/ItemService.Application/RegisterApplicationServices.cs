using Franz.Common.Mapping.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace ItemService.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddFranzMapping(Assembly.GetExecutingAssembly());


    return collection;
  }

}



