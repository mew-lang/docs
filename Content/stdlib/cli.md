---
title: std.cli
uid: stdlib.cli
order: 5
---

## Types

| Type          | Description                                                                                                  |
| :------------ | :----------------------------------------------------------------------------------------------------------- |
| [`Cli`](#cli) | Reads options from command line arguments. An option is named by its spellings, separated by a vertical bar. |

## `Cli`

| Member                                    | Description                                         |
| :---------------------------------------- | :-------------------------------------------------- |
| `Cli::new(arguments: string[]) -> Cli`    | Creates a `Cli` for the arguments.                  |
| `single(names: string) -> Option<string>` | Returns the last value given for the option.        |
| `many(names: string) -> string[]`         | Returns every value given for the option, in order. |
| `flag(names: string) -> bool`             | Whether the option was given.                       |
