// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Tests.TestUtilities.ImageComparison;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Processing.Processors.Filters
{
    [Trait("Category", "Processors")]
    [GroupOutput("Filters")]
    public class BlackWhiteTest
    {
        [Theory]
        [WithTestPatternImages(48, 48, PixelTypes.Rgba32)]
        public void ApplyBlackWhiteFilter<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            provider.RunValidatingProcessorTest(ctx => ctx.BlackWhite(), comparer: ImageComparer.TolerantPercentage(0.002f));
        }
    }
}
