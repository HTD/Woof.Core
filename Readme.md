# WOOF Toolkit

**.NET Framework** extensions created by **[CodeDog Ltd.](http://codedog.pl)**

Distributed under [MIT License](https://en.wikipedia.org/wiki/MIT_License).
(c)2018 by CodeDog Ltd., All rights reserved.

---

.NET Standard extensions.

Made to never reinvent the wheel and to keep simple things as simple and zen as possible.

Little code pearls, a part of what makes CodeDog special.

---

## Modules

### Naming convention

Namespaces extending .NET Framework features are named similar, but with "Ex" suffix.
There are good reasons for it: related features need related names but
any part of the userspace namespace CANNOT match any part of Microsoft framework namespace
or the name resolution in the project will break.

WinForms and WPF tools are exception because Woof Toolkit mixes the two to benefit from both.
WPF rendering is faster, more flexible and easier to develop, however some core parts of
Win32 subsystem intrnally use WinForms. System tray and system shell are WinForms only.
WinForms is also faster and easier to develop for some particular cases.

### Namespaces

| Namespace               | Description                                           |
|:------------------------|:------------------------------------------------------|
| Algorithms              | Number crunching stuff, universal hash code class.    |
| AssemblyEx              | Tools for dealing with assemblies.                    |
| Automation              | Tools to automate configuration and setup process.    |
| Command                 | Build your own command shell!                         |
| ConsoleEx               | Advanced console tools.                               |
| Core                    | Tools for .NET Core server projects, universal tools. |
| ProcessEx               | Advanced inter-process communication tools.           |
| SecurityEx              | X509, Windows Security, identity and such.            |
| SystemEx                | System extended data types.                           |
| TextEx                  | CSV parser, XML helpers, pattern matching and such.   |
| VectorMath              | 2D and 3D vector types                                |

## Usage

Either nuget, or copy the cs files if single exe is necessary.

---

## Disclaimer

This is a development version of the toolkit.
The files are only versioned under the project or solution they are included in.

Please report any issues to the [toolkit developer](mailto:it@codedog.pl).

Woof Toolkit is a work in progress in constant development,
however it's carefully maintained with production code quality.