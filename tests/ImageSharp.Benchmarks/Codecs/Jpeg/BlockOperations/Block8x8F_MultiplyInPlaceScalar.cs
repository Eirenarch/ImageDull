// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Jpeg.Components;

namespace ImageDull.ImageSharp.Benchmarks.Codecs.Jpeg.BlockOperations
{
    [Config(typeof(Config.HwIntrinsics_SSE_AVX))]
    public class Block8x8F_MultiplyInPlaceScalar
    {
        [Benchmark]
        public float MultiplyInPlaceScalar()
        {
            float f = 42F;
            Block8x8F b = default;
            b.MultiplyInPlace(f);
            return f;
        }
    }
}
