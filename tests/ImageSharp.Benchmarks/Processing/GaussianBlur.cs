// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;

namespace ImageDull.ImageSharp.Benchmarks.Samplers
{
    [Config(typeof(Config.MultiFramework))]
    public class GaussianBlur
    {
        [Benchmark]
        public void Blur()
        {
            using var image = new Image<Rgba32>(Configuration.Default, 400, 400, Color.White);
            image.Mutate(c => c.GaussianBlur());
        }
    }
}
