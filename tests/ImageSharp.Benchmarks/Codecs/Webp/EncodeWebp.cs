// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests;

namespace SixLabors.ImageSharp.Benchmarks.Codecs
{
    [MarkdownExporter]
    [HtmlExporter]
    [Config(typeof(Config.ShortMultiFramework))]
    public class EncodeWebp
    {
        private Image<Rgba32> webp;

        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage);

        [Params(TestImages.Png.Bike)] // The bike image will have all 3 transforms as lossless webp.
        public string TestImage { get; set; }

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.webp == null)
            {
                this.webp = Image.Load<Rgba32>(this.TestImageFullPath);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.webp.Dispose();
        }

        [Benchmark(Description = "ImageSharp Webp Lossy")]
        public void ImageSharpWebpLossy()
        {
            using var memoryStream = new MemoryStream();
            this.webp.Save(memoryStream, new WebpEncoder()
            {
                FileFormat = WebpFileFormatType.Lossy,
                Method = WebpEncodingMethod.Level4,
                UseAlphaCompression = false,
                FilterStrength = 60,
                SpatialNoiseShaping = 50,
                EntropyPasses = 1
            });
        }

        [Benchmark(Baseline = true, Description = "ImageSharp Webp Lossless")]
        public void ImageSharpWebpLossless()
        {
            using var memoryStream = new MemoryStream();
            this.webp.Save(memoryStream, new WebpEncoder()
            {
                FileFormat = WebpFileFormatType.Lossless,
                Method = WebpEncodingMethod.Level4,
                NearLossless = false,

                // This is equal to exact = false in libwebp, which is the default.
                TransparentColorMode = WebpTransparentColorMode.Clear
            });
        }
    }
}
