// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Bmp;

namespace ImageDull.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(Config.Short))]
    public class EncodeBmpMultiple : MultiImageBenchmarkBase.WithImagesPreloaded
    {
        protected override IEnumerable<string> InputImageSubfoldersOrFiles => new[] { "Bmp/", "Jpg/baseline" };

        [Benchmark(Baseline = true, Description = "EncodeBmpMultiple - ImageSharp")]
        public void EncodeBmpImageSharp()
            => this.ForEachImageSharpImage((img, ms) =>
            {
                img.Save(ms, new BmpEncoder());
                return null;
            });
    }
}
