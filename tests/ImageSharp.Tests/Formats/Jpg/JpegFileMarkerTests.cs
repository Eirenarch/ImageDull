// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.Formats.Jpeg.Components.Decoder;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Formats.Jpg
{
    [Trait("Format", "Jpg")]
    public class JpegFileMarkerTests
    {
        [Fact]
        public void MarkerConstructorAssignsProperties()
        {
            const byte app1 = JpegConstants.Markers.APP1;
            const int position = 5;
            var marker = new JpegFileMarker(app1, position);

            Assert.Equal(app1, marker.Marker);
            Assert.Equal(position, marker.Position);
            Assert.False(marker.Invalid);
            Assert.Equal(app1.ToString("X"), marker.ToString());
        }
    }
}
