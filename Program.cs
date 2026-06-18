using UserManagementAPI.Data;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<UserRepository>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Required middleware order:
// 1. Error Handling
// 2. Authentication
// 3. Logging

app.UseErrorHandlingMiddleware();
app.UseAuthenticationMiddleware();
app.UseLoggingMiddleware();

app.MapControllers();

app.Run();