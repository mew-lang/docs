---
title: Attributes
uid: language.attributes
order: 22
---

An attribute is metadata attached to a declaration, written in `[]` before it.
Every attribute is built in, so one the compiler does not know is an error.

```
[name]
[name("arg1", "arg2")]
```

## `ffi`

Names the library an external function is found in. The
[foreign function interface](xref:language.ffi) covers what can cross the boundary.

```mew
[ffi("mylib")]
pub static external fn bar(first: i8) -> void;
```

## `host`

Names the method on the platform an external function stands for. The
declaration beside the attribute is the signature, and the compiler holds it
against what the platform declares, reporting MEW2111 when the two differ.

```mew
[host("global::System.Console.WriteLine")]
pub static external fn write(text: string) -> void;
```

## `struct`

Says a type is copied rather than shared. Assigning one, passing it to a
function or storing it in an array copies the value, so a write through one
name is not seen through another.

```mew
[struct]
pub type Counter {
    pub mut field n: i32;
}

let counter = new Counter { n: 1 };
let mut copied = counter;
copied.n = 2;
```

`counter.n` is still `1`.

## `noreturn`

Says that a function never hands control back. Nothing after a call to one
runs, so a path that ends in such a call owes no `return` of its own.

```mew
use std;

pub fn pick(flag: bool) -> i32 {
    if flag {
        return 1;
    }

    panic("no value");
}
```

[`panic`](xref:stdlib.std#functions) carries the attribute, which is why `pick`
compiles. Without it the compiler reports that not all code paths return a
value, since nothing says the last statement is the end.

A function that never returns has nothing to return, so its return type has to
be `void`.

```mew error=MEW2086
[noreturn]
pub fn stop() -> i32 {
    return 1;
}
```

```
Error [MEW2086]: Nothing to return
'stop' never returns, so it cannot also return 'i32'
```

> [!NOTE]
> The compiler takes the promise at its word. It does not check that the body
> never reaches its end, because the thing that ends the program is usually
> behind the [FFI](xref:language.ffi), where there is nothing to check. What
> happens when a function says it never returns and then does is
> [undefined](xref:language.undefined#a-noreturn-function-that-returns).
