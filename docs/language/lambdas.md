---
sidebar_position: 75
---

# Lambdas

A function can be held in a value, passed to another function and written
inline. That is what makes `map` and `filter` possible, and it is what a sort
takes to know which order you meant.

## A function type

A parameter says it takes a function by writing the signature with the name and
the body removed.

```mew
use std;

pub fn twice(n: i32) -> i32 {
    return n * 2;
}

pub fn apply(f: fn(i32) -> i32, v: i32) -> i32 {
    return f(v);
}
```

```mew
// Usage:
println($"{apply(twice, 21)}");
```

```
42
```

A function type reads the same wherever it is written, so two of them with the
same parameters and result are the same type. `fn() -> void` takes nothing and
answers nothing.

```mew
// Usage:
let held: fn(i32) -> i32 = twice;
let nothing: fn() -> void = shout;

println($"{held(4)}");
```

```
8
```

:::note
A name that means more than one function has no single type, so it cannot be
held on its own. Nothing at that point says which one was meant.
:::

## A lambda

A function written where it is used. The parameters go between two bars and the
body follows.

```mew
// Usage:
println($"{apply(|n| n * 2, 21)}");
println($"{apply(|n| n + 1, 41)}");
```

```
42
42
```

`|a, b|` takes two, `||` takes none, and a block is a body like any other.

```mew
use std;

pub fn combine(f: fn(i32, i32) -> i32) -> i32 {
    return f(3, 4);
}

pub fn run(action: fn() -> void) -> void {
    action();
}
```

```mew
// Usage:
println($"{combine(|a, b| a + b)}");
run(|| println("nothing to take"));

println($"{apply(|n| {
    let doubled = n * 2;
    return doubled + 1;
}, 20)}");
```

```
7
nothing to take
41
```

### Where the parameter types come from

A lambda does not say what its parameters are. Whatever the value is going into
does.

```mew
// apply takes a fn(i32) -> i32, so `n` is an i32
apply(|n| n * 2, 21);
```

Where nothing says, write the types down and the lambda describes itself.

```mew
// Usage:
let doubler = |n: i32| n * 2;

println($"{doubler(21)}");
```

```
42
```

A lambda with no parameters describes itself too, since there is nothing left to
say about it.

```mew
// Usage:
let seven = || 7;

println($"{seven()}");
```

```
7
```

Where neither is true, there is nothing to work the parameters out from.

```mew
let f = |x| x * 2;
```

```
Error [MEW2091]: No function to infer
Nothing here says what this takes, so its parameters cannot be worked out
```

### A lambda sees what is around it

The values in scope where a lambda is written stay reachable from inside it, for
as long as the lambda lives.

```mew
// Usage:
let factor = 3;

println($"{apply(|n| n * factor, 14)}");
```

```
42
```

## Generic higher-order functions

A [type parameter](./generics.md) can appear in a function type, and the lambda
handed in is what settles it.

```mew
use std;

pub fn mapped<T, U>(items: T[], f: fn(T) -> U) -> U[] {
    let out = new U[items.count];
    let mut i = 0;
    while i < items.count {
        out[i] = f(items[i]);
        i += 1;
    }

    return out;
}
```

```mew
// Usage:
let numbers = new i32[] { 1, 2, 3 };

let doubled = mapped(numbers, |n| n * 2);
let texts = mapped(numbers, |n| $"<{n}>");

println($"{doubled[2]}");
println(texts[1]);
```

```
6
<2>
```

`U` is worked out from the lambda's body, so `mapped` answers an `i32[]` in the
first call and a `string[]` in the second. The arguments that already have a
type settle what they can first, which is how `T` is known by the time the
lambda is read.

Where nothing pins a type parameter, the compiler says which one.

```mew
pub fn only<T, U>(f: fn(T) -> U) -> i32 { return 1; }

let answer = only(|n| n);
```

```
Error [MEW2066]: 'T' cannot be worked out from the arguments to 'only'
```

## Not yet

A lambda's return type is never written down. It is whatever the body produces,
and where that is wrong the error is about the body.

There is no partial application and no way to combine two functions into a
third. A lambda that closes over what it needs covers the same ground.
