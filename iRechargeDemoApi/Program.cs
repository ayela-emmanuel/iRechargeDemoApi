

using iRechargeDemoApi.DataContext;
using iRechargeDemoApi.Models.Config;
using iRechargeDemoApi.Services;
using iRechargeDemoApi.Services.AWS;
using iRechargeDemoApi.Services.BillProviders;
using iRechargeDemoApi.Services.NotificationProviders;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle






builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();


// Add config
builder.Services.Configure<TwilioConfig>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<MailConfig>(builder.Configuration.GetSection("Mail"));
builder.Services.Configure<AWSConfig>(builder.Configuration.GetSection("AWS"));



// Add Services


builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<SmsService>();

builder.Services.AddSingleton<SNSHelper>();
builder.Services.AddSingleton<SQSHelper>();

builder.Services.AddTransient<AppDBContext>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<BuyPowerProvider>();
builder.Services.AddScoped<FlutterwaveProvider>();


var app = builder.Build();



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
