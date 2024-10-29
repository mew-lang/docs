---
title: Numbers
sidebar_position: 10
---

# Numbers

## Integers

Mew's integer types represent a "whole" number.

```mew
let foo = 1;
let bar = -2;
let baz = 0x2A;
let qux = 0xDEADBEEF;
```

Large integers can also be separated with `_` to make
them easier to read.

```mew
let foo = 100_000_000;
```

### Signed integers

| Length | Type  | Alias | Range                                                   |
| :----- | :---- | :---- | :------------------------------------------------------ |
| 8-bit  | `i8`  |       | -128 to 127                                             |
| 16-bit | `i16` |       | -32,768 to 32,767                                       |
| 32-bit | `i32` | `int` | -2,147,483,648 to 2,147,483,647                         |
| 64-bit | `i64` |       | -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 |

### Unsigned integers

| Length | Type  | Range                           |
| :----- | :---- | :------------------------------ |
| 8-bit  | `u8`  | 0 to 255                        |
| 16-bit | `u16` | 0 to 65,535                     |
| 32-bit | `u32` | 0 to 4,294,967,295              |
| 64-bit | `u64` | 0 to 18,446,744,073,709,551,615 |

### Operators

```mew
1 + 2 // => 3
3 - 2 // => 1
3 * 3 // => 9
9 / 3 // => 3
9 % 3 // => 0

1 > 2  // => false
1 < 2  // => true
1 >= 2 // => false
1 <= 2 // => true
```

## Floating Point Numbers

Mew's float types represents a number with a decimal point.

```mew
let foo = 1.0;
let bar = -2.0;
```

Large floating point numbers can also be separated with `_` to make
them easier to read.

```mew
let foo = 100_000.23_32;
```

| Length | Type  | Approximate range | Precision     |
| :----- | :---- | :---------------- | :------------ |
| 32-bit | `f32` | N/A               | ~6-9 digits   |
| 64-bit | `f64` | N/A               | ~15-17 digits |

### Operators

```mew
1.0 + 2.0 // => 3.0
3.0 - 2.0 // => 1.0
3.0 * 3.0 // => 9.0
9.0 / 3.0 // => 3.0
9.0 % 3.0 // => 0.0

1.0 > 2.0  // => false
1.0 < 2.0  // => true
1.0 >= 2.0 // => false
1.0 <= 2.0 // => true
```

## Suffixes

You can use the types name as a suffix to specify what
the integer/floating point kind:

```mew
let foo = 32u8;
let bar = 128.32i16;
let qux = 0xDEADBEEFu64
```

## Coercion

_Example: Performing an add (`+`) of a `i8` and a `u32`, results in a `i64`._

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

## Casting

_Example: `i8` can be cast to `i32` implicitly._  
_Example: `i8` can only be cast to `u8` explicitly._

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
| **f64**  | i   | i   | i   | i   | i   | i   | i   | i   |     | i   | e    |
| **char** | e   | e   | i   | i   | e   | i   | i   | i   | i   | i   | i    |

#### Legend

| Character | Meaning       |
| --------- | ------------- |
| i         | Implicit cast |
| u         | Explicit cast |
| [blank]   | Not possible  |