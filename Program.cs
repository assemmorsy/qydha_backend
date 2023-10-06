using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Qydha.Controllers.Attributes;
using Qydha.Helpers;
using Qydha.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddControllers().AddNewtonsoftJson();
//  Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// otp options  
builder.Services.Configure<OTPSettings>(builder.Configuration.GetSection("OTP"));
// twilio options 
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
// twilio options 
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Authentication"));
// mail server settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));

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
// repos 
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<RegistrationOTPRequestRepo>();
builder.Services.AddScoped<UpdatePhoneOTPRequestRepo>();
builder.Services.AddScoped<UpdateEmailRequestRepo>();




//defined filters 
builder.Services.AddScoped<ValidateModelAttribute>();
builder.Services.AddScoped<ExceptionHandlerAttribute>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// services 
builder.Services.AddScoped<TokenManager>();
builder.Services.AddTransient<OtpManager>();
builder.Services.AddTransient<ISMSService, SMSService>();
builder.Services.AddTransient<IMailingService, MailingService>();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseStaticFiles();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

if (connectionString is not null)
    DbMigrator.Migrate(connectionString);

app.Run();



