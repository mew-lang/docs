---
title: Loops
sidebar_position: 20
---

# Loops

### `loop`

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

### `while`

```mew
let mut foo = 0;
while foo < 100 {
    foo = foo + 1;
}
```

### `for`

`for` walks an array, binding each element in turn.

```mew
let primes = new int[] { 2, 3, 5, 7, 11 };

for prime in primes {
    println($"{prime}");
}
```

The loop variable belongs to the loop. It cannot be assigned to, it is not in
scope after the loop ends, and it may reuse a name from the enclosing scope.

```mew
let value = 100;

for value in primes {
    println($"{value}");
}

println($"{value}"); // 100
```

The collection is evaluated once, before the first iteration.

`break` and `continue` work as they do in the other loops.

```mew
let mut total = 0;

for prime in primes {
    if prime > 7 {
        break;
    }

    total += prime;
}
```

### Walking your own types

An array is not the only thing `for` walks. A type is walkable when it
implements `Enumerable<T>`, which the language declares:

```mew
pub interface Enumerator<T> {
    fn next() -> bool;
    fn current() -> T;
}

pub interface Enumerable<T> {
    fn iter() -> Enumerator<T>;
}
```

`iter` hands back a cursor. `next` moves it on and answers whether there is
anything there; `current` reads what it is. `for` calls `next` first, so a
cursor starts before the first element.

```mew
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
pub fn total(source: Enumerable<i32>) -> i32 {
    let mut sum = 0;
    for value in source {
        sum += value;
    }

    return sum;
}
```

A type is walkable one way. `iter` differs only in what it returns, and
[two functions cannot](../functions.md#overloading), so a type that reads more than one way
offers each as its own method returning its own collection.

Walking something that is neither an array nor an `Enumerable<T>` is an error.
