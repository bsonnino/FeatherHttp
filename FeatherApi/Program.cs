using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var app = WebApplication.Create(args);
var Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

app.MapGet("/weatherforecast", async http =>
{
    var rng = new System.Random();
    var weatherForecast = Enumerable.Range(1, 5)
        .Select(index => new WeatherForecast(
            DateTime.Now.AddDays(index), rng.Next(-20, 55), 
            Summaries[rng.Next(Summaries.Length)])).ToArray();
    http.Response.ContentType = "application/json";    
    await http.Response.WriteAsync(JsonSerializer.Serialize(weatherForecast));
});

await app.RunAsync();

public record WeatherForecast(DateTime Date, int TemperatureC,string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}