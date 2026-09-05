---
title: Booleans
uid: language.primitives.bool
order: 6
---

`bool` holds `true` or `false`, and nothing else.

```mew
use std;

let ready = true;
let done = false;

println($"{ready} {done}");
```

It is what the comparison operators answer with, and what
[`if`](xref:language.control.conditions) and `while` require. Nothing converts to
a `bool`, so a number is never a condition on its own.

## Operators

`&&` is and, `||` is or, and `!` negates.

```mew
use std;

let ready = true;
let done = false;

println($"{ready && done}");
println($"{ready || done}");
println($"{!ready}");
```

`==` and `!=` compare two of them.

```mew
use std;

let ready = true;

println($"{ready == true}");
println($"{ready != false}");
```

## Short circuiting

`&&` and `||` evaluate their right operand only when the left has not already
settled the answer. `false && x` never looks at `x`, and neither does `true || x`.

That is what lets a cheap test guard an expensive or unsafe one, because the
guard is evaluated first and the rest is skipped when it fails.

```mew
use std;

pub fn at(values: i32[], index: i32) -> i32 {
    if index < values.count && values[index] > 0 {
        return values[index];
    }

    return -1;
}

println($"{at(new i32[] { 5, 6 }, 0)}");
println($"{at(new i32[] { 5, 6 }, 9)}");
```

The index check runs first, so the read never happens when the index is out of
range. Every other binary operator evaluates both sides, left before right,
before it is applied.

```mew
use std;

pub fn noisy(answer: bool) -> bool {
    println("evaluated");
    return answer;
}

let skipped = false && noisy(true);

println($"{skipped}");
```

That program prints `false` and nothing else. `noisy` is never called.
