---
title: Type Casting
uid: language.type-casting
order: 11
---

`any` holds a value of any type. Assigning into one needs nothing, because
every type widens to `any` on its own.

```mew
let number: any = 32;
let text: any = "hello";
let numbers: any = new i32[] { 1, 2, 3 };
```

A [primitive](xref:language.primitives), a `string`, an [array](xref:language.arrays), a
[type](xref:language.types) you declare, a [union](xref:language.unions) and an
[interface](xref:language.interfaces) all go in.

Getting the value back out is a cast, written with `as`.

```mew
let number: any = 32;
let i = number as i32;
```

The cast is required in that direction, because `any` says nothing about what
is in there.

```mew error=MEW2006
let boxed: any = new i32[] { 1, 2, 3 };
let numbers: i32[] = boxed;
```

```
Error [MEW2006]: Cannot convert type 'any' to 'i32[]'. An explicit conversion exists (are you missing a cast?)
```

## Casting a declared type

A cast reaches an [interface](xref:language.interfaces) the value's type implements as
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
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

pub type Circle {
    pub field radius: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

let boxed: any = new Point { x: 32, y: 40 };

println($"{(boxed as Point).x}");
println($"{(boxed as Describable).describe()}");
```

```
32
(32, 40)
```

A [union](xref:language.unions) comes back out the same way, and `match` then reads it.

```mew
pub union Slot {
    empty,
    filled(i32),
}
```

```mew
use std;

pub union Slot {
    empty,
    filled(i32),
}

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
pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

pub type Circle {
    pub field radius: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

let boxed: any = new Point { x: 32, y: 40 };
let circle = boxed as Circle;
```

```
Unhandled error: Cannot convert a 'Point' to 'Circle'
```

[`is`](xref:language.type-checking) is what asks first.

```mew
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

pub type Circle {
    pub field radius: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

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

```mew error=MEW2007
pub fn nothing() -> void { }

let x: any = nothing();
```

```
Error [MEW2007]: Cannot convert type 'void' to 'any'
```
