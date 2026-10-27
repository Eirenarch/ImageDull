// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Filters;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Processing.Filters
{
    [Trait("Category", "Processors")]
    public class BlackWhiteTest : BaseImageOperationsExtensionTest
    {
        [Fact]
        public void BlackWhite_CorrectProcessor()
        {
            this.operations.BlackWhite();
            this.Verify<BlackWhiteProcessor>();
        }

        [Fact]
        public void BlackWhite_rect_CorrectProcessor()
        {
            this.operations.BlackWhite(this.rect);
            this.Verify<BlackWhiteProcessor>(this.rect);
        }
    }
}
