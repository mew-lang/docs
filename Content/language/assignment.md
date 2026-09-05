---
title: Assignment
uid: language.assignment
order: 4
---

`let` gives a value a name.

```mew
use std;

let count = 1;

println($"{count}");
```

The value is not optional. There is no way to declare a local and fill it in
later, so a name always has something behind it from the line that introduces it.

## The type

Without an annotation, a local takes the type of its initializer. A whole number
literal with nothing to go on is an `i32`, and one with a fraction is an `f32`.

```mew
use std;

let count = 1;      // i32
let ratio = 1.5;    // f32
let name = "Ada";   // string

println($"{count} {ratio} {name}");
```

Write the type down when a different one is wanted, and the initializer is
converted to it.

```mew
use std;

let narrow: i16 = 1;
let wide: i64 = narrow;

println($"{wide}");
```

Some initializers say nothing about what the local should hold. `null` is one, so
a `let` with only `null` to go on needs the type written.

```mew error=MEW2014
let text = null;
```

```mew
use std;

let text: string = null;
let filled: string = "Ada";

println(filled);
```

> [!NOTE]
> There is no way to ask whether a value is `null`. `text == null` is not a
> defined comparison, and `is` only answers about the type. That is one of the
> reasons the standard library answers an absent value with
> [`Option<T>`](xref:stdlib#optiont-and-resultt-e) rather than with `null`.

A function that answers nothing is another. There is no value to name, so the
`let` itself is the error.

```mew error=MEW2013
pub fn nothing() -> void { }

let value = nothing();
```

## Mutability

A local is immutable. Assigning to one is an error unless it was declared `mut`.

```mew error=MEW2025
let count = 1;
count = 2;
```

```mew
use std;

let mut count = 1;
count = 2;

println($"{count}");
```

`mut` is about the name, not about what the name holds. A local without it that
holds an [array](xref:language.arrays) can still have its elements written,
because that writes into the array rather than to the name.

```mew
use std;

let values = new i32[3];
values[0] = 7;

println($"{values[0]}");
```

[Fields](xref:language.types#mutability) work the same way, and need their own
`mut` to be assignable.

Declaring a name that is already in scope is an error. To give something a new
value, assign to it; to give it a new name, pick a different one.

```mew error=MEW2012
let value = 1;
let value = 2;
```

An inner block is a different scope, so a name there may
[shadow](xref:language.names#shadowing) one from outside it.

## Assignment is an expression

Assignment produces the value it assigned, so it can be read where a value is
wanted, and it groups right to left. `a = b = 3` gives both `a` and `b` the
value `3`.

```mew
use std;

let mut a = 1;
let read = (a = 3);

println($"{a} {read}");
```

The grouping is the part to remember. Reading from an assignment is rarely what
you want in code someone else has to follow.

## Compound assignment

`+=`, `-=`, `*=`, `/=` and `%=` apply the operator to the target and the right
side, then assign. `total += n` means `total = total + n`, and converts the same
way.

```mew
use std;

let mut total = 0;

for value in new i32[] { 1, 2, 3, 4 } {
    total += value;
}

println($"{total}");
```

Because `+` also joins text, `+=` builds up a string.

```mew
use std;

let mut line = "";

for letter in "abc".chars() {
    line += letter;
}

println(line);
```

## What can be assigned to

The target of an assignment is a name, an [index](xref:language.arrays#reading-and-writing-elements),
or a [member](xref:language.types#fields). Anything else is not something a value
can be put into, and the parser says so.

```mew
use std;

pub type Counter {
    pub mut field total: i32;
}

let mut name = "a";
let values = new i32[2];
let counter = new Counter { total: 0 };

name = "b";
values[0] = 1;
counter.total = 2;

println($"{name} {values[0]} {counter.total}");
```

`count` on an array is not one of them. It reports how long the array is, and an
array's length is fixed when it is created.
