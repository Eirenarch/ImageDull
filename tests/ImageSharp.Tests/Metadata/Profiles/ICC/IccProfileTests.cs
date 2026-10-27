// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.Linq;
using ImageDull.ImageSharp.Metadata.Profiles.Icc;
using Xunit;

namespace ImageDull.ImageSharp.Tests.Metadata.Profiles.Icc
{
    [Trait("Profile", "Icc")]
    public class IccProfileTests
    {
        [Theory]
        [MemberData(nameof(IccTestDataProfiles.ProfileIdTestData), MemberType = typeof(IccTestDataProfiles))]
        public void CalculateHash_WithByteArray_CalculatesProfileHash(byte[] data, IccProfileId expected)
        {
            IccProfileId result = IccProfile.CalculateHash(data);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateHash_WithByteArray_DoesNotModifyData()
        {
            byte[] data = IccTestDataProfiles.Profile_Random_Array;
            var copy = new byte[data.Length];
            Buffer.BlockCopy(data, 0, copy, 0, data.Length);

            IccProfile.CalculateHash(data);

            Assert.Equal(data, copy);
        }

        [Theory]
        [MemberData(nameof(IccTestDataProfiles.ProfileValidityTestData), MemberType = typeof(IccTestDataProfiles))]
        public void CheckIsValid_WithProfiles_ReturnsValidity(byte[] data, bool expected)
        {
            var profile = new IccProfile(data);

            bool result = profile.CheckIsValid();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entries_WithTruncatedMpetClut_DoesNotAllocateFromDeclaredDimensions()
        {
            // 15 input and 15 output channels with 3 grid points each would size a CLUT of roughly 3^15 entries,
            // but the profile ends right after the grid descriptor.
            byte[] data = CreateProfileWithTruncatedMpetClut();
            var profile = new IccProfile(data);

            long before = GC.GetAllocatedBytesForCurrentThread();
            IccTagDataEntry[] entries = profile.Entries;
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.DoesNotContain(entries, e => e.TagSignature == IccProfileTag.AToB0);
            Assert.True(allocated < 1_000_000, $"Parsing a {data.Length} byte profile allocated {allocated} bytes.");
        }

        private static byte[] CreateProfileWithTruncatedMpetClut()
        {
            const int tagOffset = 144;
            const int elementOffset = 168;
            var profile = new byte[200];

            void U32(int offset, uint value) => BinaryPrimitives.WriteUInt32BigEndian(profile.AsSpan(offset, 4), value);
            void U16(int offset, ushort value) => BinaryPrimitives.WriteUInt16BigEndian(profile.AsSpan(offset, 2), value);

            U32(0, (uint)profile.Length);
            U32(8, 0x04000000);     // version 4
            U32(12, 0x6D6E7472);    // mntr
            U32(16, 0x52474220);    // RGB
            U32(20, 0x58595A20);    // XYZ
            U32(36, 0x61637370);    // acsp

            // Tag table: one A2B0 tag.
            U32(128, 1);
            U32(132, (uint)IccProfileTag.AToB0);
            U32(136, tagOffset);
            U32(140, 56);

            // mpet tag with a single element, located 24 bytes into the tag.
            U32(tagOffset, 0x6D706574);
            U16(tagOffset + 8, 0);
            U16(tagOffset + 10, 0);
            U32(tagOffset + 12, 1);
            U32(tagOffset + 16, 24);
            U32(tagOffset + 20, 32);

            // clut element: 15 in, 15 out, 3 grid points per channel and no CLUT values.
            U32(elementOffset, 0x636C7574);
            U16(elementOffset + 4, 15);
            U16(elementOffset + 6, 15);
            for (int i = 0; i < 15; i++)
            {
                profile[elementOffset + 8 + i] = 3;
            }

            return profile;
        }
    }
}
