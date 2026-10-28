// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

namespace ImageDull.ImageSharp.Benchmarks.Codecs.Jpeg
{
    [Config(typeof(Config.Short))]
    public class YCbCrColorConversion : ColorConversionBenchmark
    {
        public YCbCrColorConversion()
            : base(3)
        {
        }

        [Benchmark]
        public void Scalar()
        {
            var values = new JpegColorConverterBase.ComponentValues(this.Input, 0);

            new JpegColorConverterBase.FromYCbCrScalar(8).ConvertToRgbInplace(values);
        }

        [Benchmark]
        public void SimdVector8()
        {
            var values = new JpegColorConverterBase.ComponentValues(this.Input, 0);

            new JpegColorConverterBase.FromYCbCrVector(8).ConvertToRgbInplace(values);
        }

        [Benchmark]
        public void SimdVectorAvx()
        {
            var values = new JpegColorConverterBase.ComponentValues(this.Input, 0);

            new JpegColorConverterBase.FromYCbCrAvx(8).ConvertToRgbInplace(values);
        }
    }
}
