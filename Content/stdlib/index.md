---
title: Standard Library
uid: stdlib
order: 1
---

The Mew code that comes with the compiler, compiled once into `Mew.Std.dll`.

| Namespace                                     | Description                                                     |
| :-------------------------------------------- | :-------------------------------------------------------------- |
| [`std`](xref:stdlib.std)                       | Printing, stopping, `Option`, `Result`, sequences and strings    |
| [`std.build`](xref:stdlib.build)               | What the tasks of a build call                                   |
| [`std.build.dotnet`](xref:stdlib.build.dotnet) | Running the .NET SDK from a build                                |
| [`std.build.engine`](xref:stdlib.build.engine) | What a build file is compiled into                               |
| [`std.cli`](xref:stdlib.cli)                   | Reading options from a program's arguments                       |
| [`std.collections`](xref:stdlib.collections)   | Collections that grow                                            |
| [`std.convert`](xref:stdlib.convert)           | Turning text into numbers and back                               |
| [`std.io`](xref:stdlib.io)                     | Files, directories, and the patterns that find them              |
| [`std.process`](xref:stdlib.process)           | Running other programs                                           |

- `std` is imported into every file. The others are imported with `use`, or
  written in front of the name, as `std.convert.itoa(42)`.
- Your own functions come first, so one named `println` hides the library's,
  which is still there as `std.println`.
- You can [add members](xref:language.extending) to the library's types, but not
  make them implement an interface.
- The library is ordinary Mew, and reaches the
  [platform](xref:language.platform) the way your own code does.

> [!IMPORTANT]
> This surface is expected to change, and to grow. What belongs in the language,
> in the library that ships with it, and in a package someone installs has not
> been settled, so treat this as what exists today rather than as a stable
> surface.
