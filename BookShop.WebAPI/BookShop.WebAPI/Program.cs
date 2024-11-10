using BookShop.Common.DataContext.Postgress;
using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.DataContext.Postgress.Repositories;

using NLog.Web;
using NLog;
using BookShop.WebAPI.Middlewares;
using Dapper;
using BookShop.Common.Models.Postgress.Models;
using System.Data;
using System.Text.Json;
using BookShop.Common.DataContext.Postgress.Mappers;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try 
{
    var builder = WebApplication.CreateBuilder(args);


    // Добавление NLog как провайдера логов
    //builder.Logging.ClearProviders();
    //builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddSingleton<DapperDbContext>();
    builder.Services.AddScoped<IPublisherRepos, PublisherRepos>();
    builder.Services.AddScoped<ICustomerRepos, CustomerRepos>();
    builder.Services.AddScoped<IGenreRepos, GenreRepos>();
    builder.Services.AddScoped<IReviewRepos, ReviewRepos>();
    builder.Services.AddScoped<IBookGenresRepos, BookGenreRepos>();
    builder.Services.AddScoped<IAuthorRepos, AuthorRepos>();
    builder.Services.AddScoped<IBookAuthorsRepos, BookAuthorRepos>();
    builder.Services.AddScoped<IBookRepos, BookRepos>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();




// Регистрация маппера
SqlMapper.AddTypeHandler(new ImageLinkMapper());



var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseMiddleware<RequestControllerMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

