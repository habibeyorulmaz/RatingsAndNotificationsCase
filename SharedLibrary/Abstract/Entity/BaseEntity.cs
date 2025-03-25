using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Abstract.Entity;

public abstract partial class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}