---
sidebar_position: 70
---

# Functions

### Defining functions

```mew
pub static fn square(value: i32) -> i32 {
    return value * value;
}
```

### Visibility

`pub` makes a function visible to other files. Without it the function belongs to
the file that declares it, and that is narrower than its namespace: two files
sharing a `namespace` still cannot call each other's private functions.

```mew
pub fn shared() -> i32 {
    return 1;
}

fn local() -> i32 {
    return 2;
}
```

:::note
This applies to free functions. A function declared inside a `type` is scoped to
that type rather than to the file.
:::

### Returning

Every path through a function that declares a return type has to return a value.
A path that falls off the end is an error.

```mew
pub fn sign(value: i32) -> i32 {
    if value > 0 {
        return 1;
    }

    return -1;
}
```

A function that returns `void` needs no `return` at all. Anything written after a
`return` is reported as unreachable.

### Calling functions

```mew
let result = square(32);
```

### Overloading

Several functions can share a name as long as their parameters differ, either in
type or in how many there are. The one whose parameters fit the arguments is the
one that is called.

```mew
pub fn describe(value: i32) -> string {
    return itoa(value);
}

pub fn describe(value: string) -> string {
    return value;
}
```

```mew
// Usage:
let a = describe(32);
let b = describe("Patrik");
```

The return type is not part of what makes two functions different, so declaring
the same parameters twice is an error however the results differ.

:::note
A function can only be declared at the top level of a file. There are no
functions inside other functions.
:::
