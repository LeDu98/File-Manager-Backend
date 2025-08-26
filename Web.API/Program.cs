using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application;
using System.Reflection.Metadata;
using Application.BusinessLogic.Queries.Folder;
using Application.BusinessLogic.Commands;
using Application.BusinessLogic.Commands.Folder;

var builder = WebApplication.CreateBuilder(args);

// CORS
const string DevCors = "DevCors";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(DevCors, p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// appsettings.json: ConnectionStrings:Default
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

builder.Services.AddMediatR(Application.AssemblyReference.Assembly);
builder.Services.AddMediatR(typeof(GetFolderChildrenHandler).Assembly);
builder.Services.AddMediatR(typeof(GetFolderBreadcrumbQuery).Assembly);
builder.Services.AddMediatR(typeof(DeleteItemsCommand).Assembly);
builder.Services.AddMediatR(typeof(CreateFolderCommand).Assembly);

// Add services to the container.

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FileManager API",
        Version = "v1",
        Description = "API za rad sa folderima i fajlovima"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ProblemDetails (uniformne greške)
builder.Services.AddProblemDetails();

var app = builder.Build();

// Swagger UI only in DEV
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileManager API v1");
        c.RoutePrefix = "swagger"; // otvara se na /swagger
    });
}

// Configure the HTTP request pipeline.
app.UseCors(DevCors);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
