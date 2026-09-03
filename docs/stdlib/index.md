---
sidebar_position: 15
---

# Standard Library

The standard library is Mew, not compiler machinery. It ships beside the
compiler as `.mew` source, and every file in it is loaded into every
compilation, so nothing in it needs a `#load`.

Being loaded is not the same as being in scope without a prefix. Those are
decided separately: the shipped folder decides what is *loaded*, and each file's
[namespace](../language/namespaces.md) decides what it is *called*. A few
declarations are in the global namespace and are reachable by their own name;
everything else is namespaced under `std`, and is reached either by qualifying
it or by importing it with `use`.

```mew
use std.convert;

println(itoa(42));               // imported, so no prefix
println(std.convert.itoa(42));   // the same call, qualified
```

## Always in scope

These are in the global namespace, so they need neither a `#load` nor a `use`.

| Signature                        | Does                                     |
| :------------------------------- | :--------------------------------------- |
| `print(value: string) -> void`   | Writes the text, with no line break       |
| `println(value: string) -> void` | Writes the text, followed by a line break |

```mew
println("Hello, world!");

print("no newline here");
println("");
```

Both take text, so anything else is turned into text first.
[Interpolation](../language/primitives/text.md#string-interpolation) is how, and
it is the only way a value becomes text, so what a program prints reads the same
wherever it was written.

```mew
let name = "world";
let count = 3;

println($"{name} has {count}");
println($"{count}");
```

A value whose type is `any` cannot be printed. Interpolation has no text for it
either, so say what it is first.

```mew
let boxed: any = 3;

println($"{boxed as i32}");
```

`Enumerable<T>` and `Enumerator<T>` are also global, because `for` resolves them
by name. They are described under [loops](../language/control/loops.md).

## `std.convert`

Conversions between text and numbers.

| Signature                    | Does                       |
| :--------------------------- | :------------------------- |
| `itoa(value: i32) -> string` | The text of an `i32`       |
| `atoi(value: string) -> i32` | The `i32` a string spells  |

```mew
use std.convert;

let text = itoa(42);
let number = atoi("42");
```

`atoi` answers `0` for anything it cannot read, including the empty string and
text with a trailing remainder, so `atoi("12abc")` is `0` rather than `12`.

Printing a number needs neither of these. Interpolation already turns one into
text, and `itoa` is for when the text itself is the value you want.

## Native code

Some of the library cannot be written in Mew, and that part is a native library
called `mewstd` that ships with the compiler. `println` and `atoi` reach it
through the [foreign function interface](../language/ffi.md) exactly as your own
code would, with no privilege the language does not give you.

:::info
This surface is expected to change, and to grow. What belongs in the language,
in the library that ships with it, and in a package someone installs has not
been settled, so treat this as what exists today rather than as a stable
surface.
:::
