---
title: std.build.dotnet
uid: stdlib.build.dotnet
order: 3
---

## Functions

| Function                                                           | Description                                                                    |
| :----------------------------------------------------------------- | :----------------------------------------------------------------------------- |
| `dotnet(arguments: ...string) -> Result<(), string>`               | Runs `dotnet` with the arguments.                                              |
| `product_version(assembly: std.io.File) -> Result<string, string>` | Reads the informational version of the assembly, without anything after a `+`. |
