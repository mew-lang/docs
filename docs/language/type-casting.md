---
sidebar_position: 50
---

# Type Casting

`any` holds a value of any type. Assigning into one needs nothing, because
every type widens to `any` on its own.

```mew
let number: any = 32;
let text: any = "hello";
let numbers: any = new i32[] { 1, 2, 3 };
```

A [primitive](./primitives/index.md), a `string`, an [array](./arrays.md), a
[type](./types.md) you declare, a [union](./unions.md) and an
[interface](./interfaces.md) all go in.

Getting the value back out is a cast, written with `as`.

```mew
let number: any = 32;
let i = number as i32;
```

The cast is required in that direction, because `any` says nothing about what
is in there.

```mew
let boxed: any = new i32[] { 1, 2, 3 };
let numbers: i32[] = boxed;
```

```
Error [MEW2006]: Cannot convert type 'any' to 'i32[]'. An explicit conversion exists (are you missing a cast?)
```

## Casting a declared type

A cast reaches an [interface](./interfaces.md) the value's type implements as
well as the type itself.

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
let boxed: any = new Point { x: 32, y: 40 };

println($"{(boxed as Point).x}");
println($"{(boxed as Describable).describe()}");
```

```
32
(32, 40)
```

A [union](./unions.md) comes back out the same way, and `match` then reads it.

```mew
pub union Slot {
    empty,
    filled(i32),
}
```

```mew
// Usage:
let held: any = Slot::filled(41);

match held as Slot {
    .empty => {
        println("nothing");
    },
    .filled(value) => {
        println($"{value}");
    },
}
```

```
41
```

## A cast that is wrong fails when it runs

`as` is checked while the program runs, so a cast to the wrong type ends it.

```mew
pub type Circle {
    pub field radius: i32;
}
```

```mew
// Usage:
let boxed: any = new Point { x: 32, y: 40 };
let circle = boxed as Circle;
```

```
Unhandled error: Cannot convert a 'Point' to 'Circle'
```

[`is`](./type-checking.md) is what asks first.

```mew
// Usage:
let boxed: any = new Point { x: 32, y: 40 };

if boxed is Circle {
    println($"{(boxed as Circle).radius}");
} else {
    println("not a circle");
}
```

```
not a circle
```

## What does not go in

`void` is what a function returns when it returns nothing, so there is no value
to widen.

```mew
pub fn nothing() -> void { }

let x: any = nothing();
```

```
Error [MEW2007]: Cannot convert type 'void' to 'any'
```
