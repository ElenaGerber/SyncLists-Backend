using synclists_backend.Dtos;
using synclists_backend.Entities;

namespace synclists_backend.Mapping;

public static class ItemMapping
{
    public static Item ToEntity(this CreateItemDto item)
    {
        return new Item()
        {
            Name = item.Name,
            ListId = item.ListId,
            IsChecked = false,
            Order = 0
        };
    }

    public static ItemDto ToDto(this Item item)
    {
        return new ItemDto(
            item.Id,
            item.Name,
            item.ListId,
            item.IsChecked,
            item.Order
        );
    }
}