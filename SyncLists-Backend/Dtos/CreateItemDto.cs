using System.ComponentModel.DataAnnotations;

namespace synclists_backend.Dtos;

public record CreateItemDto(
    [Required] [StringLength(50)]string Name,
    int ListId
    );