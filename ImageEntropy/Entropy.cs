using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace ImageEntropy;

/// <summary>
/// The <c>Entropy</c> class.
/// Contains all methods for calculating information entropy of an image.
/// </summary>
public static class Entropy
{
    /// <summary>
    /// Calculates the information entropy for the Red, Green, and Blue channels of an image separately.
    /// </summary>
    /// <returns>A Tuple containing the entropy values for each channel (R, G, B).</returns>
    public static (double Red, double Green, double Blue) CalculateRgbEntropy(Image<Rgb24> image)
    {
        var histogram = CreateImageHistogram(image);
        double totalPixels = image.Width * image.Height;

        double redEntropy = CalculateChannelEntropy(histogram.Red, totalPixels);
        double greenEntropy = CalculateChannelEntropy(histogram.Green, totalPixels);
        double blueEntropy = CalculateChannelEntropy(histogram.Blue, totalPixels);

        return (redEntropy, greenEntropy, blueEntropy);
    }

    /// <summary>
    /// Calculates the entropy from the histogram data of a single color channel.
    /// </summary>
    private static double CalculateChannelEntropy(int[] channelHistogram, double totalPixels)
    {
        double entropy = channelHistogram
            .Where(frequency => frequency > 0)
            .Select(frequency =>
            {
                double probability = frequency / totalPixels;
                return probability * Math.Log2(probability);
            })
            .Sum();

        return -entropy;
    }

    /// <summary>
    /// Creates a color histogram for an image.
    /// </summary>
    private static Histogram CreateImageHistogram(Image<Rgb24> image)
    {
        int[] red = new int[256];
        int[] green = new int[256];
        int[] blue = new int[256];

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> row = accessor.GetRowSpan(y);
                foreach (ref Rgb24 pixel in row)
                {
                    red[pixel.R]++;
                    green[pixel.G]++;
                    blue[pixel.B]++;
                }
            }
        });

        return new Histogram { Red = red, Green = green, Blue = blue };
    }
}