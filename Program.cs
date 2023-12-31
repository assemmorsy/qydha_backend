using System.Data;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Qydha.Controllers.Attributes;
using Qydha.Helpers;
// using Qydha.Hubs;
using Qydha.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_private_key.json")
});
//  Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("postgres");
// builder.Services.AddSignalR();
builder.Services.AddControllers((options) =>
{
    options.Filters.Add<ExceptionHandlerAttribute>();
}).AddNewtonsoftJson();
//  Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Serilog
Log.Logger =
    new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(), "./logs/qydha_.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();
builder.Host.UseSerilog();


#region DI settings
// otp options  
builder.Services.Configure<OTPSettings>(builder.Configuration.GetSection("OTP"));
// twilio options 
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
// JWT options 
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Authentication"));
// mail server settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));
// whatsapp  settings
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
// Photo Settings
builder.Services.Configure<PhotoSettings>(builder.Configuration.GetSection("PhotoSettings"));
// IAPHub Settings
builder.Services.Configure<IAPHubSettings>(builder.Configuration.GetSection("IAPHubSettings"));
// Products Settings
builder.Services.Configure<ProductsSettings>(builder.Configuration.GetSection("ProductsSettings"));


// Authentication 
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new()
        {
            // TODO ::  Validate Life time when have refresh token mechanism
            // ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"] ?? "MustProvideSecretKeyIn__CONFIGURATION__")
            )
        };
    });

// db connection
builder.Services.AddScoped<IDbConnection, NpgsqlConnection>(
    sp => new NpgsqlConnection(connectionString));

#endregion

#region DI Repos
// repos 
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<RegistrationOTPRequestRepo>();
builder.Services.AddScoped<UpdatePhoneOTPRequestRepo>();
builder.Services.AddScoped<UpdateEmailRequestRepo>();
builder.Services.AddScoped<SubscriptionRepo>();
builder.Services.AddScoped<NotificationRepo>();
#endregion

#region Filters

//defined filters 
builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddScoped<ExceptionHandlerAttribute>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

#endregion

#region DI Services
// services 
builder.Services.AddScoped<TokenManager>();
builder.Services.AddTransient<OtpManager>();
builder.Services.AddTransient<IMessageService, WhatsAppService>();
builder.Services.AddTransient<IMailingService, MailingService>();
builder.Services.AddTransient<IFileService, GoogleCloudFileService>();
builder.Services.AddScoped<FCMService>();
#endregion

#region ADD Cors
// string MyAllowSpecificOrigins = "_MyAllowSpecificOrigins";

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
//     {
//         builder.WithOrigins("http://localhost:5173")
//         .AllowAnyMethod()
//         .AllowAnyHeader()
//         .AllowCredentials();
//     });
// });
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseCors(MyAllowSpecificOrigins);
app.UseStaticFiles();

// app.UseHttpLogging();

app.UseSerilogRequestLogging(); // <-- Add this line

app.UseAuthentication();

app.UseAuthorization();

// app.MapHub<ChatHub>("/chat");
app.MapControllers();

if (connectionString is not null)
    DbMigrator.Migrate(connectionString);

app.Run();
