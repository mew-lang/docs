---
title: Any
uid: language.primitives.any
order: 5
---

`any` holds a value of any type. Every type widens into one on its own, so
assigning into an `any` needs nothing written.

```mew
let number: any = 32;
let text: any = "hello";
let numbers: any = new i32[] { 1, 2, 3 };
let nothing: any = null;
```

The only thing that does not go in is `void`, because a function that returns
nothing produces no value to widen.

## Nothing is reachable through it

An `any` says nothing about what is in there, so no field, method or operator
reaches through one.

```mew error=MEW2029
pub type Point {
    pub field x: i32;
    pub field y: i32;
}

let boxed: any = new Point { x: 32, y: 40 };
let x = boxed.x;
```

```
Error [MEW2029]: Field does not exist
The field 'x' does not exist within type 'any'
```

Getting the value back out is a [cast](xref:language.type-casting), and
[`is`](xref:language.type-checking) is what asks what is in there first.

```mew
use std;

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

let boxed: any = new Point { x: 32, y: 40 };

if boxed is Point {
    println($"{(boxed as Point).x}");
}
```
