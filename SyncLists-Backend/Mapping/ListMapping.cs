using synclists_backend.Dtos;
using synclists_backend.Entities;

namespace synclists_backend.Mapping;

public static class ListMapping
{
    public static List ToEntity(this CreateListDto list)
    {
        return new List()
        {
            Name = list.Name,
        };
    }

    public static ListDto ToDto(this List list)
    {
        return new ListDto(
            list.Id,
            list.Name
        );
    }
}