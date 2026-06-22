using Anthropic.SDK;
using DocumentSummariser.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Anthropic client
builder.Services.AddSingleton<AnthropicClient>(serviceProvider =>
{
    var apiKey = builder.Configuration["Anthropic:ApiKey"];
    return new AnthropicClient(apiKey);
});

builder.Services.AddScoped<ISummariseService, SummariseService>();

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
