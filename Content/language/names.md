---
title: Names and Scopes
uid: language.names
order: 17
---

A name written on its own has to be resolved to exactly one thing. This page is
about how that is decided, and about how long a name lasts.

## Scopes

A local is in scope from the statement that declares it to the end of the block
that holds it.

```mew
use std;

let outer = 1;

if outer == 1 {
    let inner = 2;
    println($"{outer + inner}");
}
```

`inner` is gone once the block ends. A local is not in scope on its own line
either, so an initializer cannot read the name it is initializing.

```mew error=MEW2009
let value = value + 1;
```

Declaring the same name twice in one scope is an error.

```mew error=MEW2012
let value = 1;
let value = 2;
```

The bindings a [`for`](xref:language.control.loops#for) loop and a
[`match`](xref:language.unions#reading-a-value) arm introduce follow the same
rules: they belong to that loop or that arm, and nothing outside it can read them.

## Shadowing

An inner scope may reuse a name from an outer one, and does so for as long as it
lasts. The outer name is untouched and comes back afterwards.

```mew
use std;

let value = 100;

for value in new i32[] { 1, 2, 3 } {
    println($"{value}");
}

println($"{value}");
```

That prints `1`, `2`, `3`, and then `100`.

## Resolution order

A bare name is looked up in this order, and the first thing found is what it
means.

| | Looked in |
| :-- | :-------- |
| 1 | Locals in scope, innermost first |
| 2 | The enclosing function's parameters |
| 3 | The enclosing type's members, when there is one |
| 4 | Declarations in the file |
| 5 | Visible declarations in the file's own [namespace](xref:language.namespaces), from any file |
| 6 | Visible declarations in each `use`d namespace |
| 7 | Visible declarations in the global namespace |

A name that resolves to nothing is an error, and the compiler says which kind it
was looking for: a variable it could not find is reported differently from a type
or a function.

A name that two visible declarations both answer for is ambiguous, and writing the
[namespace out](xref:language.namespaces#reaching-a-namespace-without-importing-it)
is what says which was meant.

## A member is not a name

The name after a `.` is looked up on the target and nowhere else. It never falls
back to the enclosing scope, so a local can never capture a field.

```mew
use std;

pub type Point {
    pub field x: i32;
}

let x = 100;
let point = new Point { x: 1 };

println($"{point.x}");
println($"{x}");
```

`point.x` is the field and prints `1`. The local `x` is a different thing
entirely, and the two never collide.

## Declarations do not need to come first

Declarations are found before any body is bound, so the order they are written in
does not matter. A function may call one written below it.

```mew
use std;

println($"{doubled(21)}");

pub fn doubled(value: i32) -> i32 {
    return value * 2;
}
```

Statements are the exception. They run in order, so a local has to be declared
before it is read.

## Primitive names are not keywords

`string`, `i32`, `bool` and the rest of the [primitives](xref:language.primitives)
are names the language declares rather than reserved words. A local, a field or a
function may take one, though there is rarely a reason to.

```mew
use std;

pub type Reading {
    pub field int: i32;
}

let string = 5;

println($"{string}");
println($"{new Reading { int: 7 }.int}");
```

A type declaration is the exception: the name is already a type, so declaring
another one with it collides.

```mew error=MEW2001
pub type string {
    pub field length: i32;
}
```

`int` and `float` are aliases, so taking one of those collides with the type it
spells: `pub type int` reports a duplicate `i32`.

The [keywords](https://github.com/mew-lang/mew/blob/main/spec/02-lexical-structure.md)
proper cannot be used as names at all. `new` is the one that bends: it is a
keyword at the start of an expression, where it creates a value, which is what
lets a type give itself a [static method called `new`](xref:language.types#constructors).
