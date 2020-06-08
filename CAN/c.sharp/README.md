# C# based CAN examples

The following files are provided:

* `vs_can_api_wrapper.cs` - C# wrapper class
* `Doxyfile.doxyfile` - Doxygen[1] configuration file to produce HTML
  documentation for C# wrapper class
* `TestDrive.cs` - API usage example

## Requirements

C# sharp wrapper (`vs_can_api_wrapper.cs`) invokes functions provided by
`vs_can_api.dll`. Before using the wrapper make sure this DLL can be
found by Windows either via placing it in one of the folders mentioned in
PATH environment variable or via placing in your application's binary
folder.

## Usage

To use the class, just create a new project with Visual Studio and add
`vs_can_api_wrapper.cs` to this project. The same can be done with
the `TestDrive.cs` - create a project and add both `vs_can_api_wrapper.cs` and
`TestDrive.cs`. The `TestDrive.cs` is configured to open a first found device
and using self reception mode, so if for example USB-CAN Plus is installed
properly and there is a functional connection between USB-CAN and your CAN bus
(termination), the example program should run as it is.

## CMake support in Windows

If you have CMake version 3.8 and later as also Visual Studio, you can use
CMake to create your Visual Studio project. Usually, it is enough to open this
folder with Visual Studio, and it will automatically create a solution/project
using `CMakeLists.txt` file.

## .NET Core support

Both the `TestDrive.cs` and `vs_can_api_wrapper.cs` can be compiled and run via
the `dotnet` utility. Invoke the following command to create a sample console
project:

    dotnet new console --output sample1

Now copy `TestDrive.cs` as `Program.cs` to the `sample1` folder together with
the `vs_can_api_wrapper.cs` file and invoke the following command to run the
example:

    dotnet run --project sample1
    
[1] Doxygen project: http://www.stack.nl/~dimitri/doxygen/
