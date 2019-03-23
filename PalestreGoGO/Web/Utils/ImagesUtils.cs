using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Utils
{
    public static class ImagesUtils
    {
        public static Stream ResizeImage(Stream stream)
        {
            MemoryStream result = new MemoryStream();
            JpegEncoder encoder = new JpegEncoder();
            IImageFormat imageFormat;
            using (Image<Rgba32> image = Image.Load(stream, out imageFormat))
            {
                encoder.Quality = 75;
                image.SaveAsJpeg(result, encoder);
            }
            result.Position = 0;
            return result;
        }
    }
}
