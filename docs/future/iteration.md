---
sidebar_position: 100
---

# Iteration

### `for`

Anything that implements the `Iterator` interface
can be enumerated using `for`.

```mew
let primes = new int[] { 2, 3, 5, 7, 11 }

for prime in primes {
    print(prime);
}
```