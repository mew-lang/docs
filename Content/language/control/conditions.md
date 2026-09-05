---
title: Conditions
uid: language.control.conditions
order: 2
---

`if` runs a block when a condition holds, and `else` says what to do when it does
not.

```mew
use std;

let value = 3;

if value == 0 {
    println("zero");
} else if value < 0 {
    println("negative");
} else {
    println("positive");
}
```

There are no parentheses around the condition, and the branches are always blocks.
There is no single statement form, so a body is written in braces however short it
is.

## The condition is a `bool`

A condition has to be a [`bool`](xref:language.primitives.bool) and nothing else.
Mew has no notion of a truthy value, so a number is not a condition, and neither
is a value that might be `null`.

```mew error=MEW2007
let count = 1;

if count {
    let unreachable = 0;
}
```

Compare it instead, and the comparison is the `bool`.

```mew
use std;

let count = 1;

if count != 0 {
    println("not zero");
}
```

The operators that produce one are the comparisons and the logical operators, and
`&&` and `||` [short circuit](xref:language.primitives.bool#short-circuiting), so a
cheap test can guard an expensive one.

```mew
use std;

pub fn at(values: i32[], index: i32) -> i32 {
    if index >= 0 && index < values.count {
        return values[index];
    }

    return -1;
}

println($"{at(new i32[] { 1, 2, 3 }, 1)}");
println($"{at(new i32[] { 1, 2, 3 }, 9)}");
```

## An `if` produces no value

`if` is a statement. It does not produce a value, so it cannot be assigned from,
and Mew has no conditional expression to reach for instead.

Write the branch as an assignment to a `mut` local.

```mew
use std;

let value = 3;
let mut label = "";

if value < 0 {
    label = "negative";
} else {
    label = "positive";
}

println(label);
```

Or return from each branch, which is usually clearer.

```mew
use std;

pub fn label(value: i32) -> string {
    if value < 0 {
        return "negative";
    }

    return "positive";
}

println(label(3));
```

[`match`](xref:language.unions#matching-as-a-value) is the one conditional that
does read as a value, and it works on a [union](xref:language.unions) rather than
on a `bool`.

> [!NOTE]
> A conditional expression, the `a ? b : c` of other languages, does not exist.
> `?` is lexed and reserved for one, so writing it is an error rather than
> something stranger. [Ternaries](xref:future.ternaries) covers what is planned.
