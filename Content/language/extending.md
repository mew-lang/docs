---
title: Extending a type
uid: language.extending
order: 10
---

An `impl` block without an interface adds members to a type. The type does not
have to be one you declared, and it does not have to be one you can declare:
the same block works on your own types, on types from another file, on generic
types, and on the primitives the language defines itself.

```mew
use std;

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

println(new Point { x: 1, y: 2 }.describe());
```

```
(1, 2)
```

A member added this way is the same as one written inside the type. It reaches
the value it was called on through `self`, it takes part in overload
resolution, and `pub` decides whether other files can see it.

## Extending a primitive

The primitives take members the same way, which is how a standard library
gives `string` and the numbers anything to do.

```mew
use std;

impl i32 {
    pub fn doubled() -> i32 {
        return self * 2;
    }
}

println($"{21.doubled()}");
```

```
42
```

## Static members

A `static` member belongs to the type rather than to a value, and is called
through the type name. It has no `self`, so anything it works on arrives as an
argument.

```mew
use std;

impl string {
    pub static fn twice(value: string) -> string {
        return value + value;
    }

    pub fn shout() -> string {
        return self + "!";
    }
}

println(string::twice("ab"));
println("hello".shout());
```

```
abab
hello!
```

## Extending a generic type

The block names the type parameters the declaration did, and the members can
use them.

```mew
use std;

pub type Box<T> {
    pub field value: T;
}

impl Box<T> {
    pub fn get() -> T {
        return self.value;
    }
}

println($"{new Box<i32> { value: 7 }.get()}");
```

```
7
```

## What cannot be extended

The target has to be a named type. An array is written as its element type
followed by `[]` rather than named, so `impl i32[]` is an error.

## Adding members and implementing an interface

The two forms of `impl` do different jobs. Without an interface it adds
members. With one it says the type implements that interface, and the block
holds the methods the interface requires:

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
        return "a point";
    }
}
```

A type can have as many of each as it needs. See [Interfaces](xref:language.interfaces)
for the second form.
