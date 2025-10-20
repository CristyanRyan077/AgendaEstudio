using AgendaApi.Application.Services;
using AgendaApi.Extensions;
using AgendaApi.Infra;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Infra.Repositories;
using AgendaApi.Interfaces;
using AgendaApi.Repositories;
using AgendaApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new() { Title = "Agenda API", Version = "v1" });
});
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<ICriancaService, CriancaService>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IPacoteService, PacoteService>();

builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICriancaRepository, CriancaRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IPacoteRepository, PacoteRepository>();

builder.Services.AddDbContext<AgendaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 4) CORS (libera seu app MAUI e localhost)
const string CorsPolicy = "CorsDev";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, policy =>
        policy
            .WithOrigins(
                "http://localhost",
                "http://localhost:5173",
                "http://localhost:5000",
                "http://10.0.2.2:5173",  // web local via emulador
                "http://10.0.2.2:5000",  // API via emulador Android
                "http://192.168.30.112"

            )
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgendaContext>();
    try
    {
        db.Database.CanConnect();
        Console.WriteLine("✅ Conexão com o banco bem-sucedida.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Erro ao conectar com o banco:");
        Console.WriteLine(ex.Message);
    }
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agenda API V1");
});


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
} */

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();


app.Run();

