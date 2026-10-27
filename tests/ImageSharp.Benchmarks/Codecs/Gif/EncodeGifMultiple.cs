// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Gif;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Quantization;

namespace ImageDull.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(Config.ShortMultiFramework))]
    public class EncodeGifMultiple : MultiImageBenchmarkBase.WithImagesPreloaded
    {
        [Params(InputImageCategory.AllImages)]
        public override InputImageCategory InputCategory { get; set; }

        protected override IEnumerable<string> InputImageSubfoldersOrFiles => new[] { "Gif/" };

        [Benchmark(Baseline = true, Description = "EncodeGifMultiple - ImageSharp")]
        public void EncodeGifImageSharp()
            => this.ForEachImageSharpImage((img, ms) =>
            {
                var options = new GifEncoder
                {
                    Quantizer = new WebSafePaletteQuantizer(new QuantizerOptions { Dither = KnownDitherings.Bayer4x4 })
                };

                img.Save(ms, options);
                return null;
            });
    }
}
