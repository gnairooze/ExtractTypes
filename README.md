# Extract Types
![.NET Core](https://github.com/gnairooze/ExtractTypes/workflows/.NET%20Core/badge.svg?branch=master)

Extract type properties from dotnet assembly and write it to CSV file.

it needs [dotnet core 3.1 runtime package](https://dotnet.microsoft.com/download/dotnet-core/3.1).

-----

## Command line help

**--help**  show this help. This help also shows when no arguments added

**--assembly-Path** set the assembly path that contains the type

**--type-fullname** set the type full name to be exported

### Example:

```shell:
Extractor.exe --assembly-Path"c:\temp\assembly.dll" --type-fullname"Namespace.ClassName"
```
