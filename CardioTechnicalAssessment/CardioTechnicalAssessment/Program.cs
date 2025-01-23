using CardioTechnicalAssessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<SpotifyAuthService>();
builder.Services.AddHttpClient();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", policy =>
//    {
//        policy.WithOrigins("https://localhost:7205")  // Specify the origin here (for example, your Swagger UI URL)
//              .AllowAnyMethod()  // Allows any HTTP method (GET, POST, etc.)
//              .AllowAnyHeader()  // Allows any headers
//              .AllowCredentials();  // Allow credentials (cookies, etc.)
//    });
//});

var app = builder.Build();

//app.UseCors("AllowSpecificOrigin");  //Added by me


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
