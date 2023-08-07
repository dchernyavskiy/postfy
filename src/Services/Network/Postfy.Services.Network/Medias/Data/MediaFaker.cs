using Bogus;
using Postfy.Services.Network.Shared.Models;

namespace Postfy.Services.Network.Medias.Data;

internal sealed class MediaFaker : Faker<Media>
{
    private static int _position = 1;

    public MediaFaker()
    {
        CustomInstantiator(
            f =>
            {
                return new Media() {Position = _position++, Url = f.Image.PicsumUrl()};
            });
    }
}
