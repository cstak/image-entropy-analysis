using System.Runtime.CompilerServices;
using ImageEntropy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

Console.WriteLine("***Information Entropy of An 256x256 Image***");

string EncrFolder = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Encrypted")).FullName;
string DecrFolder = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Decrypted")).FullName;
string orgImage = Path.Combine(Environment.CurrentDirectory, "baboon.jpg");

EncryptImageAndCalculateEntropy();
DecryptImage();

void EncryptImageAndCalculateEntropy()
{
    using (Image<Rgb24> image = Image.Load<Rgb24>(orgImage))
    {
        image.Mutate(x => x
         .Resize(256, 256)
         .Grayscale());

        Console.WriteLine($"Entropy of Image is: {Entropy.CalculateEntropy(image)}");
        byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgb24>()];
        image.CopyPixelDataTo(pixelBytes);
        string encrBytesBase64 = Helper.Encrypt(pixelBytes, password: "myPassword");
        using (var encrImage = Image.LoadPixelData<Rgb24>(Convert.FromBase64String(encrBytesBase64), image.Width, image.Height))
        {
            encrImage.Save(Path.Combine(EncrFolder, "encrBaboon.bmp"));
            Console.WriteLine($"Entropy of Encrypted Image is: {Entropy.CalculateEntropy(encrImage)}");
        }
    }
}
void DecryptImage()
{
    using (Image<Rgb24> image = Image.Load<Rgb24>(Path.Combine(EncrFolder, "encrBaboon.bmp")))
    {
        byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgb24>()];
        image.CopyPixelDataTo(pixelBytes);
        string decrBytesBase64 = Helper.Decrypt(pixelBytes, password: "myPassword");
        using (var decrImage = Image.LoadPixelData<Rgb24>(Convert.FromBase64String(decrBytesBase64), image.Width, image.Height))
        {
            decrImage.Save(Path.Combine(DecrFolder, "decrBaboon.bmp"));
        }
    }
}
