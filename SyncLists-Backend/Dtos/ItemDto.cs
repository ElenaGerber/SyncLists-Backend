namespace synclists_backend.Dtos;

public record ItemDto(
    int Id,
    string Name,
    int ListId,
    bool IsChecked,
    int Order
);