using MedicalAI.Infrastructure.Data;
using MedicalAI.Infrastructure;
using MedicalAI.Core.Interfaces;
using MedicalAI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClinicalService, ClinicalService>();

// NEO4J RAG SERVICES
if (builder.Configuration.GetValue<bool>("Neo4j:Enabled"))
{
    builder.Services.AddNeo4jRagServices(
        neo4jUri: builder.Configuration["Neo4j:Uri"],
        neo4jUsername: builder.Configuration["Neo4j:Username"],
        neo4jPassword: builder.Configuration["Neo4j:Password"]
    );
}

builder.Services.AddScoped<IClinicalServiceWithRAG, ClinicalServiceWithRAG>();
builder.Services.AddHttpClient<IAIPredictionClient, AIPredictionClient>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000"); 
    client.Timeout = TimeSpan.FromSeconds(30);
});

// CẤU HÌNH JWT BẢO MẬT 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Medical AI API", Version = "v1" });

    // 1. Thêm định nghĩa cho nút Authorize (hiển thị giao diện)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Nhập JWT token theo định dạng: Bearer [khoảng trắng] [chuỗi token]. Ví dụ: 'Bearer eyJhbGci...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Ép Swagger phải đính kèm token này vào Header của mỗi request
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Initialize Neo4j Knowledge Graph
using (var scope = app.Services.CreateScope())
{
    try
    {
        await scope.ServiceProvider.InitializeNeo4jKnowledgeGraphAsync();
        Console.WriteLine("✅ Neo4j Knowledge Graph initialized");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Neo4j initialization: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();