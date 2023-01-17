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
        Console.WriteLine($"Plain bytes: {pixelBytes[0]}");
        byte[] encrBytes = Helper.Encrypt(pixelBytes, password: "myPassword");
        Console.WriteLine($"Cipher bytes: {encrBytes[0]}");
        using (var encrImage = Image.LoadPixelData<Rgb24>(encrBytes, image.Width, image.Height))
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
        Console.WriteLine($"cipher bytes: {pixelBytes[0]}");
        byte[] decrBytes = Helper.Decrypt(pixelBytes, password: "myPassword");
        Console.WriteLine($"Plain bytes: {decrBytes[0]}");
        using (var decrImage = Image.LoadPixelData<Rgb24>(decrBytes, image.Width, image.Height))
        {
            decrImage.Save(Path.Combine(DecrFolder, "decrBaboon.bmp"));
        }
    }
}
