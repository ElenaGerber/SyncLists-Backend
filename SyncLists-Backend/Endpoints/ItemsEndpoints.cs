using synclists_backend.Dtos;
using synclists_backend.Services;

namespace synclists_backend.Endpoints;

public static class ItemsEndpoints
{
    const string GetGameEndpointName = "GetShoppingItem";

    public static RouteGroupBuilder MapShoppingItemsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("items");
        
        group.MapGet("/", async (ItemService service) =>
        {
            var items = await service.GetAllAsync();
            return Results.Ok(items);
        });

        group.MapGet("/{id}", async (int id, ItemService service) =>
        {
            var item = await service.GetByIdAsync(id);
            return item is null ? Results.NotFound() : Results.Ok(item);
        }).WithName(GetGameEndpointName);

        group.MapPost("/", async (CreateItemDto newItem, ItemService service) =>
        {
            var itemDto = await service.CreateAsync(newItem);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = itemDto.Id }, itemDto);
        }).WithParameterValidation();
        
        group.MapPut("/{id}", async (int id, ItemDto updatedItem, ItemService service) =>
        {
            var item = await service.UpdateAsync(id, updatedItem);
            return item is null ? Results.NotFound() : Results.Ok(item);
        }).WithParameterValidation();

        group.MapDelete("/{id}", async (int id, ItemService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

        return group;
    }
}