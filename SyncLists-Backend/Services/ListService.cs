using Microsoft.EntityFrameworkCore;
using synclists_backend.Data;
using synclists_backend.Dtos;
using synclists_backend.Endpoints;
using synclists_backend.Mapping;

namespace synclists_backend.Services;

public class ListService
{
    private readonly ListStoreContext _dbContext;

    public ListService(ListStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListDto>> GetAllAsync()
    {
        return await _dbContext.Lists
            .Select(list => list.ToDto())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ListDto?> GetByIdAsync(int id)
    {
        var list = await _dbContext.Lists.FirstOrDefaultAsync(l => l.Id == id);
        return list?.ToDto();
    }

    public async Task<List<ItemDto>> GetItemsAsync(int listId)
    {
        return await _dbContext.Items
            .Where(item => item.ListId == listId)
            .Select(item => item.ToDto())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ListDto> CreateAsync(CreateListDto newList)
    {
        var list = newList.ToEntity();
        _dbContext.Lists.Add(list);
        await _dbContext.SaveChangesAsync();
        
        var lists = await GetAllAsync();
        await WSEndpoints.BroadcastMessageAsync(0,"lists_updated", lists);
        
        return list.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        await _dbContext.Items
            .Where(i => i.ListId == id)
            .ExecuteDeleteAsync();
        
        await _dbContext.Lists
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync();

        var lists = await GetAllAsync();
        await WSEndpoints.BroadcastMessageAsync(0,"lists_updated", lists);
    }
    
}