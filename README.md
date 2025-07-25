# 🖼️ Image Encryption & Shannon Entropy Analysis 🔐

This project demonstrates a practical application of cryptography and information theory by encrypting an image and then analyzing the effectiveness of the encryption using Shannon entropy.

---

## 📜 Overview

> Information entropy is a key concept in the security analysis of cryptographic methods. It measures the amount of uncertainty or randomness in a set of data. A successful encryption algorithm should significantly increase the entropy of the original (plain) data, making the encrypted output appear as random as possible and removing any discernible patterns.

This simple .NET console application performs the following steps:

1. **Loads an original image** (e.g., `baboon.png`).
2. **Calculates the Shannon entropy** for each color channel (Red, Green, Blue) of the original image.
3. **Encrypts the image's pixel data** using the AES (Advanced Encryption Standard) algorithm.
4. **Embeds the encrypted data** into a new, lossless image file (e.g., a PNG).
5. **Calculates the Shannon entropy** of the encrypted image, demonstrating a significant increase in randomness, which indicates a strong encryption process.
6. **Decrypts the data** from the container image to restore the original image, proving the process is fully reversible.

For a detailed explanation of the algorithm and its mathematical background (in Turkish), please visit this page:

[Information Theory](https://kriptogram.net/blog/bilgi-entropisi/)

---

## 🚀 How to Use

1. Place an input image (e.g., `baboon.png`) in the project's execution directory.
2. Run the application.
3. Observe the console output, which displays the entropy values at each stage of the process.
4. Check the `EncryptedOutput` and `DecryptedOutput` folders to see the generated images.

*Feel free to explore and modify the code to experiment with different images or settings.*
