// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Gif;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Quantization;
using ImageDull.ImageSharp.Tests;

namespace ImageDull.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(Config.ShortMultiFramework))]
    public class EncodeGif
    {
        private Stream bmpStream;
        private Image<Rgba32> bmpCore;

        private readonly GifEncoder encoder = new GifEncoder
        {
            Quantizer = new WebSafePaletteQuantizer(new QuantizerOptions { Dither = KnownDitherings.Bayer4x4 })
        };

        [Params(TestImages.Bmp.Car, TestImages.Png.Rgb48Bpp)]
        public string TestImage { get; set; }

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.bmpStream == null)
            {
                this.bmpStream = File.OpenRead(Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage));
                this.bmpCore = Image.Load<Rgba32>(this.bmpStream);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.bmpStream.Dispose();
            this.bmpStream = null;
            this.bmpCore.Dispose();
        }

        [Benchmark(Baseline = true, Description = "ImageSharp Gif")]
        public void GifImageSharp()
        {
            using var memoryStream = new MemoryStream();
            this.bmpCore.SaveAsGif(memoryStream, this.encoder);
        }
    }
}
