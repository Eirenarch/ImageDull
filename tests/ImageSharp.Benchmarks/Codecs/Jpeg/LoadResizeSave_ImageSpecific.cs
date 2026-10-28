// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Tests;

// ReSharper disable InconsistentNaming
namespace ImageDull.ImageSharp.Benchmarks.Codecs.Jpeg
{
    [Config(typeof(Config.Short))]
    public class LoadResizeSave_ImageSpecific
    {
        private readonly Configuration configuration = new Configuration(new JpegConfigurationModule());

        private byte[] sourceBytes;

        private byte[] destBytes;

        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage);

        [Params(
            TestImages.Jpeg.BenchmarkSuite.Lake_Small444YCbCr,
            TestImages.Jpeg.BenchmarkSuite.BadRstProgressive518_Large444YCbCr,
            TestImages.Jpeg.BenchmarkSuite.Jpeg420Exif_MidSizeYCbCr)]

        public string TestImage { get; set; }

        [Params(false, true)]
        public bool ParallelExec { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.configuration.MaxDegreeOfParallelism =
                this.ParallelExec ? Environment.ProcessorCount : 1;

            this.sourceBytes = File.ReadAllBytes(this.TestImageFullPath);

            this.destBytes = new byte[this.sourceBytes.Length * 2];
        }

        [Benchmark(Baseline = true)]
        public void ImageSharp()
        {
            using (var source = Image.Load(this.configuration, this.sourceBytes, new JpegDecoder { IgnoreMetadata = true }))
            using (var destStream = new MemoryStream(this.destBytes))
            {
                source.Mutate(c => c.Resize(source.Width / 4, source.Height / 4));
                source.SaveAsJpeg(destStream);
            }
        }
    }
}
