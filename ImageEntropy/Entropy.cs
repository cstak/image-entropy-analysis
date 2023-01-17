using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace ImageEntropy
{
    /// <summary>
    /// The <c>ImageEntropy</c> class.
    /// Contains all methods for calculating information entropy of an image.
    /// </summary>
    public class Entropy
    {
        public static double CalculateEntropy(Image<Rgb24> image)
        {
            var imageHistogram = ImageHistogram(image);
            double entropi = 0;
            for (int i = 0; i < 256; i++)
            {
                if (imageHistogram.red[i] == 0)
                    continue;
                else //Shannon function
                    entropi += CalculateProbability(imageHistogram.red[i], imageHistogram.red) * Math.Log((1 / CalculateProbability(imageHistogram.red[i], imageHistogram.red)), 2);
            }
            return entropi;
        }
        static (int[] red, int[] green, int[] blue) ImageHistogram(Image<Rgb24> image)
        {
            //histogram arrays of the pixel values
            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgb24 pixel = ref pixelRow[x];
                        //Fill the histogram arrays with number of appearance of current pixel values (RGB)
                        red[pixel.R]++;
                        green[pixel.G]++;
                        blue[pixel.B]++;
                    }
                }
            });
            return (red, green, blue);
        }
        static double CalculateProbability(int j, int[] k) => j / TotalPixelValue(k);
        static double TotalPixelValue(int[] i)
        {
            double totalValue = 0;
            for (int k = 0; k < 256; k++)
            {
                totalValue += i[k];
            }
            return totalValue;
        }
    }
}