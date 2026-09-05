---
title: Arrays
uid: language.arrays
order: 13
---

An array holds a fixed number of values of one type. It is written as the element
type followed by `[]`, and its length is decided when it is created.

```mew
use std;

let letters = new string[] { "A", "B", "C" };

println($"{letters[0]}");
```

## Creating one

There are three ways to say how long an array is, and one of them has to be used.

Give it values, and the length is how many there are.

```mew
use std;

let primes = new i32[] { 2, 3, 5, 7, 11 };

println($"{primes.count}");
```

Give it a size, and every element starts at the zero of the element type. That is
`0` for a number, `false` for a `bool`, and `null` for anything that can hold one.

```mew
use std;

let counts = new i32[3];

println($"{counts[0]}, {counts[1]}, {counts[2]}");
```

Give it both, and the compiler checks they agree.

```mew
use std;

let scores = new i32[3] { 10, 20, 30 };

println($"{scores.count}");
```

An initializer whose length is not the size is an error.

```mew error=MEW2027
let wrong = new i32[3] { 1, 2 };
```

So is giving neither, since nothing then says how long the array is.

```mew error=MEW2026
let unknown = new i32[];
```

## Reading and writing elements

Indexing is `[` and an `i32`. The first element is at `0`.

```mew
use std;

let primes = new i32[] { 2, 3, 5, 7, 11 };

println($"{primes[0]}");
println($"{primes[primes.count - 1]}");
```

An element is assignable whatever the binding's own mutability, because the
assignment writes into the array rather than to the name. `mut` on the `let`
would be for pointing the name at a different array.

```mew
use std;

let counts = new i32[3];
counts[0] = 7;

println($"{counts[0]}");
```

Nothing checks an index before it is used. One outside the array ends the
program, and `count` is what a program checks against itself.

```mew
use std;

pub fn at(values: i32[], index: i32) -> i32 {
    if index < 0 || index >= values.count {
        return -1;
    }

    return values[index];
}

println($"{at(new i32[] { 1, 2, 3 }, 9)}");
```

Only an array can be indexed. `[` on anything else is an error, which is why a
`string` is walked through `chars()` rather than indexed directly.

```mew error=MEW2028
let text = "hello";
let first = text[0];
```

## Counting the elements

`count` is the number of elements. It is an `i32`, it cannot be assigned to, and
it is the only member an array has of its own.

```mew
use std;

let letters = new string[] { "A", "B", "C" };
let slots = new i32[8];

println($"{letters.count + slots.count}");
```

Because the length travels with the array, a function that is handed one does not
need to be told how long it is.

```mew
use std;

pub fn total(values: i32[]) -> i32 {
    let mut sum = 0;
    let mut index = 0;
    while index < values.count {
        sum += values[index];
        index += 1;
    }

    return sum;
}

println($"{total(new i32[] { 1, 2, 3, 4 })}");
```

That loop is written out to show what `count` is for. To visit every element,
[`for`](xref:language.control.loops#for) says the same thing in less.

```mew
use std;

let mut sum = 0;

for value in new i32[] { 1, 2, 3, 4 } {
    sum += value;
}

println($"{sum}");
```

## An array is a sequence

`count` is the only member an array has of its own. Everything else it can do
comes from being a [sequence](xref:stdlib#sequences): an array converts to
`Enumerable<T>` on its own, so it carries `map`, `filter`, `fold`, `find`, `any`,
`count()` and `to_array()`.

```mew
use std;

let values = new i32[] { 1, 2, 3, 4 };

let doubled = values
    .filter(|n| n % 2 == 0)
    .map(|n| n * 10)
    .to_array();

println($"{doubled.count}");
println($"{doubled[0]}");
```

The same conversion is why a function that takes `Enumerable<T>` takes an array.

```mew
use std;

pub fn how_many(source: Enumerable<i32>) -> i32 {
    return source.count();
}

println($"{how_many(new i32[] { 1, 2, 3 })}");
```

> [!NOTE]
> `count` on an array is a field and costs nothing. `count()` on a sequence is a
> method and walks it. An array has both, and the one without parentheses is the
> one that already knows the answer.

## What an array is not

An array is not a named type. It is written as its element type followed by `[]`
rather than declared, so it cannot be the target of an
[`impl` block](xref:language.extending): `impl i32[]` is an error. To give every
array a member, write the block on `Enumerable<T>` instead.

Two array types convert only when their element types are the same. An `i32[]` is
not an `i64[]`, however the elements themselves convert.

```mew error=MEW2007
let numbers = new i32[] { 1, 2, 3 };
let wider: i64[] = numbers;
```

> [!WARNING]
> An array of a [union](xref:language.unions) created with a size and no
> initializer holds `null` in every element, and a union has no such value. Reading
> one before writing it is [undefined](xref:language.undefined). Give the array an
> initializer, or fill it before reading it.
