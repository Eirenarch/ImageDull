// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

#if OS_WINDOWS
using System.Security.Principal;
using BenchmarkDotNet.Diagnostics.Windows;
#endif
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;

namespace ImageDull.ImageSharp.Benchmarks
{
    public partial class Config : ManualConfig
    {
        public Config()
        {
            this.AddDiagnoser(MemoryDiagnoser.Default);

#if OS_WINDOWS
            if (this.IsElevated)
            {
                this.AddDiagnoser(new NativeMemoryProfiler());
            }
#endif

            this.SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(50);
        }

        public class Short : Config
        {
            public Short()
                => this.AddJob(Job.Default.WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3));
        }

#if OS_WINDOWS
        private bool IsElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
#endif
    }
}
