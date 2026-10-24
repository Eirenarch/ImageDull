// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using BenchmarkDotNet.Attributes;

using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests;

namespace SixLabors.ImageSharp.Benchmarks.Codecs
{
    [MarkdownExporter]
    [HtmlExporter]
    [Config(typeof(Config.ShortMultiFramework))]
    public class EncodeTiff
    {
        private Image<Rgba32> core;

        private Configuration configuration;

        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage);

        [Params(TestImages.Tiff.Calliphora_RgbUncompressed)]
        public string TestImage { get; set; }

        [Params(
            TiffCompression.None,
            TiffCompression.Deflate,
            TiffCompression.Lzw,
            TiffCompression.PackBits,
            TiffCompression.CcittGroup3Fax,
            TiffCompression.Ccitt1D)]
        public TiffCompression Compression { get; set; }

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.core == null)
            {
                this.configuration = new Configuration();
                this.core = Image.Load<Rgba32>(this.configuration, this.TestImageFullPath);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
            => this.core.Dispose();

        [Benchmark(Baseline = true, Description = "ImageSharp Tiff")]
        public void TiffCore()
        {
            TiffPhotometricInterpretation photometricInterpretation = TiffPhotometricInterpretation.Rgb;

            var encoder = new TiffEncoder() { Compression = this.Compression, PhotometricInterpretation = photometricInterpretation };
            using var memoryStream = new MemoryStream();
            this.core.SaveAsTiff(memoryStream, encoder);
        }
    }
}
