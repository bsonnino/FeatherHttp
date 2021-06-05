using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using CustomerApi.Model;
using System.Text.Json;
using System.Threading.Tasks;

var app = WebApplication.Create(args);

app.MapGet("/", GetAllCustomers);
app.MapGet("/{id}", GetCustomer);
app.MapPost("/", AddCustomer);
app.MapPut("/", UpdateCustomer);
app.MapDelete("/{id}", DeleteCustomer);

await app.RunAsync();

async Task GetAllCustomers(HttpContext http)
{
    var customerRepository = new CustomerRepository();
    http.Response.ContentType = "application/json";
    await http.Response.WriteAsync(JsonSerializer.Serialize(customerRepository.GetAll()));
}

async Task GetCustomer(HttpContext http)
{
    if (!http.Request.RouteValues.TryGet("id", out string id))
    {
        http.Response.StatusCode = 400;
        return;
    }
    var customerRepository = new CustomerRepository();
    Customer customer = customerRepository.GetCustomer(id);
    if (customer != null)
    {
        http.Response.ContentType = "application/json";
        await http.Response.WriteAsync(JsonSerializer.Serialize(customer));
    }
    else
        http.Response.StatusCode = 404;
    return;
}

async Task AddCustomer(HttpContext http)
{
    var customer = await http.Request.ReadFromJsonAsync<Customer>();
    if (string.IsNullOrWhiteSpace(customer.CustomerId))
    {
        http.Response.StatusCode = 400;
        return;
    }
    var customerRepository = new CustomerRepository();
    if (customerRepository.Add(customer))
    {
        customerRepository.Commit();
        http.Response.StatusCode = 201;
        http.Response.ContentType = "application/json";
        await http.Response.WriteAsync(JsonSerializer.Serialize(customer));
        return;
    }
    http.Response.StatusCode = 409;
    return;
}

async Task UpdateCustomer(HttpContext http)
{
    var customer = await http.Request.ReadFromJsonAsync<Customer>();
    if (string.IsNullOrWhiteSpace(customer.CustomerId))
    {
        http.Response.StatusCode = 400;
        return;
    }
    var customerRepository = new CustomerRepository();
    var currentCustomer = customerRepository.GetCustomer(customer.CustomerId);
    if (currentCustomer == null)
    {
        http.Response.StatusCode = 404;
        return;
    }
    if (customerRepository.Update(customer))
    {
        customerRepository.Commit();
        http.Response.ContentType = "application/json";
        await http.Response.WriteAsync(JsonSerializer.Serialize(customer));
        return;
    }
    http.Response.StatusCode = 204;
    return;
}


async Task DeleteCustomer(HttpContext http)
{
    if (!http.Request.RouteValues.TryGet("id", out string id))
    {
        http.Response.StatusCode = 400;
        return;
    }
    var customerRepository = new CustomerRepository();
    var currentCustomer = customerRepository.GetCustomer(id);
    if (currentCustomer == null)
    {
        http.Response.StatusCode = 404;
        return;
    }
    if (customerRepository.Remove(currentCustomer))
    {
        customerRepository.Commit();
        http.Response.StatusCode = 200;
        return;
    }
    http.Response.StatusCode = 204;
    return;
}
