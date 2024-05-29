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

Anything that implements the `Iterator` interface
can be enumerated using `for`.

```mew
let primes = new int[] { 2, 3, 5, 7, 11 }

for prime in primes {
    print(prime);
}
```