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

### Calling functions

```mew
let result = square(32);
```