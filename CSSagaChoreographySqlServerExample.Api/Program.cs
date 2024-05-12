using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CrystalSharp;
using CrystalSharp.EventStores.EventStoreDb.Extensions;
using CrystalSharp.MsSql.Extensions;
using CrystalSharp.MsSql.Migrator;
using CSSagaChoreographySqlServerExample.Application.OrderSaga.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string eventStoreConnectionString = builder.Configuration.GetConnectionString("AppEventStoreConnectionString");
string sagaStoreConnectionString = builder.Configuration.GetConnectionString("AppSagaStoreConnectionString");
MsSqlSettings sagaStoreDbSettings = new(sagaStoreConnectionString);

IResolver resolver = CrystalSharpAdapter.New(builder.Services)
    .AddCqrs(typeof(PlaceOrderTransaction))
    .AddEventStoreDbEventStore<int>(eventStoreConnectionString)
    .AddMsSqlSagaStore(sagaStoreDbSettings, typeof(PlaceOrderTransaction))
    .CreateResolver();

IMsSqlDatabaseMigrator dbMigrator = resolver.Resolve<IMsSqlDatabaseMigrator>();

MsSqlSagaStoreSetup.Run(dbMigrator, sagaStoreDbSettings.ConnectionString).Wait();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
