// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using System.IO;
using System.Threading;
using BenchmarkDotNet.Attributes;
using Pfim;
using ImageDull.ImageSharp.Formats.Tga;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Tests;

namespace ImageDull.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(Config.ShortMultiFramework))]
    public class DecodeTga
    {
        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, this.TestImage);

        private readonly PfimConfig pfimConfig = new PfimConfig(allocator: new PfimAllocator());

        private byte[] data;

        [Params(TestImages.Tga.Bit24BottomLeft)]
        public string TestImage { get; set; }

        [GlobalSetup]
        public void SetupData()
            => this.data = File.ReadAllBytes(this.TestImageFullPath);

        [Benchmark(Baseline = true, Description = "ImageSharp Tga")]
        public int TgaImageSharp()
        {
            using var image = Image.Load<Bgr24>(this.data, new TgaDecoder());
            return image.Width;
        }

        [Benchmark(Description = "Pfim Tga")]
        public int TgaPfim()
        {
            using var image = Targa.Create(this.data, this.pfimConfig);
            return image.Width;
        }

        private class PfimAllocator : IImageAllocator
        {
            private int rented;
            private readonly ArrayPool<byte> shared = ArrayPool<byte>.Shared;

            public byte[] Rent(int size) => this.shared.Rent(size);

            public void Return(byte[] data)
            {
                Interlocked.Decrement(ref this.rented);
                this.shared.Return(data);
            }

            public int Rented => this.rented;
        }
    }
}
