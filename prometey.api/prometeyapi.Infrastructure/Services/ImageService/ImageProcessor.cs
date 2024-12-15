using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace prometeyapi.Infrastructure.Services.ImageService
{
    public class ImageProcessor
    {
        public async Task<string> ProcessProfilePhotoAsync(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using var memoryStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync<Rgba32>(memoryStream);

            if (image.Width > 150 || image.Height > 150)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(150, 150),
                    Mode = ResizeMode.Max
                }));
            }

            int cropSize = Math.Min(image.Width, image.Height);
            image.Mutate(x => x.Crop(new Rectangle(
                (image.Width - cropSize) / 2,
                (image.Height - cropSize) / 2,
                cropSize,
                cropSize)));

            memoryStream.SetLength(0);
            int quality = 75;

            var encoder = new JpegEncoder { Quality = quality };
            await image.SaveAsJpegAsync(memoryStream, encoder);

            while (memoryStream.Length > 10 * 1024 && quality > 0)
            {
                memoryStream.SetLength(0);
                quality -= 5;
                encoder = new JpegEncoder { Quality = quality };
                await image.SaveAsJpegAsync(memoryStream, encoder);
            }

            byte[] resultImageBytes = memoryStream.ToArray();

            memoryStream.Close();
            return Convert.ToBase64String(resultImageBytes);
        }

        public async Task<string> ProcessGroupPhotoAsync(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using var memoryStream = new MemoryStream(imageBytes);

            using var image = await Image.LoadAsync<Rgba32>(memoryStream);

            if (image.Width > 150 || image.Height > 150)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(150, 150),
                    Mode = ResizeMode.Max
                }));
            }

            int quality = 75;

            var encoder = new JpegEncoder { Quality = quality };

            memoryStream.SetLength(0);

            await image.SaveAsJpegAsync(memoryStream, encoder);

            while (memoryStream.Length > 10 * 1024 && quality > 0)
            {
                memoryStream.SetLength(0);
                quality -= 5;
                encoder = new JpegEncoder { Quality = quality };
                await image.SaveAsJpegAsync(memoryStream, encoder);
            }

            byte[] resultImageBytes = memoryStream.ToArray();

            memoryStream.Close();
            return Convert.ToBase64String(resultImageBytes);
        }

        public async Task<string> ProcessPostOrApplicationPhotoAsync(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            using var memoryStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync<Rgba32>(memoryStream);

            if (image.Width > 1500 || image.Height > 1500)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(1500, 1500),
                    Mode = ResizeMode.Max
                }));
            }

            int quality = 90;
            long targetSize = 500 * 1024;

            using var resultStream = new MemoryStream();
            var encoder = new JpegEncoder { Quality = quality };

            await image.SaveAsJpegAsync(resultStream, encoder);

            while (resultStream.Length > targetSize && quality > 10)
            {
                quality -= 5;
                resultStream.SetLength(0);
                encoder = new JpegEncoder { Quality = quality };
                await image.SaveAsJpegAsync(resultStream, encoder);
            }

            byte[] resultImageBytes = resultStream.ToArray();

            memoryStream.Close();
            return Convert.ToBase64String(resultImageBytes);
        }
    }
}
