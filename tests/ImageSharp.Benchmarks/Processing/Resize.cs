// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;
using System.IO;
using BenchmarkDotNet.Attributes;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Tests;

namespace ImageDull.ImageSharp.Benchmarks
{
    [Config(typeof(Config.MultiFramework))]
    public abstract class Resize<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private byte[] bytes = null;

        private Image<TPixel> sourceImage;

        protected Configuration Configuration { get; } = new Configuration(new JpegConfigurationModule());

        protected int DestSize { get; private set; }

        [GlobalSetup]
        public virtual void Setup()
        {
            if (this.bytes is null)
            {
                this.bytes = File.ReadAllBytes(Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, TestImages.Jpeg.Baseline.Snake));

                this.sourceImage = Image.Load<TPixel>(this.bytes);
                this.DestSize = this.sourceImage.Width / 2;
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.bytes = null;
            this.sourceImage.Dispose();
        }

        [Benchmark(Baseline = true, Description = "ImageSharp, MaxDegreeOfParallelism = 1")]
        public int ImageSharp_P1() => this.RunImageSharpResize(1);

        // Parallel cases have been disabled for fast benchmark execution.
        // Uncomment, if you are interested in parallel speedup

        /*
        [Benchmark(Description = "ImageSharp, MaxDegreeOfParallelism = 4")]
        public int ImageSharp_P4() => this.RunImageSharpResize(4);

        [Benchmark(Description = "ImageSharp, MaxDegreeOfParallelism = 8")]
         public int ImageSharp_P8() => this.RunImageSharpResize(8);
        */

        protected int RunImageSharpResize(int maxDegreeOfParallelism)
        {
            this.Configuration.MaxDegreeOfParallelism = maxDegreeOfParallelism;

            using (Image<TPixel> clone = this.sourceImage.Clone(this.ExecuteResizeOperation))
            {
                return clone.Width;
            }
        }

        protected abstract void ExecuteResizeOperation(IImageProcessingContext ctx);
    }

    public class Resize_Bicubic_Rgba32 : Resize<Rgba32>
    {
        protected override void ExecuteResizeOperation(IImageProcessingContext ctx)
            => ctx.Resize(this.DestSize, this.DestSize, KnownResamplers.Bicubic);
    }

    /// <summary>
    /// Is it worth to set a larger working buffer limit for resize?
    /// Conclusion: It doesn't really have an effect.
    /// </summary>
    public class Resize_Bicubic_Rgba32_CompareWorkBufferSizes : Resize_Bicubic_Rgba32
    {
        [Params(128, 512, 1024, 8 * 1024)]
        public int WorkingBufferSizeHintInKilobytes { get; set; }

        public override void Setup()
        {
            this.Configuration.WorkingBufferSizeHintInBytes = this.WorkingBufferSizeHintInKilobytes * 1024;
            base.Setup();
        }
    }

    public class Resize_Bicubic_Bgra32 : Resize<Bgra32>
    {
        protected override void ExecuteResizeOperation(IImageProcessingContext ctx)
            => ctx.Resize(this.DestSize, this.DestSize, KnownResamplers.Bicubic);
    }

    public class Resize_Bicubic_Rgb24 : Resize<Rgb24>
    {
        protected override void ExecuteResizeOperation(IImageProcessingContext ctx)
            => ctx.Resize(this.DestSize, this.DestSize, KnownResamplers.Bicubic);
    }

    public class Resize_BicubicCompand_Rgba32 : Resize<Rgba32>
    {
        protected override void ExecuteResizeOperation(IImageProcessingContext ctx)
            => ctx.Resize(this.DestSize, this.DestSize, KnownResamplers.Bicubic, true);
    }

    public class Resize_Bicubic_Compare_Rgba32_Rgb24
    {
        private Resize_Bicubic_Rgb24 rgb24;
        private Resize_Bicubic_Rgba32 rgba32;

        [GlobalSetup]
        public void Setup()
        {
            this.rgb24 = new Resize_Bicubic_Rgb24();
            this.rgb24.Setup();
            this.rgba32 = new Resize_Bicubic_Rgba32();
            this.rgba32.Setup();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.rgb24.Cleanup();
            this.rgba32.Cleanup();
        }

        [Benchmark(Baseline = true)]
        public void Rgba32() => this.rgba32.ImageSharp_P1();

        [Benchmark]
        public void Rgb24() => this.rgb24.ImageSharp_P1();
    }
}
