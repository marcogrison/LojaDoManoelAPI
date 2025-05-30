using LojaDoManoel.API.Services;
using Microsoft.EntityFrameworkCore;
using LojaDoManoel.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container.
builder.Services.AddControllers();
builder.Services.AddScoped<PackingService>();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ****** INÍCIO: APLICAR MIGRAÇÕES AUTOMATICAMENTE ******
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApiDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Aplicando migrações pendentes...");
            context.Database.Migrate();
            Console.WriteLine("Migrações aplicadas com sucesso.");
        }
        else
        {
            Console.WriteLine("Nenhuma migração pendente.");
        }
    }
    catch (Exception ex)
    {
        // Em produção, use um logger mais robusto aqui.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu durante a aplicação das migrações do banco de dados.");
        Console.WriteLine($"ERRO AO APLICAR MIGRAÇÕES: {ex.Message}");
    }
}
// ****** FIM: APLICAR MIGRAÇÕES AUTOMATICAMENTE ******


// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();