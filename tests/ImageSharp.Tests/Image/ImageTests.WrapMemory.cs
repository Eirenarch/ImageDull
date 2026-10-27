// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImageDull.ImageSharp.Advanced;
using ImageDull.ImageSharp.Common.Helpers;
using ImageDull.ImageSharp.Memory;
using ImageDull.ImageSharp.Metadata;
using ImageDull.ImageSharp.PixelFormats;
using ImageDull.ImageSharp.Processing;
using Xunit;

// ReSharper disable InconsistentNaming
namespace ImageDull.ImageSharp.Tests
{
    public partial class ImageTests
    {
        public class WrapMemory
        {
            public sealed class CastMemoryManager<TFrom, TTo> : MemoryManager<TTo>
                where TFrom : unmanaged
                where TTo : unmanaged
            {
                private readonly Memory<TFrom> memory;

                public CastMemoryManager(Memory<TFrom> memory)
                {
                    this.memory = memory;
                }

                /// <inheritdoc/>
                protected override void Dispose(bool disposing)
                {
                }

                /// <inheritdoc/>
                public override Span<TTo> GetSpan()
                {
                    return MemoryMarshal.Cast<TFrom, TTo>(this.memory.Span);
                }

                /// <inheritdoc/>
                public override MemoryHandle Pin(int elementIndex = 0)
                {
                    int byteOffset = elementIndex * Unsafe.SizeOf<TTo>();
                    int shiftedOffset = Math.DivRem(byteOffset, Unsafe.SizeOf<TFrom>(), out int remainder);

                    if (remainder != 0)
                    {
                        ThrowHelper.ThrowArgumentException("The input index doesn't result in an aligned item access", nameof(elementIndex));
                    }

                    return this.memory.Slice(shiftedOffset).Pin();
                }

                /// <inheritdoc/>
                public override void Unpin()
                {
                }
            }

            [Fact]
            public void WrapMemory_CreatedImageIsCorrect()
            {
                var cfg = Configuration.CreateDefaultInstance();
                var metaData = new ImageMetadata();

                var array = new Rgba32[25];
                var memory = new Memory<Rgba32>(array);

                using (var image = Image.WrapMemory(cfg, memory, 5, 5, metaData))
                {
                    Assert.True(image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> imageMem));
                    ref Rgba32 pixel0 = ref imageMem.Span[0];
                    Assert.True(Unsafe.AreSame(ref array[0], ref pixel0));

                    Assert.Equal(cfg, image.GetConfiguration());
                    Assert.Equal(metaData, image.Metadata);
                }
            }

            [Fact]
            public void WrapMemory_FromBytes_CreatedImageIsCorrect()
            {
                var cfg = Configuration.CreateDefaultInstance();
                var metaData = new ImageMetadata();

                var array = new byte[25 * Unsafe.SizeOf<Rgba32>()];
                var memory = new Memory<byte>(array);

                using (var image = Image.WrapMemory<Rgba32>(cfg, memory, 5, 5, metaData))
                {
                    Assert.True(image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> imageMem));
                    ref Rgba32 pixel0 = ref imageMem.Span[0];
                    Assert.True(Unsafe.AreSame(ref Unsafe.As<byte, Rgba32>(ref array[0]), ref pixel0));

                    Assert.Equal(cfg, image.GetConfiguration());
                    Assert.Equal(metaData, image.Metadata);
                }
            }

            [Fact]
            public unsafe void WrapMemory_FromPointer_CreatedImageIsCorrect()
            {
                var cfg = Configuration.CreateDefaultInstance();
                var metaData = new ImageMetadata();

                var array = new Rgba32[25];

                fixed (void* ptr = array)
                {
                    using (var image = Image.WrapMemory<Rgba32>(cfg, ptr, 5, 5, metaData))
                    {
                        Assert.True(image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> imageMem));
                        Span<Rgba32> imageSpan = imageMem.Span;
                        ref Rgba32 pixel0 = ref imageSpan[0];
                        Assert.True(Unsafe.AreSame(ref array[0], ref pixel0));
                        ref Rgba32 pixel_1 = ref imageSpan[imageSpan.Length - 1];
                        Assert.True(Unsafe.AreSame(ref array[array.Length - 1], ref pixel_1));

                        Assert.Equal(cfg, image.GetConfiguration());
                        Assert.Equal(metaData, image.Metadata);
                    }
                }
            }

            [Theory]
            [InlineData(0, 5, 5)]
            [InlineData(20, 5, 5)]
            [InlineData(1023, 32, 32)]
            public void WrapMemory_MemoryOfT_InvalidSize(int size, int height, int width)
            {
                var array = new Rgba32[size];
                var memory = new Memory<Rgba32>(array);

                Assert.Throws<ArgumentException>(() => Image.WrapMemory(memory, height, width));
            }

            [Theory]
            [InlineData(25, 5, 5)]
            [InlineData(26, 5, 5)]
            [InlineData(2, 1, 1)]
            [InlineData(1024, 32, 32)]
            [InlineData(2048, 32, 32)]
            public void WrapMemory_MemoryOfT_ValidSize(int size, int height, int width)
            {
                var array = new Rgba32[size];
                var memory = new Memory<Rgba32>(array);

                Image.WrapMemory(memory, height, width);
            }

            private class TestMemoryOwner<T> : IMemoryOwner<T>
            {
                public bool Disposed { get; private set; }

                public Memory<T> Memory { get; set; }

                public void Dispose() => this.Disposed = true;
            }

            [Theory]
            [InlineData(0, 5, 5)]
            [InlineData(20, 5, 5)]
            [InlineData(1023, 32, 32)]
            public void WrapMemory_IMemoryOwnerOfT_InvalidSize(int size, int height, int width)
            {
                var array = new Rgba32[size];
                var memory = new TestMemoryOwner<Rgba32> { Memory = array };

                Assert.Throws<ArgumentException>(() => Image.WrapMemory(memory, height, width));
            }

            [Theory]
            [InlineData(25, 5, 5)]
            [InlineData(26, 5, 5)]
            [InlineData(2, 1, 1)]
            [InlineData(1024, 32, 32)]
            [InlineData(2048, 32, 32)]
            public void WrapMemory_IMemoryOwnerOfT_ValidSize(int size, int height, int width)
            {
                var array = new Rgba32[size];
                var memory = new TestMemoryOwner<Rgba32> { Memory = array };

                using (var img = Image.WrapMemory<Rgba32>(memory, width, height))
                {
                    Assert.Equal(width, img.Width);
                    Assert.Equal(height, img.Height);

                    img.ProcessPixelRows(accessor =>
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            var arrayIndex = width * i;

                            Span<Rgba32> rowSpan = accessor.GetRowSpan(i);
                            ref Rgba32 r0 = ref rowSpan[0];
                            ref Rgba32 r1 = ref array[arrayIndex];

                            Assert.True(Unsafe.AreSame(ref r0, ref r1));
                        }
                    });
                }

                Assert.True(memory.Disposed);
            }

            [Theory]
            [InlineData(0, 5, 5)]
            [InlineData(20, 5, 5)]
            [InlineData(1023, 32, 32)]
            public void WrapMemory_IMemoryOwnerOfByte_InvalidSize(int size, int height, int width)
            {
                var array = new byte[size * Unsafe.SizeOf<Rgba32>()];
                var memory = new TestMemoryOwner<byte> { Memory = array };

                Assert.Throws<ArgumentException>(() => Image.WrapMemory<Rgba32>(memory, height, width));
            }

            [Theory]
            [InlineData(25, 5, 5)]
            [InlineData(26, 5, 5)]
            [InlineData(2, 1, 1)]
            [InlineData(1024, 32, 32)]
            [InlineData(2048, 32, 32)]
            public void WrapMemory_IMemoryOwnerOfByte_ValidSize(int size, int height, int width)
            {
                var pixelSize = Unsafe.SizeOf<Rgba32>();
                var array = new byte[size * pixelSize];
                var memory = new TestMemoryOwner<byte> { Memory = array };

                using (var img = Image.WrapMemory<Rgba32>(memory, width, height))
                {
                    Assert.Equal(width, img.Width);
                    Assert.Equal(height, img.Height);

                    img.ProcessPixelRows(acccessor =>
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            var arrayIndex = pixelSize * width * i;

                            Span<Rgba32> rowSpan = acccessor.GetRowSpan(i);
                            ref Rgba32 r0 = ref rowSpan[0];
                            ref Rgba32 r1 = ref Unsafe.As<byte, Rgba32>(ref array[arrayIndex]);

                            Assert.True(Unsafe.AreSame(ref r0, ref r1));
                        }
                    });
                }

                Assert.True(memory.Disposed);
            }

            [Theory]
            [InlineData(0, 5, 5)]
            [InlineData(20, 5, 5)]
            [InlineData(1023, 32, 32)]
            public void WrapMemory_MemoryOfByte_InvalidSize(int size, int height, int width)
            {
                var array = new byte[size * Unsafe.SizeOf<Rgba32>()];
                var memory = new Memory<byte>(array);

                Assert.Throws<ArgumentException>(() => Image.WrapMemory<Rgba32>(memory, height, width));
            }

            [Theory]
            [InlineData(25, 5, 5)]
            [InlineData(26, 5, 5)]
            [InlineData(2, 1, 1)]
            [InlineData(1024, 32, 32)]
            [InlineData(2048, 32, 32)]
            public void WrapMemory_MemoryOfByte_ValidSize(int size, int height, int width)
            {
                var array = new byte[size * Unsafe.SizeOf<Rgba32>()];
                var memory = new Memory<byte>(array);

                Image.WrapMemory<Rgba32>(memory, height, width);
            }

            [Theory]
            [InlineData(0, 5, 5)]
            [InlineData(20, 5, 5)]
            [InlineData(26, 5, 5)]
            [InlineData(2, 1, 1)]
            [InlineData(1023, 32, 32)]
            public unsafe void WrapMemory_Pointer_Null(int size, int height, int width)
            {
                Assert.Throws<ArgumentException>(() => Image.WrapMemory<Rgba32>((void*)null, height, width));
            }
        }
    }
}
