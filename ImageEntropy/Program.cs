using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageEntropy;


const string Password = "mySecretPassword123!";
const int TargetWidth = 256;
const int TargetHeight = 256;
const string InputImageName = "baboon.png";

Console.WriteLine("--- Color Image Encryption and Channel-Based Entropy Analysis ---");

string inputImagePath = Path.Combine(Environment.CurrentDirectory, InputImageName);
string encryptedDir = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "EncryptedOutput")).FullName;
string decryptedDir = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "DecryptedOutput")).FullName;

string encryptedImagePath = Path.Combine(encryptedDir, "encrypted_image.png");
string decryptedImagePath = Path.Combine(decryptedDir, "decrypted_image.png");

try
{
    if (!File.Exists(inputImagePath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nERROR: Input image not found -> '{inputImagePath}'");
        Console.WriteLine("Please ensure 'baboon.jpg' exists in the root directory");
        Console.ResetColor();
        return;
    }

    // --- ENCRYPTION STAGE ---
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n--- STAGE 1: ENCRYPTION ---");
    Console.ResetColor();

    using var originalImage = Image.Load<Rgb24>(inputImagePath);
    originalImage.Mutate(ctx => ctx.Resize(new ResizeOptions
    {
        Size = new Size(TargetWidth, TargetHeight),
        Mode = ResizeMode.Crop
    }));
    Console.WriteLine($"1. Original image '{InputImageName}' loaded and resized to {TargetWidth}x{TargetHeight}.");

    var originalEntropy = Entropy.CalculateRgbEntropy(originalImage);
    Console.WriteLine("2. Channel-based entropy of the original image:");
    Console.WriteLine($"   - Red: {originalEntropy.Red:F4}, Green: {originalEntropy.Green:F4}, Blue: {originalEntropy.Blue:F4}");

    byte[] originalPixels = GetPixelBytes(originalImage);
    byte[] encryptedBytes = Helper.Encrypt(originalPixels, Password);
    Console.WriteLine($"3. Pixel data encrypted with AES. (Size: {encryptedBytes.Length} bytes)");

    using (var containerImage = EmbedDataInImage(encryptedBytes))
    {
        containerImage.Save(encryptedImagePath);
        Console.WriteLine($"4. Encrypted data embedded into the image '{Path.GetFileName(encryptedImagePath)}' ({containerImage.Width}x{containerImage.Height}).");

        var encryptedEntropy = Entropy.CalculateRgbEntropy(containerImage);
        Console.WriteLine("5. Channel-based entropy of the encrypted image (note that the values approach ~8.0):");
        Console.WriteLine($"   - Red: {encryptedEntropy.Red:F4}, Green: {encryptedEntropy.Green:F4}, Blue: {encryptedEntropy.Blue:F4}");
    }

    // --- DECRYPTION STAGE ---
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n--- STAGE 2: DECRYPTION ---");
    Console.ResetColor();

    using var loadedContainerImage = Image.Load<Rgb24>(encryptedImagePath);
    Console.WriteLine($"1. Encrypted data container '{Path.GetFileName(encryptedImagePath)}' loaded.");

    byte[] extractedEncryptedBytes = ExtractDataFromImage(loadedContainerImage);
    Console.WriteLine($"2. Embedded encrypted data extracted. (Size: {extractedEncryptedBytes.Length} bytes)");

    byte[] decryptedBytes = Helper.Decrypt(extractedEncryptedBytes, Password);
    Console.WriteLine($"3. Data decrypted. (Size: {decryptedBytes.Length} bytes)");

    using (var decryptedImage = Image.LoadPixelData<Rgb24>(decryptedBytes, TargetWidth, TargetHeight))
    {
        decryptedImage.Save(decryptedImagePath);
        Console.WriteLine($"4. Decrypted image saved as '{Path.GetFileName(decryptedImagePath)}'.");
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n*** All operations completed successfully! ***");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nUNEXPECTED ERROR: {ex.Message}");
    Console.ResetColor();
    Console.WriteLine(ex.StackTrace);
}

// --- Local Functions ---

/// <summary>
/// Embeds the given byte array (payload) into a new, validly-sized 2D image.
/// </summary>
Image<Rgb24> EmbedDataInImage(byte[] payload)
{
    // Prepare the data to be embedded (length prefix + actual data)
    byte[] lengthAsBytes = BitConverter.GetBytes(payload.Length);
    byte[] dataToEmbed = new byte[4 + payload.Length];
    lengthAsBytes.CopyTo(dataToEmbed, 0);
    payload.CopyTo(dataToEmbed, 4);

    // Calculate the total number of pixels required to store the data
    const int bytesPerPixel = 3; // For Rgb24
    int totalPixelsNeeded = (int)Math.Ceiling(dataToEmbed.Length / (double)bytesPerPixel);

    // Determine suitable 2D dimensions for the container image
    const int maxWidth = 65535;
    int width;
    int height;

    if (totalPixelsNeeded <= maxWidth)
    {
        // If the data fits in a single row without exceeding the limit, use that.
        width = totalPixelsNeeded;
        height = 1;
    }
    else
    {
        // Otherwise, calculate a more square-like shape to fit the data.
        width = (int)Math.Ceiling(Math.Sqrt(totalPixelsNeeded));
        if (width > maxWidth) width = maxWidth;
        height = (int)Math.Ceiling(totalPixelsNeeded / (double)width);
    }

    // Create the final pixel buffer for the full rectangular image
    int finalBufferSize = width * height * bytesPerPixel;
    byte[] containerPixels = new byte[finalBufferSize];

    // Copy the data to embed into the start of the buffer.
    // The rest of the buffer will remain black (0s), acting as padding.
    dataToEmbed.CopyTo(containerPixels, 0);

    // Create and return the image from the pixel data
    return Image.LoadPixelData<Rgb24>(containerPixels, width, height);
}

/// <summary>
/// Extracts the embedded encrypted data from a container image.
/// </summary>
byte[] ExtractDataFromImage(Image<Rgb24> containerImage)
{
    byte[] containerPixels = GetPixelBytes(containerImage);
    // Read the length of the actual data from the first 4 bytes
    int storedLength = BitConverter.ToInt32(containerPixels, 0);
    byte[] encryptedBytesWithSalt = new byte[storedLength];
    // Extract the exact number of bytes needed
    Array.Copy(containerPixels, 4, encryptedBytesWithSalt, 0, storedLength);
    return encryptedBytesWithSalt;
}

byte[] GetPixelBytes(Image<Rgb24> image)
{
    var pixelBytes = new byte[image.Width * image.Height * 3];
    image.CopyPixelDataTo(pixelBytes);
    return pixelBytes;
}
