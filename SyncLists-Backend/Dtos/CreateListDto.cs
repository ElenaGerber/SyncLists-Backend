using System.ComponentModel.DataAnnotations;

namespace synclists_backend.Dtos;

public record CreateListDto(
    [Required] [StringLength(50)]string Name
    );