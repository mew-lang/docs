---
title: std.process
uid: stdlib.process
order: 9
---

## Functions

| Function                                                             | Description                                                                                                                                                         |
| :------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `git(arguments: ...string) -> Result<string, string>`                | Runs git and returns its output.                                                                                                                                    |
| `shell(program: string, arguments: ...string) -> Result<(), string>` | Runs the program with the arguments and prints its output. Fails if the program exits with anything but `0`. A program without a directory is looked for on `PATH`. |
