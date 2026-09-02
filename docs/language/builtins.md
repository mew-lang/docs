---
sidebar_position: 80
---

# Built-in Functions

A handful of functions are always in scope. They need no `#load` and no `use`,
and nothing declares them.

| Signature                        | Does                                      |
| :------------------------------- | :---------------------------------------- |
| `print(value: string) -> void`   | Writes the text, with no line break        |
| `println(value: string) -> void` | Writes the text, followed by a line break  |
| `itoa(value: i32) -> string`     | The text of an `i32`                       |
| `atoi(value: string) -> i32`     | The `i32` a string spells                  |

```mew
println("Hello, world!");

print("no newline here");
println("");

let text = itoa(42);
let number = atoi("42");
```

`print` and `println` take text, so anything else is turned into text first.
[Interpolation](./primitives/text.md#string-interpolation) is how, and it is the
only way a value becomes text, so what a program prints reads the same wherever
it was written.

```mew
let name = "world";
let count = 3;

println($"{name} has {count}");
println($"{count}");
println(itoa(count));
```

A value whose type is `any` cannot be printed. Interpolation has no text for it
either, so say what it is first.

```mew
let boxed: any = 3;

println($"{boxed as i32}");
```

:::info
This list is expected to change. What belongs in the language itself rather than
in a library it ships with has not been settled, so treat these as the functions
that exist today rather than as a stable surface.
:::
