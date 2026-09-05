---
title: Type Checking
uid: language.type-checking
order: 12
---

`is` asks what a value is, and produces a `bool`.

```mew
use std;

let number: any = 32;
println($"{number is i32}");
println($"{number is string}");
```

```
true
false
```

It reads a [type](xref:language.types) you declare, a [union](xref:language.unions), and an
[interface](xref:language.interfaces) the value's type implements.

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
        return "a point";
    }
}

let boxed: any = new Point { x: 32, y: 40 };

println($"{boxed is Point}");
println($"{boxed is Describable}");
println($"{boxed is Circle}");
```

```
true
true
false
```

Asking about something the compiler already knows is a warning, since the
answer cannot be anything else.

```mew warning=MEW2041
let text = "hello";
let known = text is string;
```

```
Warning [MEW2041]: The given expression is always of the provided ('string') type
```

`is` is how a [cast](xref:language.type-casting) is made safe, because a cast that is
wrong ends the program.

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
        return "a point";
    }
}

let boxed: any = new Point { x: 32, y: 40 };

if boxed is Circle {
    println($"{(boxed as Circle).radius}");
}
```

> [!NOTE]
> A union case is not a type, so `is` cannot ask which case a value is.
> [`match`](xref:language.unions#reading-a-value) is what asks that.
