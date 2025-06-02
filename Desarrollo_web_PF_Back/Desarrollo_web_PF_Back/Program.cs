using Desarrollo_web_PF_Back.Custom;
using Desarrollo_web_PF_Back.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Agregar servicios para conectar a la base de datos SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));


// Agregar el servicio de Utilidades para encriptaci�n y generaci�n de tokens JWT
builder.Services.AddSingleton<Utilidades>();

// Configurar autenticaci�n JWT
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validamos la firma
        ValidateIssuer = true,           // Se valida que el token provenga del emisor correcto
        ValidateAudience = true,         // Se valida que el token sea para la audiencia esperada
        ValidateLifetime = true,         // Se verifica que el token no haya expirado
        ClockSkew = TimeSpan.Zero,       // Evita retrasos en la expiraci�n del token
        ValidIssuer = builder.Configuration["Jwt:Issuer"],           // Obtener el Issuer desde appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],       // Obtener el Audience desde appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Agregar servicios de controladores
builder.Services.AddControllers();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.KeyLengthLimit = int.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configurar CORS para permitir solicitudes de cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();//Para subir archivos est�ticos.
// Aplicar la pol�tica CORS
app.UseCors("NewPolicy");


app.UseAuthentication();
app.UseAuthorization(); //PRUEBA DEL JTW

app.MapControllers();
app.Use(async (context, next) =>
{
    Console.WriteLine($"=== REQUEST: {context.Request.Method} {context.Request.Path} ===");
    Console.WriteLine($"Content-Type: {context.Request.ContentType}");
    Console.WriteLine($"Content-Length: {context.Request.ContentLength}");

    try
    {
        await next();
        Console.WriteLine($"=== RESPONSE: {context.Response.StatusCode} ===");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"=== MIDDLEWARE ERROR: {ex.Message} ===");
        Console.WriteLine($"=== STACK: {ex.StackTrace} ===");
        throw;
    }
});
app.Run();
