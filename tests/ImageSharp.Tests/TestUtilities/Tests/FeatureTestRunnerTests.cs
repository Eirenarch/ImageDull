// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
#if SUPPORTS_RUNTIME_INTRINSICS
using System.Runtime.Intrinsics.X86;
#endif
using Xunit;
using Xunit.Abstractions;

namespace ImageDull.ImageSharp.Tests.TestUtilities.Tests;

public class FeatureTestRunnerTests
{
    public static TheoryData<HwIntrinsics, string[]> Intrinsics =>
        new()
        {
            { HwIntrinsics.DisableAES | HwIntrinsics.AllowAll, new string[] { "EnableAES", "AllowAll" } },
            { HwIntrinsics.DisableSIMD | HwIntrinsics.DisableHWIntrinsic, new string[] { "FeatureSIMD", "EnableHWIntrinsic" } },
            { HwIntrinsics.DisableSSE42 | HwIntrinsics.DisableAVX, new string[] { "EnableSSE42", "EnableAVX" } }
        };

    [Theory]
    [MemberData(nameof(Intrinsics))]
    public void ToFeatureCollectionReturnsExpectedResult(HwIntrinsics expectedIntrinsics, string[] expectedValues)
    {
        Dictionary<HwIntrinsics, string> features = expectedIntrinsics.ToFeatureKeyValueCollection();
        HwIntrinsics[] keys = features.Keys.ToArray();

        HwIntrinsics actualIntrinsics = keys[0];
        for (int i = 1; i < keys.Length; i++)
        {
            actualIntrinsics |= keys[i];
        }

        Assert.Equal(expectedIntrinsics, actualIntrinsics);

        IEnumerable<string> actualValues = features.Select(x => x.Value);
        Assert.Equal(expectedValues, actualValues);
    }

    [Fact]
    public void AllowsAllHwIntrinsicFeatures()
    {
        if (!Vector.IsHardwareAccelerated)
        {
            return;
        }

        FeatureTestRunner.RunWithHwIntrinsicsFeature(
            () => Assert.True(Vector.IsHardwareAccelerated),
            HwIntrinsics.AllowAll);
    }

#if SUPPORTS_RUNTIME_INTRINSICS
    [Fact]
    public void CanLimitHwIntrinsicBaseFeatures()
    {
        static void AssertDisabled()
        {
            Assert.False(Sse.IsSupported);
            Assert.False(Sse2.IsSupported);
            Assert.False(Aes.IsSupported);
            Assert.False(Pclmulqdq.IsSupported);
            Assert.False(Sse3.IsSupported);
            Assert.False(Ssse3.IsSupported);
            Assert.False(Sse41.IsSupported);
            Assert.False(Sse42.IsSupported);
            Assert.False(Popcnt.IsSupported);
            Assert.False(Avx.IsSupported);
            Assert.False(Fma.IsSupported);
            Assert.False(Avx2.IsSupported);
            Assert.False(Bmi1.IsSupported);
            Assert.False(Bmi2.IsSupported);
            Assert.False(Lzcnt.IsSupported);
        }

        FeatureTestRunner.RunWithHwIntrinsicsFeature(
            AssertDisabled,
            HwIntrinsics.DisableHWIntrinsic);
    }
#endif

    [Fact]
    public void CanLimitHwIntrinsicFeaturesWithSerializableParam()
    {
        static void AssertHwIntrinsicsFeatureDisabled(string serializable)
        {
            Assert.NotNull(serializable);
            Assert.NotNull(FeatureTestRunner.DeserializeForXunit<FakeSerializable>(serializable));

#if SUPPORTS_RUNTIME_INTRINSICS
            Assert.False(Sse.IsSupported);
#endif
        }

        FeatureTestRunner.RunWithHwIntrinsicsFeature(
            AssertHwIntrinsicsFeatureDisabled,
            HwIntrinsics.DisableSSE,
            new FakeSerializable());
    }

    public class FakeSerializable : IXunitSerializable
    {
        public void Deserialize(IXunitSerializationInfo info)
        {
        }

        public void Serialize(IXunitSerializationInfo info)
        {
        }
    }
}
