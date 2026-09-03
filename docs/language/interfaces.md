---
sidebar_position: 45
---

# Interfaces

An interface names a set of methods. A type implements one with an `impl`
block, and a method reaches the value it was called on through `self`. An
`impl` block that names no interface adds members instead, which
[Extending a type](./extending.md) covers.

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

A member the interface declares and the type does not supply is an error, and so
is a method in an `impl` block that the interface never declared.

### Using a value through its interface

A type that implements an interface can be used wherever that interface is
expected. This is what interfaces are for: writing one piece of code that works
for every type implementing it.

```mew
pub type Circle {
    pub field radius: i32;
}

impl Describable for Circle {
    pub fn describe() -> string {
        return $"a circle of {self.radius}";
    }
}

pub fn announce(item: Describable) -> void {
    println(item.describe());
}
```

```mew
// Usage:
announce(new Point { x: 1, y: 2 });
announce(new Circle { radius: 3 });
```

The same holds for a variable, a field, a return type and an array.

```mew
let shapes = new Describable[] {
    new Point { x: 1, y: 2 },
    new Circle { radius: 3 }
};

let mut index = 0;
while index < shapes.count {
    announce(shapes[index]);
    index += 1;
}
```

Only the methods the interface declares are reachable through it. To get back to
the type, check with `is` and convert with `as`.

```mew
let first: Describable = new Point { x: 32, y: 40 };

if first is Point {
    let point = first as Point;
    println($"{point.x}");
}
```

:::note
A type either implements an interface or it does not, and there is no
inheritance to change that later. Assigning a type that does not implement one
is an error, and a cast cannot rescue it.
:::

### Implementing more than one

A type can implement any number of interfaces, one `impl` block each. Naming the
same interface twice for the same type is an error.

```mew
pub interface Countable {
    fn size() -> i32;
}

pub type Bag {
    pub field items: i32[];
}

impl Countable for Bag {
    pub fn size() -> i32 {
        return self.items.count;
    }
}

impl Describable for Bag {
    pub fn describe() -> string {
        return $"a bag of {self.size()}";
    }
}
```
