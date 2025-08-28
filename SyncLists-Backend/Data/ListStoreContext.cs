using Microsoft.EntityFrameworkCore;
using synclists_backend.Entities;

namespace synclists_backend.Data;

public class ListStoreContext(DbContextOptions<ListStoreContext> options) : DbContext(options)
{
    public DbSet<List> Lists => Set<List>();

    public DbSet<Item> Items => Set<Item>();
}