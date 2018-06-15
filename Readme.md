# WOOF Toolkit

**.NET Framework** extensions created by **[CodeDog Ltd.](http://codedog.pl)**

Distributed under [MIT License](https://en.wikipedia.org/wiki/MIT_License) (see [License.txt](License.txt)).
(c)2018 by CodeDog Ltd., All rights reserved.

---

This toolkit is used in all CodeDog productions and contracted works.
It's free, Open-Source and can be used, re-used and developed by anyone.
Little code pearls contained in it are a part of what makes **CodeDog** special.
Made to never reinvent the wheel and to keep simple things as simple and zen as possible.

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

| Namespace               | Description                                          |
|:------------------------|:-----------------------------------------------------|
| AssemblyEx              | Tools for dealing with current and other assemblies. |
| ConsoleEx               | Advanced console tools.                              |
| Core                    | Tools for .NET Core server projects.                 |
| DeploymentEx            | Tools for building custom installers.                |
| ProcessEx               | Advanced inter-process communication tools.          |
| SystemEx                | Provides various information about computer system.  |

## Usage

### As project / DLL reference

Just reference the project Woof.

PROS:

- You can forget about references, everything's in one place.

CONS:

- Target project will require Woof.dll file as runtime dependency.

### As linked dependencies

PROS:

- Minimal build (target project will build only selected modules).
- No runtime dependencies (at least from Woof).
- If many projects in a solution use the same libraries, all point to the same files.

CONS:

- You have to configure references manually.
- The process of adding project links is tedious.

HOW:

- Create directory "Linked" in target project.
- Use "Add existing item..." option in VS (Shift+Alt+A).
- Select relevant files, don't forget about XAML and RES files.
- Use "Add as link" option.

### As copied dependencies

PROS:

- Easy to distribute the sources.
- Separate, immutable library version for the project.

CONS:

- Full manual configuration.
- When used more than onece - many versions of the same code - could be a mess.

HOW:

- Like with linked dependencies, but just use "Add" instead of "Add as link"

---

## Disclaimer

This is a development version of the toolkit.
The files are only versioned under the project or solution they are included in.

Please report any issues to the [toolkit developer](mailto:it@codedog.pl).

Woof Toolkit is a work in progress in constant development,
however it's carefully maintained with production code quality.