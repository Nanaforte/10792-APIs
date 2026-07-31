using System.Text;
using ApiAnaForteProjetoFinal.Cache;
using ApiAnaForteProjetoFinal.Resilience;
using ApiAnaForteProjetoFinal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//config dos servicos da app

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



//1 registar suporte para cache em memoria
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IBookCacheService, BookCacheService>();

//2 registo do nosso servico personalizado de autenticacao na injecao de dependencias
builder.Services.AddScoped<IAuthService, AuthService>();

//3 registo do HttpClient com Resiliencia POLLY retry + circuit breaker)
string mountebankUrl = builder.Configuration["ExternalServices:MountebankUrl"] ?? "http://localhost:4545";

builder.Services.AddHttpClient<IExternalService, ExternalService>(client =>
{
    client.BaseAddress = new Uri(mountebankUrl);
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())         // Associa a Política de Retry
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy()); // Associa a Política de Circuit Breaker

//4 ler definicoes do JWT do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

//config do Middleware de autenticacao JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

//5 config do Swagger para aceitar Tokens JWT no btn "Authorize"
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Livraria API - Projeto Final", Version = "v1" });

    //add a definicao de segurança JWT ao Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT neste formato: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//ativa a autenticacao e autorizacao (A ORDEM IMPORTA!)
app.UseAuthentication(); //1º Quem es tu?
app.UseAuthorization();  //2º O que podes fazer?

app.MapControllers();

app.Run();