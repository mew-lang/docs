---
sidebar_position: 110
---

# Iteration

:::info
This functionality is not yet implemented
:::

### `for`

`for` will enumerate anything that implements an iteration interface. What
that interface looks like has not been decided.

```mew
let primes = new int[] { 2, 3, 5, 7, 11 };

for prime in primes {
    print(prime);
}
```