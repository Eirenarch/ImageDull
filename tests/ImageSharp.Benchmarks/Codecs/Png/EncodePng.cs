// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Png;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Tests;

namespace ImageDull.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(Config.Short))]
    public class EncodePng
    {
        private Stream bmpStream;
        private Image<Rgba32> bmpCore;

        [Params(false)]
        public bool LargeImage { get; set; }

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.bmpStream == null)
            {
                string path = Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.LargeImage ? TestImages.Jpeg.Baseline.Jpeg420Exif : TestImages.Bmp.Car);
                this.bmpStream = File.OpenRead(path);
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

        [Benchmark(Baseline = true, Description = "ImageSharp Png")]
        public void PngCore()
        {
            using var memoryStream = new MemoryStream();
            var encoder = new PngEncoder { FilterMethod = PngFilterMethod.None };
            this.bmpCore.SaveAsPng(memoryStream, encoder);
        }
    }
}
