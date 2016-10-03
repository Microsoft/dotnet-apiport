# .NET API Portability

This repository contains the source code for .NET Portability Analyzer tools and
dependencies.

## Usage

To use this tool, please refer to the [documentation](docs/HowTo/Introduction.md). 
For a quick introduction, check out [this video on Channel 9][Channel 9 Video]:

[<img src="https://sec.ch9.ms/ch9/031c/f3d7672b-dd71-4a18-a8b4-37573c08031c/DotNetPortabilityAnalyzer_960.jpg" width="480" />][Channel 9 Video]

## Projects

| Project | Description |
| :------- | :----------- |
| ApiPort | Console tool to access portability webservice | 
| ApiPort.Core | Cross-platform .NET Core application | 
| ApiPort.Vsix | Visual Studio Extension | 
| Microsoft.Fx.Portability [![][Version-Portability]][myget] | Provides common types for API Port |
| Microsoft.Fx.Portability.MetadataReader [![][Version-MetadataReader]][myget] | Implements a dependency finder based off of [System.Reflection.Metadata](https://github.com/dotnet/corefx/tree/master/src/System.Reflection.Metadata). The library will generate DocIds that conform to [these specifications](https://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx). |
| Microsoft.Fx.Portability.Offline [![][Version-Offline]][myget] | Provides access to data in an offline setting so network calls are not needed |
| Microsoft.Fx.Portability.Reporting.Html [![][Version-Reporting Html]][myget] | Provides an HTML report for ApiPort (used in offline mode) |
| Microsoft.Fx.Portability.Reporting.Json [![][Version-Reporting Json]][myget] | Provides a JSON reporter for ApiPort (used in offline mode) |

## Using this Repository

* Install [Visual Studio 2015 with Update 3][Visual Studio 2015]

### ApiPort.Core

__Prerequisites__

* Install [.NET Core 1.0.1](https://dot.net/core)

__Compiling, Debugging and Running__

* Change __Platform__ to `x64`
* Compile solution
* Run/debug the solution by:
    * Executing: `dotnet.exe ApiPort.exe [arguments]`
    * Debug `ApiPort.Core` project in Visual Studio

## How to Engage, Contribute and Provide Feedback

Here are some ways to contribute:
* [Update/Add recommended changes](docs/RecommendedChanges/README.md)
* Try things out!
* File issues
* Join in design conversations

Want to get more familiar with what's going on in the code?
* [Pull requests][PR]: [Open][PR-Open]/[Closed][PR-Closed]

Looking for something to work on? The list of [up-for-grabs issues][Issues-Open]
is a great place to start.

We're re-using the same contributing approach as .NET Core. You can check out 
the .NET Core [contributing guide][Contributing Guide] at the corefx repo wiki 
for more details.

* [How to Contribute][Contributing Guide]
    * [Contributing Guide][Contributing Guide]
    * [Developer Guide]

You are also encouraged to start a discussion on the .NET Foundation forums!

## Related Projects

For an overview of all the .NET related projects, have a look at the
[.NET home repository](https://github.com/Microsoft/dotnet).

## License

This project is licensed under the [MIT license](LICENSE).

[Channel 9 Video]: https://channel9.msdn.com/Blogs/Seth-Juarez/A-Brief-Look-at-the-NET-Portability-Analyzer
[Contributing Guide]: https://github.com/dotnet/corefx/wiki/Contributing
[Developer Guide]: https://github.com/dotnet/corefx/wiki/Developer-Guide
[Issues-Open]: https://github.com/Microsoft/dotnet-apiport/issues?q=is%3Aopen+is%3Aissue
[PR]: https://github.com/Microsoft/dotnet-apiport/pulls
[PR-Closed]: https://github.com/Microsoft/dotnet-apiport/pulls?q=is%3Apr+is%3Aclosed
[PR-Open]: https://github.com/Microsoft/dotnet-apiport/pulls?q=is%3Aopen+is%3Apr
[myget]: https://www.myget.org/gallery/dotnet-apiport
[Version-Portability]: https://img.shields.io/myget/dotnet-apiport/v/Microsoft.Fx.Portability.svg
[Version-MetadataReader]: https://img.shields.io/myget/dotnet-apiport/v/Microsoft.Fx.Portability.MetadataReader.svg
[Version-Offline]: https://img.shields.io/myget/dotnet-apiport/v/Microsoft.Fx.Portability.Offline.svg
[Version-Reporting Html]: https://img.shields.io/myget/dotnet-apiport/v/Microsoft.Fx.Portability.Reports.Html.svg
[Version-Reporting Json]: https://img.shields.io/myget/dotnet-apiport/v/Microsoft.Fx.Portability.Reports.Json.svg
[Visual Studio 2015]: http://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx