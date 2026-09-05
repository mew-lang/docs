---
title: "Void"
uid: language.primitives.void
order: 7
---

`void` is what a function that produces no value returns. It is not a value, and
it is the one type that does not go into an [`any`](xref:language.primitives.any).

A function declared with no `->` returns `void`, and one that says so explicitly
means the same thing.

```mew
use std;

pub fn shout() {
    println("hey");
}

pub fn also_shout() -> void {
    println("hey again");
}

shout();
also_shout();
```

## There is nothing to name

Because `void` is not a value, a local cannot hold one. Calling such a function in
a `let` is an error, and it is reported against the `let` rather than the call.

```mew error=MEW2013
pub fn nothing() -> void { }

let value = nothing();
```

Writing the annotation out makes no difference, because the problem is the missing
value rather than how it is spelled.

```mew error=MEW2013
pub fn nothing() -> void { }

let value: void = nothing();
```

There is no text representation either, so a `void` call cannot go in an
[interpolation hole](xref:language.primitives.text#string-interpolation).

## Returning from one

A `void` function needs no `return` at all. Reaching the end is how it finishes.

```mew
use std;

pub fn greet(name: string) -> void {
    println($"Hello, {name}!");
}

greet("world");
```

A bare `return` leaves early.

```mew
use std;

pub fn greet(name: string) -> void {
    if name == "" {
        return;
    }

    println($"Hello, {name}!");
}

greet("world");
greet("");
```

Returning a value from one is an error, since there is nothing for the caller to
receive.

```mew error=MEW2017
pub fn greet() -> void {
    return 1;
}
```

> [!NOTE]
> `void` is a return type and nothing else. It is not written on a field, a
> parameter or a type argument, and a function type spells the same idea as
> `fn() -> void`, where the `-> void` may be left off.
