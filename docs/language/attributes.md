---
sidebar_position: 130
---

# Attributes

An attribute is metadata attached to a declaration, written in `[]` before it.
Every attribute is built in, so one the compiler does not know is an error.

```
[name]
[name("arg1", "arg2")]
```

## `ffi`

Names the library an external function is found in. The
[foreign function interface](./ffi.md) covers what can cross the boundary.

```mew
[ffi("mylib")]
pub static external fn bar(first: i8) -> void;
```

## `noreturn`

Says that a function never hands control back. Nothing after a call to one
runs, so a path that ends in such a call owes no `return` of its own.

```mew
pub fn pick(flag: bool) -> i32 {
    if flag {
        return 1;
    }

    panic("no value");
}
```

[`panic`](../stdlib/index.md#panic) carries the attribute, which is why `pick`
compiles. Without it the compiler reports that not all code paths return a
value, since nothing says the last statement is the end.

A function that never returns has nothing to return, so its return type has to
be `void`.

```mew
[noreturn]
pub fn stop() -> i32 {
    return 1;
}
```

```
Error [MEW2086]: Nothing to return
'stop' never returns, so it cannot also return 'i32'
```

:::note
The compiler takes the promise at its word. It does not check that the body
never reaches its end, because the thing that ends the program is usually
behind the [FFI](./ffi.md), where there is nothing to check. A function that
says it never returns and then does ends the program with an error naming it.
:::
