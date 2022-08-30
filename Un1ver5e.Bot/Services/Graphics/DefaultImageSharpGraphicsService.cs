using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Un1ver5e.Bot.Services.Graphics
{
    public class DefaultImageSharpGraphicsService :
        IMonoColorImageGenerator
    {
        public async ValueTask<Stream> GetImage(Disqord.Color color)
        {
            using Image<Rgb24> image = new(1024, 1024, new Rgb24(color.R, color.G, color.B));
            Stream result = new MemoryStream();

            await image.SaveAsJpegAsync(result);

            result.Position = 0;
            return result;
        }
    }
}
