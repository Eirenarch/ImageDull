// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Tests;

namespace SixLabors.ImageSharp.Benchmarks.Codecs.Jpeg
{
    [Config(typeof(Config.ShortMultiFramework))]
    public class DecodeJpegParseStreamOnly
    {
        [Params(TestImages.Jpeg.BenchmarkSuite.Lake_Small444YCbCr)]
        public string TestImage { get; set; }

        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage);

        private byte[] jpegBytes;

        [GlobalSetup]
        public void Setup()
            => this.jpegBytes = File.ReadAllBytes(this.TestImageFullPath);

        [Benchmark(Baseline = true, Description = "JpegDecoderCore.ParseStream")]
        public void ParseStream()
        {
            using var memoryStream = new MemoryStream(this.jpegBytes);
            using var bufferedStream = new BufferedReadStream(Configuration.Default, memoryStream);

            using var decoder = new JpegDecoderCore(Configuration.Default, new JpegDecoder { IgnoreMetadata = true });
            var scanDecoder = new HuffmanScanDecoder(bufferedStream, new NoopSpectralConverter(), cancellationToken: default);
            decoder.ParseStream(bufferedStream, scanDecoder, cancellationToken: default);
        }

        // We want to test only stream parsing and scan decoding, we don't need to convert spectral data to actual pixels
        // Nor we need to allocate final pixel buffer
        // Note: this still introduces virtual method call overhead for baseline interleaved images
        // There's no way to eliminate it as spectral conversion is built into the scan decoding loop for memory footprint reduction
        private class NoopSpectralConverter : SpectralConverter
        {
            public override void ConvertStrideBaseline()
            {
            }

            public override void InjectFrameData(JpegFrame frame, IRawJpegData jpegData)
            {
            }
        }
    }
}
