# netcore.utilities ![](https://img.shields.io/github/license/iowacomputergurus/netcore.utilities.svg)

![Build Status](https://github.com/IowaComputerGurus/netcore.utilities/actions/workflows/ci-build.yml/badge.svg)

## NuGet Package Information
ICG.NetCore.Utilities ![](https://img.shields.io/nuget/v/icg.netcore.utilities.svg) ![](https://img.shields.io/nuget/dt/icg.netcore.utilities.svg)


A collection of helpful utilities for working with .NET 5 + projects.  These items are used by the IowaComputerGurus Team to aid in unit testing and other common tasks

## Usage

### Installation

Install from NuGet

```
Install-Package ICG.AspNetCore.Utilities
```

### Register Dependencies

Inside of of your project's Startus.cs within the RegisterServices method add this line of code.

```
services.UseIcgNetCoreUtilities();
```

### Included C# Objects

| Object | Purpose |
| ---- | --- |
| IDirectory Provider | Provides a shim around the System.IO.Directory object to allow for unit testing. |
| IGuidProvider | Provides a shim around the System.Guid object to allow for unit testing of Guid operations.  |
| IFileProvider | Provides a shim around the System.IO.File object to allow for unit testing of file related operations. |
| IPathProvider | Provides a shim around the System.IO.Path object to allow for unit testing of path related operations | 
| ITimeProvider | Provides a shim around the System.DateTime object to allow for unit testing of date operations |
| ITimeSpanProvider | Provides a shim around the System.TimeSpan object to allow for unit testing/injection of TimeSpan operations |
| IUrlSlugGenerator | Provides a service that will take input and generate a url friendly slug from the content |

Detailed information can be found in the XML Comment documentation for the objects, we are working to add to this document as well.

