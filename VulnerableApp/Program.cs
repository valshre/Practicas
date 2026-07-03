using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Diagnostics; 

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .Enrich.WithMachineName() // (enriquecedor)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.R
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
);
builder.Services.AddSession();
var app = builder.Build();

// ====================================================
// 1. EXCEPTION MIDDLEWARE (Atrapa errores no controlados)
// ====================================================
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled Exception: Error no controlado interceptado por Middleware");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Ocurrio un error interno en el servidor.");
    }
});

// ====================================================
// 2. CORRELATION ID MIDDLEWARE (Rastreo de peticiones)
// ====================================================
app.Use(async (context, next) =>
{
    var cid = Guid.NewGuid().ToString();
    context.Response.Headers["X-Correlation-ID"] = cid;
    
    // Empuja el CorrelationId al contexto de Serilog para que Seq lo atrape
    using (Serilog.Context.LogContext.PushProperty("CorrelationId", cid))
    {
        await next(context);
    }
});

// ====================================================
// 3. REQUEST LOGGING MIDDLEWARE (Métricas de tiempo y estado)
// ====================================================
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    
    await next(context); 
    
    sw.Stop();
    
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var method = context.Request.Method;
    var path = context.Request.Path;
    var statusCode = context.Response.StatusCode;
    
    logger.LogInformation("Solicitud: {Method} {Path} | Código: {StatusCode} | Tiempo: {TiempoMs} ms", 
        method, path, statusCode, sw.ElapsedMilliseconds);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
    
app.UseSession();

await app.RunAsync();