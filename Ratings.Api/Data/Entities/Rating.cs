using SharedLibrary.Abstract.Entity;

namespace Ratings.Api.Data.Entities;

public class Rating : BaseEntity
{
    public Guid ProviderId { get; set; }
    public Guid CustomerId { get; set; }
    public int Score { get; set; }  // 1-10 arasında puan
    public string? Comment { get; set; }
}