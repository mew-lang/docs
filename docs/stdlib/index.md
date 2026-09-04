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

## `panic`

Ends the program, for the case where carrying on would be worse than stopping.

| Signature                       | Does                                        |
| :------------------------------ | :------------------------------------------ |
| `panic(reason: string) -> void` | Writes the reason and ends the program with exit code 1 |

```mew
if count < 0 {
    panic("a count cannot be negative");
}
```

```
Unhandled error: a count cannot be negative
```

The reason goes to standard error, where the runtime's own failures go, so it
does not land in output a caller is reading.

`panic` carries the [`noreturn`](../language/attributes.md#noreturn) attribute,
so a path that ends in one owes no `return`. That is what makes `unwrap` below
possible.

:::note
This is the only way a Mew program stops early. There are no exceptions, so
nothing catches a panic and nothing runs after it.
:::

## `Option<T>` and `Result<T, E>`

Two global [unions](../language/unions.md), for a value that may be absent and
an operation that may fail. They exist so that neither has to be answered with
`null`.

```mew
pub union Option<T> {
    none,
    some(T),
}

pub union Result<T, E> {
    ok(T),
    err(E),
}
```

A union is never `null` and a `match` has to handle every case, so a caller
cannot read a value that is not there by forgetting to check.

```mew
pub fn head(values: i32[]) -> Option<i32> {
    if values.count == 0 {
        return Option<i32>::none;
    }

    return Option<i32>::some(values[0]);
}
```

```mew
// Usage:
match head(new i32[] { 3, 4 }) {
    .some(value) => {
        println($"{value}");
    },
    .none => {
        println("nothing there");
    },
}
```

```
3
```

### Reading one without a match

Both carry methods for the cases where a full `match` is more than the question
needs.

| On `Option<T>`             | Gives                                            |
| :------------------------- | :----------------------------------------------- |
| `is_some() -> bool`        | Whether there is a value                          |
| `is_none() -> bool`        | Whether there is not                              |
| `unwrap() -> T`            | The value, or a [panic](#panic)                   |
| `unwrap_or(fallback: T)`   | The value, or `fallback`                          |
| `or(other: Option<T>)`     | This option if it has a value, otherwise `other`  |
| `ok_or<E>(error: E)`       | A `Result<T, E>`, using `error` for `none`        |

| On `Result<T, E>`          | Gives                                            |
| :------------------------- | :----------------------------------------------- |
| `is_ok() -> bool`          | Whether it succeeded                              |
| `is_err() -> bool`         | Whether it failed                                 |
| `unwrap() -> T`            | The value, or a [panic](#panic)                   |
| `unwrap_or(fallback: T)`   | The value, or `fallback`                          |
| `ok() -> Option<T>`        | The value as an option                            |
| `err() -> Option<E>`       | The error as an option                            |

```mew
pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return Result<i32, string>::err("divide by zero");
    }

    return Result<i32, string>::ok(left / right);
}
```

```mew
// Usage:
println($"{divide(6, 2).unwrap_or(0)}");
println($"{divide(6, 0).unwrap_or(0)}");
println($"{divide(6, 0).err().unwrap_or("")}");
println($"{head(new i32[0]).ok_or<string>("empty").is_err()}");
```

```
3
0
divide by zero
true
```

`unwrap` reads the value and [panics](#panic) when there is none, so reach for
it only where the absent case is a bug rather than something to handle.

```mew
// Usage:
println($"{divide(6, 2).unwrap()}");
println($"{divide(6, 0).unwrap()}");
```

```
3
Unhandled error: unwrapped a result that failed
```

:::note
The message names neither the value nor the error, because `T` and `E` can be
any type and not every type has a text representation. Use `err()` and print
that yourself when the reason matters.
:::

Nothing here takes a function, so there is no `map`, `and_then` or `filter`.
A function is not a value in Mew yet, so those cannot be written.

## `std.convert`

Conversions between text and numbers.

| Signature                                        | Does                          |
| :----------------------------------------------- | :---------------------------- |
| `itoa(value: i32) -> string`                     | The text of an `i32`          |
| `atoi(value: string) -> Result<i32, ParseError>` | The `i32` a string spells, or why it does not |

```mew
use std.convert;

let text = itoa(42);
let number = atoi("42").unwrap_or(0);
```

Not every string spells a number, so `atoi` answers with a
[`Result`](#optiont-and-resultt-e) rather than a number. `ParseError` says
which way it failed.

```mew
pub union ParseError {
    invalid,
    overflow,
}
```

```mew
// Usage:
match atoi(text) {
    .ok(value) => {
        println($"{value}");
    },
    .err(reason) => {
        println(reason.describe());
    },
}
```

| Given                  | Answers                                     |
| :--------------------- | :------------------------------------------ |
| `"42"`                 | `ok(42)`                                     |
| `""`                   | `err(invalid)`, "not a number"               |
| `"abc"`                | `err(invalid)`, "not a number"               |
| `"12abc"`              | `err(invalid)`, "not a number"               |
| `"2147483648"`         | `err(overflow)`, "outside the range of an i32" |

`describe()` is the text of a `ParseError`, for when the reason is going
straight to a reader.

`itoa` cannot fail, so it hands back a `string` rather than a `Result`. It
writes into a buffer of its own choosing, and twelve bytes is always enough for
an `i32`, the widest being the eleven characters of `-2147483648`.

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
