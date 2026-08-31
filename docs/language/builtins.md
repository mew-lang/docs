---
sidebar_position: 80
---

# Built-in Functions

A handful of functions are always in scope. They need no `#load` and no `use`,
and nothing declares them.

| Signature                        | Does                                      |
| :------------------------------- | :---------------------------------------- |
| `print(value: any) -> void`      | Writes the value, with no line break       |
| `println(value: any) -> void`    | Writes the value, followed by a line break |
| `itoa(value: i32) -> string`     | The text of an `i32`                       |
| `atoi(value: string) -> i32`     | The `i32` a string spells                  |

```mew
println("Hello, world!");

print("no newline here");
println("");

let text = itoa(42);
let number = atoi("42");
```

`print` and `println` take `any`, so they accept a value of any type. To control
how a value reads, build the text yourself with
[interpolation](./primitives/text.md#string-interpolation).

```mew
let name = "world";
let count = 3;

println($"{name} has {count}");
```

:::info
This list is expected to change. What belongs in the language itself rather than
in a library it ships with has not been settled, so treat these as the functions
that exist today rather than as a stable surface.
:::
