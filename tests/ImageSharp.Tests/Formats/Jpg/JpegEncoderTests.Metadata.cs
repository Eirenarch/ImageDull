// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.PixelFormats;

using Xunit;

namespace ImageDull.ImageSharp.Tests.Formats.Jpg
{
    [Trait("Format", "Jpg")]
    public partial class JpegEncoderTests
    {
        [Theory]
        [WithFile(TestImages.Jpeg.Issues.ValidExifArgumentNullExceptionOnEncode, PixelTypes.Rgba32)]
        public void Encode_WithValidExifProfile_DoesNotThrowException<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Exception ex = Record.Exception(() =>
            {
                var encoder = new JpegEncoder();
                var stream = new MemoryStream();

                using Image<TPixel> image = provider.GetImage(JpegDecoder);
                image.Save(stream, encoder);
            });

            Assert.Null(ex);
        }
    }
}
