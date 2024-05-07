using Core.Configurations;
using Core.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("mssql"),b=>b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
});

builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection("CacheConfiguration"));


//Injecting Hangfire
builder.Services.AddHangfire(x=>x.UseSqlServerStorage(builder.Configuration.GetConnectionString("mssql")));
builder.Services.AddHangfireServer();


//For In-Memory Caching
builder.Services.AddMemoryCache();
builder.Services.AddTransient<RedisCacheService>();
builder.Services.AddTransient<MemoryCacheService>();
builder.Services.AddTransient<Func<CacheTech,ICacheService>>(serviceProvider=> key=>
{
    switch (key)
    {
        case CacheTech.Memory:
            return serviceProvider.GetService<MemoryCacheService>();
        case CacheTech.Redis:
            return serviceProvider.GetService<RedisCacheService>();
        default:
            return serviceProvider.GetService<MemoryCacheService>();
    }
});

#region Repositories
builder.Services.AddTransient(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddTransient<ICustomerRepository,CustomerRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/dashboard");

app.MapControllers();

app.Run();
