var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GoldeniaAuthApi.Services.IEmailService, GoldeniaAuthApi.Services.EmailService>();
builder.Services.AddScoped<GoldeniaAuthApi.Services.ISmsService, GoldeniaAuthApi.Services.SmsService>();
var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
