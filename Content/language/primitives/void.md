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

## Nowhere a value goes

`void` is a return type and nothing else. A function type spells the same idea as
`fn() -> void`, where the `-> void` may be left off.

A return type is the only place it may be written. Every other position describes
a value a program holds, and writing `void` in one is an error.

| Position | Written as |
| :------- | :--------- |
| A field | `pub field x: void;` |
| A parameter | `pub fn f(value: void)` |
| A parameter of a [function type](xref:language.functions#functions-as-values) | `fn(void) -> i32` |
| An [array's](xref:language.arrays) element | `new void[3]` |
| A [type argument](xref:language.generics#filling-them-in) | `Box<void>` |
| The right side of [`is`](xref:language.type-checking) | `value is void` |

```mew error=MEW2096
pub type Point {
    pub field x: void;
}
```

```mew error=MEW2096
pub fn shout(value: void) {
}
```

A type argument worked out at a call is the same rule. A
[lambda](xref:language.lambdas) whose body produces nothing is the usual way to
arrive at one by accident.

```mew error=MEW2096
use std;

pub fn each<T, U>(items: T[], apply: fn(T) -> U) -> U {
    return apply(items[0]);
}

let done = each(new i32[] { 1 }, |value| println($"{value}"));
```
