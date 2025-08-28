namespace synclists_backend.Entities;

public class Item
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int ListId { get; set; }
    public List? List { get; set; }
    
    public bool IsChecked { get; set; } = false;

    public int Order { get; set; }
}