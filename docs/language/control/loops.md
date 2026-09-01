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
    println(itoa(prime));
}
```

The loop variable belongs to the loop. It cannot be assigned to, it is not in
scope after the loop ends, and it may reuse a name from the enclosing scope.

```mew
let value = 100;

for value in primes {
    println(itoa(value));
}

println(itoa(value)); // 100
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

An array is the only thing that can be walked today. Walking anything else is an
error, and what a type has to provide to be walkable is
[still being decided](../../future/iteration.md).
