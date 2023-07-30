using BuildingBlocks.Core.Domain;

namespace Postfy.Services.Network.Shared.Models;

public class Media : ValueObject
{
    public Media()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
    public int Position { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
        yield return Type;
        yield return Position;
    }
}
