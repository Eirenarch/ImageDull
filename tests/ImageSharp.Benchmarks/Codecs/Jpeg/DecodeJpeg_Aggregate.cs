// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Tests;

// ReSharper disable InconsistentNaming
namespace ImageDull.ImageSharp.Benchmarks.Codecs.Jpeg
{
    /// <summary>
    /// An expensive Jpeg benchmark, running on a wide range of input images,
    /// showing aggregate results.
    /// </summary>
    [Config(typeof(Config.Short))]
    public class DecodeJpeg_Aggregate : MultiImageBenchmarkBase
    {
        protected override IEnumerable<string> InputImageSubfoldersOrFiles =>
            new[]
                {
                    TestImages.Jpeg.BenchmarkSuite.Jpeg400_SmallMonochrome,
                    TestImages.Jpeg.BenchmarkSuite.Jpeg420Exif_MidSizeYCbCr,
                    TestImages.Jpeg.BenchmarkSuite.Lake_Small444YCbCr,
                    TestImages.Jpeg.BenchmarkSuite.MissingFF00ProgressiveBedroom159_MidSize420YCbCr,
                    TestImages.Jpeg.BenchmarkSuite.ExifGetString750Transform_Huge420YCbCr,
                };

        [Params(InputImageCategory.AllImages)]
        public override InputImageCategory InputCategory { get; set; }

        [Benchmark(Baseline = true)]
        public void ImageSharp()
            => this.ForEachStream(ms => Image.Load<Rgba32>(ms, new JpegDecoder()));
    }
}
