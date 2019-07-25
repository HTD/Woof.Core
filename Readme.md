# Woof.Core 2.0

**.NET Framework** extensions created by **[CodeDog Ltd.](https://codedog.pl)**

Distributed under [MIT License](https://en.wikipedia.org/wiki/MIT_License).
(c)2019 by CodeDog Ltd., All rights reserved.

---

.NET Standard extensions.

Made to never reinvent the wheel and to keep simple things as simple and zen as possible.

Little code pearls, a part of what makes CodeDog special.

---

## Modules (namespace: module list)
- Algorithms: ArrayFisherYates, HashCode, Fast PRNG-s
- AssemblyEx: AssemblyInfo.
- Core: ApplicationDirectory, ExpandoObjectExtensions, MimeMapping, Resource, ResourceAttachment, XTemplate
- SystemEx: BufferEventArgs, BufferPool, CommandLineArguments, DateRange, DGuid, DiagnosticStream, Download, ExceptionEventArgs, IniFile, ItemEventArgs, Paged, PathTools, PercentEventArgs, TimeTrigger

---

### Naming convention

Namespaces extending .NET Framework features are named similar, but with "Ex" suffix.
There are good reasons for it: related features need related names but
any part of the userspace namespace CANNOT match any part of Microsoft framework namespace
or the name resolution in the project will break.

## Usage

Either nuget, or copy the cs files if single exe is necessary.

## Testing

Unit tests require .NET Core SDK 3.0.

---

## Disclaimer

Please report any issues to the [toolkit developer](mailto:it@codedog.pl).

Woof Toolkit is a work in progress in constant development,
however it's carefully maintained with production code quality.