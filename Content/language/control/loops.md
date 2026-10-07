---
title: Loops
uid: language.control.loops
order: 3
---

## `loop`

```mew
let mut foo = 0;
loop {
    if foo < 100 {
        foo = foo + 1;
        continue;
    }
    
    break;
}
```

## `while`

```mew
let mut foo = 0;
while foo < 100 {
    foo = foo + 1;
}
```

## `for`

`for` walks an array, binding each element in turn.

```mew
use std;

let primes = new int[] { 2, 3, 5, 7, 11 };

for prime in primes {
    println($"{prime}");
}
```

The loop variable belongs to the loop. It cannot be assigned to, it is not in
scope after the loop ends, and it may reuse a name from the enclosing scope.

```mew
use std;

let primes = new i32[] { 2, 3, 5, 7, 11 };

let value = 100;

for value in primes {
    println($"{value}");
}

println($"{value}"); // 100
```

The collection is evaluated once, before the first iteration.

`break` and `continue` work as they do in the other loops.

```mew
use std;

let primes = new i32[] { 2, 3, 5, 7, 11 };

let mut total = 0;

for prime in primes {
    if prime > 7 {
        break;
    }

    total += prime;
}
```

## Counting

To loop over numbers, write a range. `a..b` counts from `a` up to `b`, and stops
just before it:

```mew
use std;

for n in 1..4 {
    println($"{n}");
}
```

```
1
2
3
```

Without a range, the same loop needs a counter of its own, and you have to
remember to move it on:

```mew
use std;

let mut n = 1;
while n < 4 {
    println($"{n}");
    n += 1;
}
```

Both ends are `i32`, and either can be any expression. `..` binds more loosely
than arithmetic, so `0..count + 1` runs up to and including `count`:

```mew
use std;

let count = 3;

for n in 0..count + 1 {
    println($"{n}");
}
```

```
0
1
2
3
```

A range whose end comes before its start is empty. The loop below doesn't run at
all, and that isn't an error:

```mew
use std;

for n in 5..2 {
    println("never printed");
}
```

A range is also a value, a [`Range`](xref:stdlib.std#range), so you can keep one
in a variable or pass it to a function. `start` and `end` are its two ends,
`contains` checks whether a number falls between them, and `count` returns how
many numbers it holds:

```mew
use std;

let span = 2..6;

println($"{span.start} {span.end}");
println($"{span.contains(4)} {span.contains(6)}");
println($"{span.count()}");
```

```
2 6
true false
4
```

It's a sequence like an array is, so `map`, `filter` and the rest work on it:

```mew
use std;

let squares = (1..5).map(|n| n * n).to_array();

println($"{squares}");
```

```
[1, 4, 9, 16]
```

> [!NOTE]
> A `for` over a range is a plain counting loop. It reads both ends once, before
> the first iteration, and allocates nothing.

## Walking your own types

An array is not the only thing `for` walks. A type is walkable when it
implements `Enumerable<T>`, which the language declares:

```mew ignore
pub interface Enumerator<T> {
    fn next() -> bool;
    fn current() -> T;
}

pub interface Enumerable<T> {
    fn iter() -> Enumerator<T>;
}
```

`iter` hands back a cursor. `next` moves it on and returns whether there is
anything there; `current` reads what it is. `for` calls `next` first, so a
cursor starts before the first element.

```mew
use std;

pub type Countdown {
    pub mut field at: i32;
    pub field from: i32;
}

impl Enumerator<i32> for Countdown {
    pub fn next() -> bool {
        self.at += 1;
        return self.at < self.from;
    }

    pub fn current() -> i32 {
        return self.from - self.at;
    }
}

pub type Descending {
    pub field from: i32;
}

impl Enumerable<i32> for Descending {
    pub fn iter() -> Enumerator<i32> {
        return new Countdown { at: -1, from: self.from, };
    }
}

for value in new Descending { from: 3, } {
    println($"{value}");  // 3, 2, 1
}
```

The collection may be the interface itself, so one function walks anything.

```mew
use std;

pub fn total(source: Enumerable<i32>) -> i32 {
    let mut sum = 0;
    for value in source {
        sum += value;
    }

    return sum;
}
```

An array counts as one, so it can be handed to that function directly.

```mew
use std;

pub fn total(source: Enumerable<i32>) -> i32 {
    let mut sum = 0;
    for value in source {
        sum += value;
    }

    return sum;
}

println($"{total(new i32[] { 1, 2, 3, 4 })}");
```

```
10
```

It also reaches whatever an [`impl` block on the interface](xref:language.interfaces#members-the-interface-supplies)
supplies, so a member written once is on every array.

```mew
impl Enumerable<T> {
    pub fn size() -> i32 {
        let mut n = 0;
        for item in self {
            n += 1;
        }

        return n;
    }
}
```

```mew
use std;

impl Enumerable<T> {
    pub fn size() -> i32 {
        let mut n = 0;
        for item in self {
            n += 1;
        }

        return n;
    }
}

println($"{new i32[] { 1, 2, 3 }.size()}");
println($"{new string[] { "a", "b" }.size()}");
```

```
3
2
```

> [!NOTE]
> `for` over an array is still a plain index loop rather than a walk through the
> protocol, so the most common loop in the language pays nothing for it.

A type is walkable one way. `iter` differs only in what it returns, and
[two functions cannot](xref:language.functions#overloading), so a type that reads more than one way
offers each as its own method returning its own collection.

Walking something that is not an array, a range or an `Enumerable<T>` is an error.
