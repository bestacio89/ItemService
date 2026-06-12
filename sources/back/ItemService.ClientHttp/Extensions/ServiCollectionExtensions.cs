using ItemService.Client.Http.Abstractions;
using ItemService.Client.Http.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Client.Http.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddItemServiceClient(
      this IServiceCollection services,
      string baseUrl)
  {
    services.AddHttpClient<IItemHttpClient, ItemHttpClient>(client =>
    {
      client.BaseAddress = new Uri(baseUrl);
    });

    return services;
  }
}