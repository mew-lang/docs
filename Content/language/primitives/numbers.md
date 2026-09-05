---
title: Numbers
uid: language.primitives.numbers
order: 3
---

Ten of the primitive types are numbers: eight integers and two floating point
types. `char` is not one of them, but it takes part in arithmetic and comparison
as its code point, so it appears in the tables below.

## Integers

| Length | Type  | Alias | Range                                                   |
| :----- | :---- | :---- | :------------------------------------------------------ |
| 8-bit  | `i8`  |       | -128 to 127                                             |
| 16-bit | `i16` |       | -32,768 to 32,767                                       |
| 32-bit | `i32` | `int` | -2,147,483,648 to 2,147,483,647                         |
| 64-bit | `i64` |       | -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 |
| 8-bit  | `u8`  |       | 0 to 255                                                |
| 16-bit | `u16` |       | 0 to 65,535                                             |
| 32-bit | `u32` |       | 0 to 4,294,967,295                                      |
| 64-bit | `u64` |       | 0 to 18,446,744,073,709,551,615                         |

An alias is another spelling of the same type rather than a distinct one. `int`
and `i32` are interchangeable everywhere.

## Floating point

| Length | Type  | Alias   | Precision     |
| :----- | :---- | :------ | :------------ |
| 32-bit | `f32` | `float` | 6 to 9 digits  |
| 64-bit | `f64` |         | 15 to 17 digits |

## Writing one down

A literal may be decimal or hexadecimal, and `_` may go between digits to group
them. The underscores are ignored.

```mew
use std;

let plain = 1;
let grouped = 100_000_000;
let hexadecimal = 0xDEADBEEF;
let grouped_hex = 0xDEAD_BEEF;
let fractional = 100_000.23_32;

println($"{plain} {grouped} {hexadecimal} {grouped_hex} {fractional}");
```

There is no negative literal. `-1` is the [unary minus](xref:language.operators#unary)
applied to `1`, which matters only where precedence does.

A `.` begins a fraction only when a digit follows it, so `1.max()` is a member
access on `1` rather than a malformed number. That is what lets a number carry
methods added by an [`impl` block](xref:language.extending#extending-a-primitive).

## What type a literal is

A literal with no suffix has no type of its own. It takes the type of wherever it
is used, and is checked against that type's range.

```mew
use std;

let byte: u8 = 200;
let wide: i64 = 9_000_000_000;

println($"{byte} {wide}");
```

A value that does not fit is an error, and it is about the value rather than about
the type: nothing is being converted, and the literal cannot be that type.

```mew error=MEW2004
let byte: u8 = 300;
```

With nothing to go on, a literal is an `i32` when it is whole and an `f32` when it
has a fraction.

## Suffixes

A suffix pins a literal to one type, which is how to write a value where nothing
else says what it should be.

```mew
use std;

let byte = 32u8;
let wide = 0xDEADBEEFu64;
let precise = 128.32f64;

println($"{byte} {wide} {precise}");
```

The suffixes are `i8`, `i16`, `i32`, `i64`, `u8`, `u16`, `u32`, `u64`, `f32` and
`f64`. Anything else is an error.

## Arithmetic

```mew
use std;

println($"{1 + 2}");
println($"{3 - 2}");
println($"{3 * 3}");
println($"{9 / 3}");
println($"{9 % 3}");

println($"{1 < 2}");
println($"{1 >= 2}");
```

Integer division truncates. `7 / 2` is `3`, not `3.5`, because both operands are
integers and the result is their coercion.

```mew
use std;

println($"{7 / 2}");
println($"{7.0 / 2.0}");
```

## Coercion

When a binary operator has operands of two different types, the result is the type
in this table. The row is the left operand and the column is the right. A blank
means the operator is not defined for that pair.

_Adding an `i8` to a `u32` gives an `i64`._

|          | i8  | i16 | i32 | i64 | u8  | u16 | u32 | u64 | f32 | f64 | char |
| -------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | ---- |
| **i8**   | i32 | i32 | i32 | i64 | i32 | i32 | i64 |     | f32 | f64 | i32  |
| **i16**  | i32 | i32 | i32 | i64 | i32 | i32 | i64 |     | f32 | f64 | i32  |
| **i32**  | i32 | i32 | i32 | i64 | i32 | i32 | i64 |     | f32 | f64 | i32  |
| **i64**  | i64 | i64 | i64 | i64 | i64 | i64 | i64 |     | f32 | f64 | i64  |
| **u8**   | i32 | i32 | i32 | i64 | i32 | i32 | u32 | u64 | f32 | f64 | i32  |
| **u16**  | i32 | i32 | i32 | i64 | i32 | i32 | u32 | u64 | f32 | f64 | i32  |
| **u32**  | i64 | i64 | i64 | i64 | u32 | u32 | u32 | u64 | f32 | f64 | u32  |
| **u64**  |     |     |     |     | u64 | u64 | u64 | u64 | f32 | f64 | u64  |
| **f32**  | f32 | f32 | f32 | f32 | f32 | f32 | f32 | f32 | f32 | f64 | f32  |
| **f64**  | f64 | f64 | f64 | f64 | f64 | f64 | f64 | f64 | f64 | f64 | f64  |
| **char** | i32 | i32 | i32 | i64 | i32 | i32 | u32 | u64 | f32 | f64 | i32  |

Two things in it are worth reading off directly.

Nothing narrower than `i32` comes out. Adding two `i8` values gives an `i32`,
which is why an expression widens even when both sides are the same narrow type.

```mew
use std;

let small: i8 = 3;
let wide: i64 = 4;
let sum = small + wide;

println($"{sum}");
```

`u64` has no result with any signed type, because no type in the table holds
every value of both. Mixing them is an error rather than a silent choice.

```mew error=MEW2008
let unsigned: u64 = 1;
let signed: i32 = 1;
let sum = unsigned + signed;
```

## Casting

Whether a conversion between two of these types exists, and whether it needs a
[cast](xref:language.type-casting). The row is the source and the column is the
target.

| Character | Means          |
| :-------- | :------------- |
| `i`       | Implicit        |
| `e`       | Explicit, so it needs `as` |
| blank     | Does not exist  |

|          | i8  | i16 | i32 | i64 | u8  | u16 | u32 | u64 | f32 | f64 | char |
| -------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | ---- |
| **i8**   | i   | i   | i   | i   | e   | e   | e   | e   | i   | i   | e    |
| **i16**  |     | i   | i   | i   | e   | e   | e   | e   | i   | i   | e    |
| **i32**  |     |     | i   | i   | e   | e   | e   | e   | i   | i   | e    |
| **i64**  |     |     |     | i   | e   | e   | e   | e   | i   | i   | e    |
| **u8**   | e   | i   | i   | i   | i   | i   | i   | i   | i   | i   | e    |
| **u16**  | e   | e   | i   | i   |     | i   | i   | i   | i   | i   | e    |
| **u32**  | e   | e   | e   | i   |     |     | i   | i   | i   | i   | e    |
| **u64**  | e   | e   | e   | e   |     |     |     | i   | i   | i   | e    |
| **f32**  | e   | e   | e   | e   | e   | e   | e   | e   | i   | i   | e    |
| **f64**  | e   | e   | e   | e   | e   | e   | e   | e   |     | i   | e    |
| **char** | e   | e   | i   | i   | e   | e   | i   | i   | i   | i   | i    |

A widening conversion is implicit. One that can lose information needs a cast.

The blanks are the surprise: narrowing to a smaller type of the same signedness
does not exist at all, and a cast does not help. `i64` to `i16`, `u16` to `u8`
and `f64` to `f32` are all rejected.

```mew error=MEW2007
let wide: i64 = 1;
let narrow = wide as i16;
```

Narrowing across signedness *is* explicit, so `u64` to `i8` is a cast that works
while `i64` to `i8` is not a conversion at all.

> [!NOTE]
> This is what the compiler does today. Whether it is what the language wants has
> not been settled.
