using System.Drawing;

namespace Un1ver5e.Bot.Services.Graphics
{
    public interface IMonoColorImageGenerator
    {
        /// <summary>
        /// Generates and returns a 1024x1024 image filled with <paramref name="color"/>.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>A <see cref="Stream"/> of the .jpg file.</returns>
        public ValueTask<Stream> GetImage(Disqord.Color color);
    }
}
