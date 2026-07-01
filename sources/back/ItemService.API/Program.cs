using Franz.Common.Business.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Extensions;
using ItemService.Application;
using ItemService.Application.Items.Commands;
using ItemService.Application.Items.Services;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// =========================================================================
// 1. HOST & OBSERVABILITY SUBSYSTEM
// =========================================================================
builder.Host.UseLog();
builder.Services.AddFranzTelemetry(env, config);

// =========================================================================
// 2. SELF-HEALING ARCHITECTURE CORE (AUTONOMOUS DISCOVERY)
// =========================================================================
builder.Services.RegisterApplicationServices();
builder.Services.AddBusiness(typeof(ItemEntity).Assembly);


builder.Services.AddRelationalDatabase<ApplicationDbContext>(env, config);
builder.Services.AddFranzAuditing();
// Manages Caching, DbContext, and Automated Generic CRUD Topologies
builder.Services.RegisterPersistenceServices<ApplicationDbContext>(config);

builder.Services.AddFranzMediator(
    new[] { typeof(CreateItemCommandHandler).Assembly })
  .AddFranzSerilogAuditPipeline()
  .AddFranzSerilogLoggingPipeline();

// AUTOMATIC DISCOVERY GATE: Hydrates Web Stack, Controllers, Routing, 
// Auth, Swagger, Versioning, Mediator Handlers, and ISeeder Implementations.
builder.Services.AddHttpArchitecture(env, config);

var app = builder.Build();

// =========================================================================
// 3. LIFECYCLE & ENVIRONMENT BOUNDARY STATES
// =========================================================================
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var ct = CancellationToken.None;
  if (app.Environment.IsDevelopment())
  {
    var db = services.GetRequiredService<ApplicationDbContext>();
    var seeder = services.GetRequiredService<DatabaseSeeder>();

    await db.Database.MigrateAsync();
    await seeder.RunAsync(ct);
  }
  else
  {
    Log.Information(
        "Higher Environment detected. Database runtime initialization handled by orchestration plane.");
  }
}


app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// =========================================================================
// 4. MIDDLEWARE PIPELINE
// =========================================================================
app.UseHttpArchitecture();

//app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
  app.UseEndpoints(endpoints =>
  {
    endpoints.MapControllers().AllowAnonymous();
  });
}

var _ = app.MapControllers();

app.Run();


