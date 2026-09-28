using Applications.Interface.Auth;
using Applications.Interface.Repositories;
using Applications.Service;
using DataAccess;
using DataAccess.Repositories;
using Infrastructure;
using Messanger.API.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Enter your JWT Access Token"
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { 
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"), 
            
            new List<string>()
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidIssuer = "MyMessengerApp",

        ValidateAudience = false,
        ValidAudience = "MyMessengerAppClient",

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtOptions:SecretKey"])),

        ValidateLifetime = false,
        NameClaimType = "userId"
    };
});
builder.Services.AddDbContext<MessangerDbContexts>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(MessangerDbContexts)));
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 МБ
    options.ValueCountLimit = 20;
});


builder.Services.AddAntiforgery();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IChatsRepository, ChatsRepository>();
builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();
builder.Services.AddScoped<IChatMembersRepository, ChatMembersRepository>();
builder.Services.AddScoped<IContactsRepository, ContactsRepository>();
builder.Services.AddControllers();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<MessagesServices>();
builder.Services.AddScoped<ChatMembersServices>();
builder.Services.AddScoped<ChatsService>();
builder.Services.AddScoped<ContactsService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtProvider, JWTProvider>();


var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapUsersEndpoints();
app.MapMessagesEndpoints();
app.MapChatsMembersEndpoints();
app.MapChatsEndpoints();
app.MapContactsEndpoints();
app.Run();