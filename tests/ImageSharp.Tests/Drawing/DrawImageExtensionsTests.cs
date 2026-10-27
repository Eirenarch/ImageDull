// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;
using ImageDull.ImageSharp.Processing.Processors.Drawing;
using ImageDull.ImageSharp.Tests.Processing;

using Xunit;

namespace ImageDull.ImageSharp.Tests.Drawing
{
    public class DrawImageExtensionsTests : BaseImageOperationsExtensionTest
    {
        [Fact]
        public void DrawImage_OpacityOnly_VerifyGraphicOptionsTakenFromContext()
        {
            // non-default values as we cant easly defect usage otherwise
            this.options.AlphaCompositionMode = PixelAlphaCompositionMode.Xor;
            this.options.ColorBlendingMode = PixelColorBlendingMode.Screen;

            this.operations.DrawImage(null, 0.5f);
            DrawImageProcessor dip = this.Verify<DrawImageProcessor>();

            Assert.Equal(0.5, dip.Opacity);
            Assert.Equal(this.options.AlphaCompositionMode, dip.AlphaCompositionMode);
            Assert.Equal(this.options.ColorBlendingMode, dip.ColorBlendingMode);
        }

        [Fact]
        public void DrawImage_OpacityAndBlending_VerifyGraphicOptionsTakenFromContext()
        {
            // non-default values as we cant easly defect usage otherwise
            this.options.AlphaCompositionMode = PixelAlphaCompositionMode.Xor;
            this.options.ColorBlendingMode = PixelColorBlendingMode.Screen;

            this.operations.DrawImage(null, PixelColorBlendingMode.Multiply, 0.5f);
            DrawImageProcessor dip = this.Verify<DrawImageProcessor>();

            Assert.Equal(0.5, dip.Opacity);
            Assert.Equal(this.options.AlphaCompositionMode, dip.AlphaCompositionMode);
            Assert.Equal(PixelColorBlendingMode.Multiply, dip.ColorBlendingMode);
        }

        [Fact]
        public void DrawImage_LocationAndOpacity_VerifyGraphicOptionsTakenFromContext()
        {
            // non-default values as we cant easly defect usage otherwise
            this.options.AlphaCompositionMode = PixelAlphaCompositionMode.Xor;
            this.options.ColorBlendingMode = PixelColorBlendingMode.Screen;

            this.operations.DrawImage(null, Point.Empty, 0.5f);
            DrawImageProcessor dip = this.Verify<DrawImageProcessor>();

            Assert.Equal(0.5, dip.Opacity);
            Assert.Equal(this.options.AlphaCompositionMode, dip.AlphaCompositionMode);
            Assert.Equal(this.options.ColorBlendingMode, dip.ColorBlendingMode);
        }

        [Fact]
        public void DrawImage_LocationAndOpacityAndBlending_VerifyGraphicOptionsTakenFromContext()
        {
            // non-default values as we cant easly defect usage otherwise
            this.options.AlphaCompositionMode = PixelAlphaCompositionMode.Xor;
            this.options.ColorBlendingMode = PixelColorBlendingMode.Screen;

            this.operations.DrawImage(null, Point.Empty, PixelColorBlendingMode.Multiply, 0.5f);
            DrawImageProcessor dip = this.Verify<DrawImageProcessor>();

            Assert.Equal(0.5, dip.Opacity);
            Assert.Equal(this.options.AlphaCompositionMode, dip.AlphaCompositionMode);
            Assert.Equal(PixelColorBlendingMode.Multiply, dip.ColorBlendingMode);
        }
    }
}
