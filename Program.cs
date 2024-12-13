using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Hubs;
using GoWheels_WebAPI.Mapping;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var secretKey = builder.Configuration["Jwt:SecretKey"];//Get SercretKey from appsetting.json

// Add services to the container.
var connection = builder.Configuration
                        .GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection String not found");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Config token validate
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
    options.SaveToken = true;
    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        // Đọc token từ query string khi dùng SignalR
    //        var accessToken = context.Request.Query["access_token"];
    //        if (!string.IsNullOrEmpty(accessToken))
    //        {
    //            context.Token = accessToken;
    //        }
    //        return Task.CompletedTask;
    //    }
    //};
});
builder.Services.AddHttpContextAccessor();
//cho vuejs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5500", "http://localhost:5173")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

//builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ILocatorService,GoogleApiService>(client =>
{
    client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/distancematrix/");
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(config =>
{
    config.Cookie.HttpOnly = true;
    config.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllers();
//Register dependency service
builder.Services.AddScoped<IAdminPromotionService, AdminPromotionService>();
builder.Services.AddScoped<IGenericRepository<Amenity>, AmenityRepository>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService,BookingService>();
builder.Services.AddScoped<ICarTypeDetailRepository, CarTypeDetailRepository>();
builder.Services.AddScoped<IGenericRepository<CarType>, CarTypeRepository>();
builder.Services.AddScoped<ICarTypeService, CarTypeService>();
builder.Services.AddScoped<IGenericRepository<Company>, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IDriverBookingRepository, DriverBookingRepository>();
builder.Services.AddScoped<IDriverBookingService, DriverBookingService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IUserRepository, AuthenticationRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<INotifyRepository, NotifyRepository>();
builder.Services.AddScoped<INotifyService, NotifyService>();
builder.Services.AddScoped<IPostAmenityRepository, PostAmenityRepository>();
builder.Services.AddScoped<IPostPromotionRepository, PostPromotionRepository>();
builder.Services.AddScoped<IPostPromotionService, PostPromotionService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IGenericRepository<Report>, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IGenericRepository<ReportType>, ReportTypeRepository>();
builder.Services.AddScoped<IReportTypeService, ReportTypeService>();
builder.Services.AddScoped<IStartupService, StartupService>();
builder.Services.AddScoped<IUserPromotionService, UserPromotionService>();
builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<ILocatorService, GoogleApiService>();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo 
    {
        Title = "Product APIs", Version = "v1" 
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
// Add NewtonsoftJSON for serializing/deserializing JSON
//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//});


var app = builder.Build();

//Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ApplicationRole.InitialRoles(services);//Initial default role

    //Startup Service
    var startupService = scope.ServiceProvider.GetRequiredService<IStartupService>();
    startupService.UpdateBookingsOnStartup();
    startupService.UpdatePostOnStartup();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    if (context.Request.Query.ContainsKey("access_token"))
    {
        var accessToken = context.Request.Query["access_token"];
        if (!string.IsNullOrEmpty(accessToken))
        {
            context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
        }
    }
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAllOrigins");
// Map SignalR Hub
app.MapHub<NotifyHub>("notifyhub").RequireCors("AllowAllOrigins");
app.MapControllers();
app.Run();