---
sidebar_position: 15
---

# Standard Library

The standard library is Mew, not compiler machinery. It ships beside the
compiler as `.mew` source, and every file in it is loaded into every
compilation, so nothing in it needs a `#load`.

Being loaded is not the same as being in scope without a prefix. Those are
decided separately: the shipped folder decides what is *loaded*, and each file's
[namespace](../language/namespaces.md) decides what it is *called*. Most of the
library is namespaced, and is reached either by qualifying it or by importing it
with `use`.

```mew
use std;

println(std.convert.itoa(42));   // qualified
```

```mew
use std;
use std.convert;

println(itoa(42));               // imported, so no prefix
```

## Always in scope

Four declarations are in the global namespace, so they need neither a `#load`
nor a `use`.

| Name                | Why it is global                                |
| :------------------ | :---------------------------------------------- |
| `Option<T>`         | A value that may be absent, described [below](#optiont-and-resultt-e) |
| `Result<T, E>`      | An operation that may fail, described [below](#optiont-and-resultt-e) |
| `Enumerable<T>`     | `for` resolves it by name                        |
| `Enumerator<T>`     | `for` resolves it by name                        |

`Enumerable<T>` and `Enumerator<T>` are described under
[loops](../language/control/loops.md).

## `std`

Writing text and stopping the program. Everything here needs `use std;` or the
`std.` prefix.

| Signature                        | Does                                     |
| :------------------------------- | :--------------------------------------- |
| `print(value: string) -> void`   | Writes the text, with no line break       |
| `println(value: string) -> void` | Writes the text, followed by a line break |
| `panic(reason: string) -> void`  | Writes the reason and ends the program with exit code 1 |
| `exit(code: i32) -> void`        | Ends the program with the given code      |

```mew
use std;

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

### Stopping early

`panic` is for the case where carrying on would be worse than stopping.

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

`exit` stops the program the same way without writing anything, and takes the
code to stop with.

```mew
if count < 0 {
    exit(2);
}
```

Both carry the [`noreturn`](../language/attributes.md#noreturn) attribute, so a
path that ends in either owes no `return`. That is what makes `unwrap` below
possible.

:::note
These two are the only way a Mew program stops early. There are no exceptions,
so nothing catches a panic and nothing runs after it.
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
| `unwrap() -> T`            | The value, or a [panic](#stopping-early)                   |
| `unwrap_or(fallback: T)`   | The value, or `fallback`                          |
| `or(other: Option<T>)`     | This option if it has a value, otherwise `other`  |
| `map<U>(f: fn(T) -> U)`    | An `Option<U>`, with `f` run over the value       |
| `and_then<U>(f)`           | What `f` answers, for chaining one option onto another |
| `filter(f: fn(T) -> bool)` | This option if `f` answers true, otherwise `none` |
| `ok_or<E>(error: E)`       | A `Result<T, E>`, using `error` for `none`        |

| On `Result<T, E>`          | Gives                                            |
| :------------------------- | :----------------------------------------------- |
| `is_ok() -> bool`          | Whether it succeeded                              |
| `is_err() -> bool`         | Whether it failed                                 |
| `unwrap() -> T`            | The value, or a [panic](#stopping-early)                   |
| `unwrap_or(fallback: T)`   | The value, or `fallback`                          |
| `map<U>(f: fn(T) -> U)`    | A `Result<U, E>`, with `f` run over the value     |
| `map_err<F>(f: fn(E) -> F)`| A `Result<T, F>`, with `f` run over the error     |
| `and_then<U>(f)`           | What `f` answers, for chaining one result onto another |
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

`unwrap` reads the value and [panics](#stopping-early) when there is none, so reach for
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

### Working on the value without unwrapping it

`map`, `and_then` and `filter` take a [lambda](../language/lambdas.md) and leave
the absent or failed case alone, so a chain of them says what to do with a value
without asking whether there is one at every step.

```mew
// Usage:
println($"{divide(6, 2).map(|n| n * 10).unwrap_or(0)}");
println($"{divide(6, 0).map(|n| n * 10).unwrap_or(-1)}");
println(divide(6, 0).map_err(|reason| $"failed: {reason}").err().unwrap_or(""));
println($"{divide(12, 2).and_then(|n| divide(n, 3)).unwrap_or(0)}");
```

```
30
-1
failed: divide by zero
2
```

`map` changes what is held, `and_then` chains one of these onto another and
flattens the result, and `filter` on an `Option<T>` drops a value that does not
answer the question.

```mew
// Usage:
let held: Option<i32> = .some(20);

println($"{held.filter(|n| n > 10).unwrap_or(0)}");
println($"{held.filter(|n| n > 100).is_none()}");
```

```
20
true
```

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
