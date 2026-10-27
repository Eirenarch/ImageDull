// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Filters;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Processing.Filters
{
    [Trait("Category", "Processors")]
    public class InvertTest : BaseImageOperationsExtensionTest
    {
        [Fact]
        public void Invert_InvertProcessorDefaultsSet()
        {
            this.operations.Invert();
            this.Verify<InvertProcessor>();
        }

        [Fact]
        public void Pixelate_rect_PixelateProcessorDefaultsSet()
        {
            this.operations.Invert(this.rect);
            this.Verify<InvertProcessor>(this.rect);
        }
    }
}
