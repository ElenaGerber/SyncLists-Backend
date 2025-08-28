using Microsoft.AspNetCore.Mvc;
using synclists_backend.Dtos;
using synclists_backend.Models;
using synclists_backend.Services;

namespace synclists_backend.Endpoints;

public static class ListsEndpoints
{
    const string GetListEndpointName = "GetShoppingList";

    public static RouteGroupBuilder MapShoppingListsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("lists");

        group.MapGet("/", async (ListService service) =>
        {
            var lists = await service.GetAllAsync();
            return Results.Ok(lists);
        });

        group.MapGet("/{id}", async (int id, ListService service) =>
        {
            var list = await service.GetByIdAsync(id);
            return list is null ? Results.NotFound() : Results.Ok(list);
        }).WithName(GetListEndpointName);

        group.MapGet("/{id}/items", async (int id, ListService service) =>
        {
            var items = await service.GetItemsAsync(id);
            return Results.Ok(items);
        });

        group.MapPost("/", async (CreateListDto newList, ListService service) =>
        {
            var listDto = await service.CreateAsync(newList);
            return Results.CreatedAtRoute(GetListEndpointName, new { id = listDto.Id }, listDto);
        }).WithParameterValidation();

        group.MapDelete("/{id}", async (int id, ListService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
        
        group.MapDelete("/{listId}/items", async (int listId,[FromBody] DeleteItemsRequest request, ItemService itemService) =>
        {
            await itemService.DeleteItemsAsync(listId, request);
            return Results.NoContent();
        });

        return group;
    }
}