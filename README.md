<h1 align="center">ImageDull</h1>

<div align="center">

[![License: Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

</div>

### ImageDull is a fork of [ImageSharp](https://github.com/sixLabors/ImageSharp) v2.1.13 (The last version under Apache license)

Since ImageSharp 2 is no longer supported this fork addresses recent vulnerability advisories by removing features. 

- The target .NET version is 8. Platforms older than .NET 8 have been removed
- TIFF support has been removed (this was the cause for several recent vulnerabilities)
- https://github.com/advisories/GHSA-gwg2-r3hj-4w44 and https://github.com/advisories/GHSA-j3p4-wp97-rph4 have been fixed with the help of AI

This fork will not get new features, in fact it is likely to lose features as more vulnerabilities are discovered. I am not providing support. If you need more features, improved performance or better support go and get a license for [ImageSharp](https://github.com/sixLabors/ImageSharp) from Six Labors.

## Credit

ImageDull is based on [ImageSharp](https://github.com/SixLabors/ImageSharp) by [Six Labors and contributors](https://github.com/SixLabors/ImageSharp/graphs/contributors). They did almost all of the work in this repository. Version 2.1.13 is the last version that Six Labors released under the Apache License 2.0.

ImageDull is not affiliated with, endorsed by, or supported by Six Labors. Do not send them questions about this fork.

## License
  
- ImageDull is licensed under the [Apache License, Version 2.0](https://opensource.org/licenses/Apache-2.0)
