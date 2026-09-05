---
title: Functions
uid: language.functions
order: 15
---

A function declared outside a type is a free function.

```mew
use std;

pub fn squared(value: i32) -> i32 {
    return value * value;
}

println($"{squared(8)}");
```

A function is declared at the top level of a file. There are no functions inside
other functions. A [lambda](xref:language.lambdas) is what a function written
inside another one looks like.

Declarations are found before any body is bound, so a function may call one
written below it.

## Visibility

`pub` makes a function visible to other files. Without it the function belongs to
the file that declares it, which is narrower than its
[namespace](xref:language.namespaces#visibility).

```mew
use std;

pub fn shared() -> i32 {
    return 1;
}

fn local() -> i32 {
    return 2;
}

println($"{shared() + local()}");
```

That applies to free functions. A function declared inside a `type` is a
[method](xref:language.types#methods), and its visibility is scoped to the type
rather than to the file.

## Parameters and results

A parameter is written `name: type`, and parameters are separated by commas. A
missing `->` means the function returns [`void`](xref:language.primitives.void).

```mew
use std;

pub fn between(value: i32, low: i32, high: i32) -> bool {
    return value >= low && value <= high;
}

pub fn announce(text: string) {
    println(text);
}

announce($"{between(5, 1, 10)}");
```

A parameter is immutable. There is no `mut` on one, so a function cannot reassign
what it was handed. It can only produce a new value.

There are no default values, no way to name an argument at the call, and no
variable argument list. A call passes exactly as many arguments as the declaration
takes, in order.

## Every path has to return

A function that declares a return type has to produce one on every path through
it. A path that falls off the end is an error, reported after the program is bound
from a control flow graph built for the function.

```mew error=MEW3000
pub fn sign(value: i32) -> i32 {
    if value > 0 {
        return 1;
    }
}
```

Give the last path an answer.

```mew
use std;

pub fn sign(value: i32) -> i32 {
    if value > 0 {
        return 1;
    }

    return -1;
}

println($"{sign(3)}");
```

A `void` function needs no `return` at all, and a `loop` with no `break` never
falls out of its block, so a function whose body is one owes nothing after it.

A path that ends in a call to a function marked
[`[noreturn]`](xref:language.attributes#noreturn) also owes no `return`, because
nothing after such a call runs. That is what lets
[`panic`](xref:stdlib#stopping-early) stand in for one.

```mew
use std;

pub fn pick(flag: bool) -> i32 {
    if flag {
        return 1;
    }

    panic("no value");
}

println($"{pick(true)}");
```

A statement no path can reach is a warning rather than an error, so the compiler
points at code that will never run without refusing to build.

```mew warning=MEW3001
use std;

pub fn first() -> i32 {
    return 1;
    println("never");
    return 2;
}

println($"{first()}");
```

## Overloading

Several functions may share a name as long as their parameters differ, in type or
in number. The one whose parameters fit the arguments is the one called.

```mew
use std;

pub fn describe(value: i32) -> string {
    return $"{value}";
}

pub fn describe(value: string) -> string {
    return value;
}

println(describe(32));
println(describe("Ada"));
```

The return type is not part of what tells two functions apart, so declaring the
same parameters twice is an error however the results differ.

```mew error=MEW2067
pub fn read() -> i32 {
    return 1;
}

pub fn read() -> string {
    return "one";
}
```

Overloading covers methods and members added by an
[`impl` block](xref:language.extending) the same way. Where an argument is a
[union case written without its union](xref:language.unions#leaving-the-union-out),
the case name is what picks between candidates.

## Functions as values

A function's name, written without a call, is a value of its function type, which
is what lets one function be handed to another.

```mew
use std;

pub fn twice(n: i32) -> i32 {
    return n * 2;
}

pub fn apply(f: fn(i32) -> i32, value: i32) -> i32 {
    return f(value);
}

println($"{apply(twice, 21)}");
```

A name that means more than one function has no single type, so it cannot be held
on its own, because nothing at that point says which one was meant.

[Lambdas](xref:language.lambdas) cover function types, writing a function inline,
and what a lambda captures.

## What functions do not have

There is no partial application and no way to compose two functions into a third.
A lambda that closes over what it needs covers the same ground.
