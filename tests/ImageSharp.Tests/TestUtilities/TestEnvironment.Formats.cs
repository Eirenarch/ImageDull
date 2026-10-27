// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using ImageDull.ImageSharp.Formats;
using ImageDull.ImageSharp.Formats.Bmp;
using ImageDull.ImageSharp.Formats.Gif;
using ImageDull.ImageSharp.Formats.Jpeg;
using ImageDull.ImageSharp.Formats.Pbm;
using ImageDull.ImageSharp.Formats.Png;
using ImageDull.ImageSharp.Formats.Tga;
using ImageDull.ImageSharp.Formats.Webp;
using ImageDull.ImageSharp.Tests.TestUtilities.ReferenceCodecs;

namespace ImageDull.ImageSharp.Tests;

public static partial class TestEnvironment
{
    private static readonly Lazy<Configuration> ConfigurationLazy = new(CreateDefaultConfiguration);

    internal static Configuration Configuration => ConfigurationLazy.Value;

    internal static IImageDecoder GetReferenceDecoder(string filePath)
    {
        IImageFormat format = GetImageFormat(filePath);
        return Configuration.ImageFormatsManager.FindDecoder(format);
    }

    internal static IImageEncoder GetReferenceEncoder(string filePath)
    {
        IImageFormat format = GetImageFormat(filePath);
        return Configuration.ImageFormatsManager.FindEncoder(format);
    }

    internal static IImageFormat GetImageFormat(string filePath)
    {
        string extension = Path.GetExtension(filePath);

        return Configuration.ImageFormatsManager.FindFormatByFileExtension(extension);
    }

    private static void ConfigureCodecs(
        this Configuration cfg,
        IImageFormat imageFormat,
        IImageDecoder decoder,
        IImageEncoder encoder,
        IImageFormatDetector detector)
    {
        cfg.ImageFormatsManager.SetDecoder(imageFormat, decoder);
        cfg.ImageFormatsManager.SetEncoder(imageFormat, encoder);
        cfg.ImageFormatsManager.AddImageFormatDetector(detector);
    }

    private static Configuration CreateDefaultConfiguration()
    {
        var cfg = new Configuration(
            new JpegConfigurationModule(),
            new GifConfigurationModule(),
            new PbmConfigurationModule(),
            new TgaConfigurationModule(),
            new WebpConfigurationModule());

        cfg.ConfigureCodecs(
            PngFormat.Instance,
            new PngDecoder(),
            new ImageSharpPngEncoderWithDefaultConfiguration(),
            new PngImageFormatDetector());

        cfg.ConfigureCodecs(
            BmpFormat.Instance,
            new BmpDecoder(),
            new BmpEncoder(),
            new BmpImageFormatDetector());

        return cfg;
    }
}
