<div align="center">

# 🚀 Zlib-ng.NET

**A .NET wrapper for zlib-ng**  

[![GitHub release](https://img.shields.io/github/v/release/NotOfficer/Zlib-ng.NET?logo=github)](https://github.com/NotOfficer/Zlib-ng.NET/releases/latest)
[![Nuget](https://img.shields.io/nuget/v/Zlib-ng.NET?logo=nuget)](https://www.nuget.org/packages/Zlib-ng.NET)
![Nuget Downloads](https://img.shields.io/nuget/dt/Zlib-ng.NET?logo=nuget)
[![GitHub issues](https://img.shields.io/github/issues/NotOfficer/Zlib-ng.NET?logo=github)](https://github.com/NotOfficer/Zlib-ng.NET/issues)
[![License](https://img.shields.io/github/license/NotOfficer/Zlib-ng.NET)](https://github.com/NotOfficer/Zlib-ng.NET/blob/master/LICENSE)

</div>

---

## 📦 Installation

Install via [NuGet](https://www.nuget.org/packages/Zlib-ng.NET):

```powershell
Install-Package Zlib-ng.NET
```

---

## ✨ Features

- Thin .NET wrapper over the native zlib-ng library
- Span-based `Compress`/`Decompress` with array, pointer, and `nint` overloads

---

## 🔧 Example Usage

```cs
using OodleDotNet;

using var zlib = new Zlibng(@"C:\Test\zlib-ng2.dll");
var compressedBuffer = System.IO.File.ReadAllBytes(@"C:\Test\Example.bin");
var decompressedBuffer = new byte[decompressedSize];
var result = zlib.Decompress(compressedBuffer, decompressedBuffer);
```

---

## 🤝 Contributing

Contributions are **welcome and appreciated**!

Whether it's fixing a typo, suggesting an improvement, or submitting a pull request — every bit helps.

---

## 📄 License

This project is licensed under the [MIT License](https://github.com/NotOfficer/Zlib-ng.NET/blob/master/LICENSE).

---

<div align="center">

⭐️ Star the repo if you find it useful!  
Feel free to open an issue if you have any questions or feedback.

</div>
