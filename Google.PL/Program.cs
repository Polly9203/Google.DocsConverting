using Google.BLL;
using Utils.Constants.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGoogleBll();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FileConfiguration>(builder.Configuration.GetSection("FileConfiguration"));
builder.Services.Configure<GoogleConfiguration>(builder.Configuration.GetSection("GoogleConfiguration"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
