---
sidebar_position: 100
---

# Generics

:::info
This functionality is not yet implemented
:::

### Types

A type parameter is declared in angle brackets, and constrained with `:`.

```mew
pub interface Comparable<T> {
    fn compare_to(other: T) -> i32;
}

pub type Smallest<T: Comparable<T>> {
    field current: T;

    pub static fn new(first: T) -> Smallest<T> {
        return new Smallest<T> { current: first };
    }

    pub fn add(item: T) {
        if item.compare_to(self.current) < 0 {
            self.current = item;
        }
    }

    pub fn get() -> T {
        return self.current;
    }
}
```

```mew
// Usage:
let smallest = Smallest<i32>::new(41);
smallest.add(7);
println(smallest.get());
```

### Unions

A union takes type parameters the same way. See [Unions](./unions.md) for the
rest of what a union can do, including `match`.

```mew
pub union Result<T, E> {
    Ok(T),
    Err(E),
}
```
