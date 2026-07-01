using Franz.Common.Mapping.Extensions;
using ItemService.Application.Items.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace ItemService.Application;
public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection collection)
  {


   collection.AddFranzMapping(Assembly.GetExecutingAssembly());
   collection.AddScoped<IItemCreationService, ItemCreationService>();
   collection.AddScoped<IItemDeletionService, ItemDeletionService>();
   collection.AddScoped<IItemUpdateService, ItemUpdateService>();
   collection.AddScoped<IItemUniquenessChecker, ItemUniquenessChecker>();

    return collection;
  }

}



