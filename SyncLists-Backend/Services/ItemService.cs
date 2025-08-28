using Microsoft.EntityFrameworkCore;
using synclists_backend.Data;
using synclists_backend.Dtos;
using synclists_backend.Endpoints;
using synclists_backend.Mapping;
using synclists_backend.Models;

namespace synclists_backend.Services;

public class ItemService
{
    private readonly ListStoreContext _dbContext;

    public ItemService(ListStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ItemDto>> GetAllAsync()
    {
        return await _dbContext.Items
            .Include(i => i.List)
            .Select(i => i.ToDto())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _dbContext.Items.FindAsync(id);
        return item?.ToDto();
    }
    
    public async Task<List<ItemDto>> GetAllByListIdAsync(int listId)
    {
        return await _dbContext.Items
            .Where(i => i.ListId == listId)
            .Select(i => i.ToDto())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto newItem)
    {
        var item = newItem.ToEntity();
        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync();
        
        var itemsInList = await _dbContext.Items
            .Where(i => i.ListId == item.ListId)
            .ToListAsync();
        
        item.Order = itemsInList.Max(i => i.Order )+1;
        await _dbContext.SaveChangesAsync();
        
        var updatedItems = await GetAllByListIdAsync(item.ListId);
        await WSEndpoints.BroadcastMessageAsync(item.ListId, "items_updated", updatedItems);

        return item.ToDto();
    }
    
    
    public async Task<ItemDto?> UpdateAsync(int id, ItemDto updatedItem)
    {
        var item = await _dbContext.Items.FindAsync(id);
        if (item is null) return null;
        
        item.IsChecked = updatedItem.IsChecked;
        item.Order = updatedItem.Order;
        
        await _dbContext.SaveChangesAsync();
        
        var updatedItems = await GetAllByListIdAsync(item.ListId);
        await WSEndpoints.BroadcastMessageAsync(item.ListId, "items_updated", updatedItems);

        return item.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        
        var item = await  GetByIdAsync(id);
        if (item is null)
        {
            return;
        }
        await _dbContext.Items.Where(i => i.Id == id).ExecuteDeleteAsync();
        var itemsInList = await GetAllByListIdAsync(item.ListId);
        await WSEndpoints.BroadcastMessageAsync(item.ListId, "items_updated", itemsInList);
    }
    
    public async Task DeleteItemsAsync(int listId, DeleteItemsRequest request)
    {
        var query = _dbContext.Items.Where(i => i.ListId == listId);

        switch (request.Action)
        {
            case "DeleteCheckedItems":
                query = query.Where(i => i.IsChecked);
                break;
        }

        await query.ExecuteDeleteAsync();

        var itemsInList = await GetAllByListIdAsync(listId);
        await WSEndpoints.BroadcastMessageAsync(listId, "items_updated", itemsInList);
    }
}