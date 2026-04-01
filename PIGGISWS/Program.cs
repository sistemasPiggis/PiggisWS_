using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Services;
using PIGGISWS.Services.Utils;
using System.Configuration;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;


var builder = WebApplication.CreateBuilder(args);

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

////// Add services to the container.
//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
//            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
//            .AddInMemoryTokenCaches();


//AMBIENTE PRODUCCIÓN
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApi(builder.Configuration);
builder.Services.AddAuthorization();



builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));



var connectionString = builder.Configuration.GetConnectionString("TESPIG11G");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseOracle(connectionString) );
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.EnableSensitiveDataLogging());

builder.Services.AddScoped<IAgenteService, AgenteService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IListaPreciosService, ListaPreciosService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFirebaseNotificationService ,FirebaseNotificationService>();
builder.Services.AddScoped<IRuteroService, RuteroService>();
builder.Services.AddScoped<ICarteraService, CarteraService>();
builder.Services.AddHostedService<FcmMessageBackgroundService>();
builder.Services.AddScoped<IDevolucionesService, DevolucionesService>();
builder.Services.AddScoped<IDescuentoService, DescuentoService>();
builder.Services.AddScoped<IMarcacionService, MarcacionService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IMensajeriaService, MensajeriaService>();
builder.Services.AddScoped<FireBaseService>();
// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Para loggear en la consola
builder.Logging.AddDebug(); // Para loggear en la ventana de salida de Visual Studio
builder.Logging.AddEventSourceLogger(); // Otras opciones de logging



builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});
var app = builder.Build();

app.UseResponseCompression();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(options =>
  options.WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader())
    ;

// Inicialización de FirebaseApp
//FirebaseApp.Create(new AppOptions()
//{
//    Credential = GoogleCredential.FromFile("path/to/serviceAccountKey.json"),
//});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
