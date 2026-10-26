// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using Xunit;

namespace SixLabors.ImageSharp.Tests.Helpers
{
    public class ColorNumericsTests
    {
        [Theory]
        [InlineData(0.2f, 0.7f, 0.1f, 256, 140)]
        [InlineData(0.5f, 0.5f, 0.5f, 256, 128)]
        [InlineData(0.5f, 0.5f, 0.5f, 65536, 32768)]
        [InlineData(0.2f, 0.7f, 0.1f, 65536, 36069)]
        public void GetBT709Luminance_WithVector4(float x, float y, float z, int luminanceLevels, int expected)
        {
            // arrange
            var vector = new Vector4(x, y, z, 0.0f);

            // act
            int actual = ColorNumerics.GetBT709Luminance(ref vector, luminanceLevels);

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1f, -1f, -1f, 256, 0)]
        [InlineData(-32767f, -32767f, -32767f, 256, 0)]
        [InlineData(2f, 2f, 2f, 256, 255)]
        [InlineData(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, 256, 255)]
        [InlineData(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, 256, 0)]
        [InlineData(float.NaN, 0.5f, 0.5f, 256, 0)]
        [InlineData(float.PositiveInfinity, float.NegativeInfinity, 0f, 65536, 0)]
        public void GetBT709Luminance_WithOutOfRangeVector4_IsClampedToValidIndex(float x, float y, float z, int luminanceLevels, int expected)
        {
            var vector = new Vector4(x, y, z, 0.0f);

            int actual = ColorNumerics.GetBT709Luminance(ref vector, luminanceLevels);

            Assert.Equal(expected, actual);
        }

        // TODO: We need to test all ColorNumerics methods!
    }
}
