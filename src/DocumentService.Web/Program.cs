using DocumentService.Web;
using DocumentService.Web.Extensions;
using DocumentService.Web.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.SetupLogging();
builder.SetupDatabase();
builder.SetupMediatR();
builder.SetupMapper();
builder.SetupAuth();
builder.SetupProductionService();
builder.SetupS3Storage();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.UseSerilogRequestLogging(opt => { opt.EnrichDiagnosticContext = HttpContextExtensions.EnrichFromRequest; });

app.UseMiddleware<ExceptionMiddleware>();

app.UseMigrations();

app.Run();
