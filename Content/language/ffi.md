---
title: FFI
uid: language.ffi
order: 21
---

Some things cannot be written in Mew. Reading a file, writing to a terminal and
asking the operating system for the time all happen somewhere else, and the
foreign function interface is how a program reaches them.

An `external` function is a declaration with no body. The library named by its
[`ffi`](xref:language.attributes#ffi) attribute supplies the body, and the
declaration ends in `;` where a block would otherwise go.

```mew
[ffi("mylib")]
pub static external fn measure(first: i8) -> void;
```

The [standard library](xref:stdlib) is built on this and gets no special
treatment: `println` and `atoi` reach a native library called `mewstd` through
exactly these declarations, with no privilege the language does not give your own
code.

## It has to be static

An `external` function has no value to be called on, so it has to be `static`.

```mew error=MEW2045
[ffi("mylib")]
pub external fn measure(first: i8) -> void;
```

## Naming the function on the other side

`ffi` says which library. [`host`](xref:language.attributes) says which function
inside it, for when the name on the other side is not the name you want to write
in Mew.

```mew
[ffi("mewstd")]
[host("mew_write_line")]
pub static external fn write_line(text: string) -> void;
```

Without `host`, the function is looked up under its own name.

## What can cross

Only some types can go across the boundary, and the set is not the same in both
directions.

| | Allowed |
| :-- | :------ |
| **Parameters** | `i8` `i16` `i32` `i64` `u8` `u16` `u32` `u64` `f32` `f64`, `bool`, `string`, arrays |
| **Return type** | `i8` `i16` `i32` `i64` `u8` `u16` `u32` `u64` `f32` `f64`, `bool`, `void` |

A `string` or an array may go in but cannot come back, so a native function that
produces text writes into a buffer it was handed rather than returning one.

`char` crosses in neither direction, and neither does `any`, a type you declare, a
[union](xref:language.unions) or an [interface](xref:language.interfaces). Anything
else is an error at the declaration rather than a surprise at run time.

```mew error=MEW2070
pub type Point {
    pub field x: i32;
}

[ffi("mylib")]
pub static external fn take(point: Point) -> void;
```

To hand a declared type across, take it apart and pass what it holds.

## Nothing checks the other side

A declaration is a promise about a library the compiler cannot see. If the
signature does not match what is really there, nothing says so until the program
runs, and what happens then is the operating system's business rather than Mew's.

If the function ends the program rather than returning, say so with
[`[noreturn]`](xref:language.attributes#noreturn). That is exactly the case the
attribute exists for, and why the compiler takes it on trust rather than checking
a body it does not have.
