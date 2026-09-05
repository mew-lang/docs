---
title: Operators
uid: language.operators
order: 14
---

## Precedence

Tightest first. An operator binds its operands more tightly than anything below it
in this table.

| Level | Operators | Groups |
| ----: | :-------- | :----- |
| 15 | `a[i]` | left |
| 14 | `f(x)` | left |
| 13 | `a.b`&nbsp;&nbsp;`a::b` | left |
| 12 | `!a`&nbsp;&nbsp;`-a`&nbsp;&nbsp;`+a` | right |
| 11 | `*`&nbsp;&nbsp;`/`&nbsp;&nbsp;`%` | left |
| 10 | `+`&nbsp;&nbsp;`-` | left |
| 9 | `is` | left |
| 8 | `as` | left |
| 7 | `<`&nbsp;&nbsp;`<=`&nbsp;&nbsp;`>`&nbsp;&nbsp;`>=` | left |
| 6 | `==`&nbsp;&nbsp;`!=` | left |
| 5 | `&&` | left |
| 4 | `\|\|` | left |
| 1 | `=`&nbsp;&nbsp;`+=`&nbsp;&nbsp;`-=`&nbsp;&nbsp;`*=`&nbsp;&nbsp;`/=`&nbsp;&nbsp;`%=` | right |

Parentheses group, and a grouped expression is whatever is inside it.

> [!IMPORTANT]
> `as` and `is` bind looser than arithmetic, which is not how they read. Both
> take a type on the right rather than an expression, so everything the arithmetic
> operators have already claimed is the left operand.
>
> `a + b as i64` is `(a + b) as i64`, not `a + (b as i64)`.

```mew
use std;

let a: i32 = 7;
let b: i32 = 2;

let together = a + b as i64;   // (a + b) as i64
let separate = a + (b as i64); // the other reading, written out

println($"{together} {separate}");
```

The two answer the same here, but they do not in general: the cast applies to a
different value, so a sum that overflows an `i32` overflows before it widens.
Write the parentheses when the difference matters.

Member access binds tighter than unary, so `-point.x` negates the field rather
than the value. A call binds tighter than the access it is part of, which is why
`value.doubled()` reads as one thing.

## Arithmetic

| Operator | On | Produces |
| :------- | :- | :------- |
| `*` `/` `%` `-` | two numbers | their [coercion](xref:language.primitives.numbers#coercion) |
| `+` | two numbers | their coercion |
| `+` | two strings | `string` |
| `+` | a `string` and a `char` | `string` |

```mew
use std;

println($"{7 * 6}");
println($"{7 / 2}");
println($"{7 % 2}");
println("Hello, " + "world");
println("x" + 'y');
```

`+` joins text only for those two cases. To put a number or a `bool` into a
string, [interpolate](xref:language.primitives.text#string-interpolation) it.

```mew error=MEW2008
let text = "count: " + 3;
```

## Comparison

`<`, `<=`, `>` and `>=` compare two numbers and answer a `bool`. A `char` takes
part as its code point.

```mew
use std;

println($"{1 < 2}");
println($"{'a' < 'b'}");
```

`==` and `!=` work on the numeric types, `string`, `bool` and `char`, the types
that have a value to compare.

```mew
use std;

let name = "Ada";

println($"{1 == 1}");
println($"{name == "Ada"}");
println($"{true != false}");
println($"{'x' == 'x'}");
```

They do not work on a type you declare, on a [union](xref:language.unions), on an
array, or on `any`. There is no structural comparison and no way to give a type
one.

```mew error=MEW2008
pub type Point {
    pub field x: i32;
}

let a = new Point { x: 1 };
let b = new Point { x: 1 };
let same = a == b;
```

To compare two values of your own type, write a method that says what comparing
them means.

```mew
use std;

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Point {
    pub fn equals(other: Point) -> bool {
        return self.x == other.x && self.y == other.y;
    }
}

println($"{new Point { x: 1, y: 2 }.equals(new Point { x: 1, y: 2 })}");
```

To ask which case a union holds, [`match`](xref:language.unions#reading-a-value)
is what asks.

## Logical

`&&`, `||` and `!` work on `bool` and answer one. `&&` and `||`
[short circuit](xref:language.primitives.bool#short-circuiting); everything else
evaluates both sides, left before right, before the operator is applied.

```mew
use std;

let ready = true;
let done = false;

println($"{ready && !done}");
println($"{done || ready}");
```

## Unary

| Operator | On | Produces |
| :------- | :- | :------- |
| `!` | `bool` | the negation |
| `-` | a number | the negation |
| `+` | a number | the operand unchanged |

There is no negative literal, so `-1` is this `-` applied to `1`. That matters
only where it changes the reading: `-a.b` is `-(a.b)`.

```mew
use std;

let value = 5;

println($"{-value}");
println($"{+value}");
println($"{!(value > 10)}");
```

## Type operators

[`is`](xref:language.type-checking) asks what a value is and answers a `bool`.
[`as`](xref:language.type-casting) performs a conversion that is not implicit.
Both take a type on the right, and both bind looser than arithmetic.

```mew
use std;

let boxed: any = 32;

if boxed is i32 {
    println($"{boxed as i32}");
}
```

## Assignment

`=` and the compound operators are the loosest of all, and they group right to
left. Assignment is an expression that produces the value it assigned, which
[Assignment](xref:language.assignment#assignment-is-an-expression) covers.

## What is not here

There is no conditional operator, no bitwise operator, no shift, no increment or
decrement, and no way to give a type an operator of its own. A method is how a
type says what an operation on it means.
