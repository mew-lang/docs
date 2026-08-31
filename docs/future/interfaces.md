---
sidebar_position: 90
---

# Interfaces

:::info
This functionality is not yet implemented
:::

An interface names a set of methods. A type implements one with an `impl`
block, and a method reaches the value it was called on through `self`.

```mew
pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}
```

```mew
// Usage:
let point = new Point { x: 32, y: 40 };
println(point.describe());
```

:::note
Interface members are public by definition, so `pub` is not written inside an
`interface` block. The implementing methods are declared like any other method.
:::
