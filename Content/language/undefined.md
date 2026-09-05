---
title: Undefined Behaviour
uid: language.undefined
order: 22
---

Almost everything in Mew is either defined or reported as a diagnostic. Two things
are neither: the language declines to say what they mean, so a program that relies
on one is not a program whose behaviour anything guarantees.

Both are reachable only by writing something that already looks wrong, and today's
compiler stops the program with a message rather than carrying on. That is a
courtesy of this implementation rather than a promise of the language, so do not
build on it.

## Reading an array element that was never written

An [array](xref:language.arrays) created with a size and no initializer starts
every element at the zero of its element type, and for anything that can hold
`null`, that zero is `null`.

A [union](xref:language.unions) cannot be `null`. It is always exactly one of its
cases, which is the whole reason a `match` on one is safe. An array of unions
created with a size therefore holds, in every element, a value the type system says
cannot exist.

```mew
use std;

pub union Slot {
    empty,
    filled(i32),
}

let slots = new Slot[2];

match slots[0] {
    .empty => {
        println("empty");
    },
    .filled(value) => {
        println($"{value}");
    },
}
```

That program compiles, and the `match` has an arm for every case, but neither arm
describes what is actually in `slots[0]`. Today it stops with
`Unhandled error: A value was null`.

Give the array an initializer, or fill it before reading it.

```mew
use std;

pub union Slot {
    empty,
    filled(i32),
}

let slots = new Slot[] { .empty, .filled(3) };

match slots[1] {
    .empty => {
        println("empty");
    },
    .filled(value) => {
        println($"{value}");
    },
}
```

## A `noreturn` function that returns

[`[noreturn]`](xref:language.attributes#noreturn) tells the compiler that nothing
after a call to the function runs, which is what lets a path ending in one owe no
`return` of its own.

The compiler takes that at its word and does not check the body, because what ends
a program usually sits behind the [FFI](xref:language.ffi) where there is nothing
to check. A function that makes the promise and then breaks it leaves the caller
having skipped a `return` it turned out to need.

```mew ignore
[noreturn]
pub fn liar() -> void {
    // Says it never comes back, and then comes back.
}

pub fn pick() -> i32 {
    liar();
}
```

Today that stops with `Unhandled error: 'liar' is declared noreturn but returned`.
Only put the attribute on something that really does end the program, such as a
call to [`panic`](xref:stdlib#stopping-early) or `exit`.

> [!NOTE]
> The [specification](https://github.com/mew-lang/mew/blob/main/spec/01-introduction.md)
> lists a third case, comparing a `string` with a `char`. The compiler rejects that
> comparison outright, so it is not something a program can reach.
