// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using Microsoft.DotNet.RemoteExecutor;

using ImageDull.ImageSharp.Formats;
using ImageDull.ImageSharp.Formats.Bmp;
using ImageDull.ImageSharp.Formats.Gif;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.Formats.Png;
using ImageDull.ImageSharp.Formats.Webp;
using ImageDull.ImageSharp.Tests.TestUtilities.ReferenceCodecs;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming
namespace ImageDull.ImageSharp.Tests
{
    public class TestEnvironmentTests
    {
        public TestEnvironmentTests(ITestOutputHelper output)
            => this.Output = output;

        private ITestOutputHelper Output { get; }

        private void CheckPath(string path)
        {
            this.Output.WriteLine(path);
            Assert.True(Directory.Exists(path));
        }

        [Fact]
        public void SolutionDirectoryFullPath()
            => this.CheckPath(TestEnvironment.SolutionDirectoryFullPath);

        [Fact]
        public void InputImagesDirectoryFullPath()
            => this.CheckPath(TestEnvironment.InputImagesDirectoryFullPath);

        [Fact]
        public void ExpectedOutputDirectoryFullPath()
            => this.CheckPath(TestEnvironment.ReferenceOutputDirectoryFullPath);

        [Fact]
        public void GetReferenceOutputFileName()
        {
            string actual = Path.Combine(TestEnvironment.ActualOutputDirectoryFullPath, @"foo\bar\lol.jpeg");
            string expected = TestEnvironment.GetReferenceOutputFileName(actual);

            this.Output.WriteLine(expected);
            Assert.Contains(TestEnvironment.ReferenceOutputDirectoryFullPath, expected);
        }

        [Theory]
        [InlineData("lol/foo.png", typeof(ImageSharpPngEncoderWithDefaultConfiguration))]
        [InlineData("lol/Rofl.bmp", typeof(BmpEncoder))]
        [InlineData("lol/Baz.JPG", typeof(JpegEncoder))]
        [InlineData("lol/Baz.gif", typeof(GifEncoder))]
        [InlineData("lol/foobar.webp", typeof(WebpEncoder))]
        public void GetReferenceEncoder_ReturnsCorrectEncoders(string fileName, Type expectedEncoderType)
        {
            IImageEncoder encoder = TestEnvironment.GetReferenceEncoder(fileName);
            Assert.IsType(expectedEncoderType, encoder);
        }

        [Theory]
        [InlineData("lol/foo.png", typeof(PngDecoder))]
        [InlineData("lol/Rofl.bmp", typeof(BmpDecoder))]
        [InlineData("lol/Baz.JPG", typeof(JpegDecoder))]
        [InlineData("lol/Baz.gif", typeof(GifDecoder))]
        [InlineData("lol/foobar.webp", typeof(WebpDecoder))]
        public void GetReferenceDecoder_ReturnsCorrectDecoders(string fileName, Type expectedDecoderType)
        {
            IImageDecoder decoder = TestEnvironment.GetReferenceDecoder(fileName);
            Assert.IsType(expectedDecoderType, decoder);
        }

        // RemoteExecutor does not work with "dotnet xunit" used to run tests on 32 bit .NET Framework:
        // https://github.com/SixLabors/ImageSharp/blob/381dff8640b721a34b1227c970fcf6ad6c5e3e72/ci-test.ps1#L30
        public static bool IsNot32BitNetFramework = !TestEnvironment.IsFramework || TestEnvironment.Is64BitProcess;

        [ConditionalFact(nameof(IsNot32BitNetFramework))]
        public void RemoteExecutor_FailingRemoteTestShouldFailLocalTest()
        {
            static void FailingCode()
            {
                Assert.False(true);
            }

            Assert.ThrowsAny<RemoteExecutionException>(() => RemoteExecutor.Invoke(FailingCode).Dispose());
        }
    }
}
