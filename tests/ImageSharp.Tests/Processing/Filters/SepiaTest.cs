// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Filters;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Processing.Filters
{
    [Trait("Category", "Processors")]
    public class SepiaTest : BaseImageOperationsExtensionTest
    {
        [Fact]
        public void Sepia_amount_SepiaProcessorDefaultsSet()
        {
            this.operations.Sepia();
            this.Verify<SepiaProcessor>();
        }

        [Fact]
        public void Sepia_amount_rect_SepiaProcessorDefaultsSet()
        {
            this.operations.Sepia(this.rect);
            this.Verify<SepiaProcessor>(this.rect);
        }
    }
}
