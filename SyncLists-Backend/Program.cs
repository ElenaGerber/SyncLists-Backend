using Microsoft.EntityFrameworkCore;
using synclists_backend.Data;
using synclists_backend.Endpoints;
using synclists_backend.Services;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ListStoreContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<ItemService>();

var app = builder.Build();

app.UseWebSockets();

app.UseCors("AllowReactApp");

app.MapShoppingItemsEndpoints();
app.MapShoppingListsEndpoints();
app.MapWebSocketEndpoints();

await app.MigrateDbAsync();

app.Run();