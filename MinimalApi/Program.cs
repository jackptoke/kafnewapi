using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;
using MinimalApi.Persistence;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "KAFNews API", Description = "Bringing the latest news to Karens all around the world", Version = "v1" });
});

// Add the Azure Key Vault configuration provider
if (!string.IsNullOrEmpty(builder.Configuration["VaultURI"]))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(builder.Configuration["VaultURI"]),
        new DefaultAzureCredential());
}

builder.Services.AddDbContext<ArticleContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AzureSQLConnectionString"));
    
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KAFNew API V1");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/articles",
    async (ArticleContext dbContext) => await dbContext.Articles.ToListAsync());

app.MapGet("/articles/{id}",
    (string id, ArticleContext dbContext) => dbContext.Articles
    .FirstOrDefault(article => article.Id == id) is Article article ? 
    Results.Ok(article) : Results.NotFound());

app.MapPost("/articles",
    async (ArticleContext dbContext, [FromBody] Article article) =>
    {
        article.Id = Guid.NewGuid().ToString();
        await dbContext.Articles.AddAsync(article);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/articles/{article.Id}", article);
    });



app.Run();

