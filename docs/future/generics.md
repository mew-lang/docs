---
sidebar_position: 100
---

# Generics

A type and an interface [take type parameters](../language/generics.md) today.
What remains is constraining them, and letting a function declare its own.

:::info
This functionality is not yet implemented
:::

### Constraints

A parameter is constrained with `:`, and the constraint says what the type has,
which is what makes a value of that type useful for more than storage.

```mew
pub interface Comparable<T> {
    fn compare_to(other: T) -> i32;
}

pub type Smallest<T: Comparable<T>> {
    field current: T;

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

### Functions

A function should be able to take parameters of its own, and have them worked
out from the arguments rather than written at the call.

```mew
pub fn first<T>(values: T[]) -> T {
    return values[0];
}

let earliest = first(new i32[] { 3, 1, 2, });
```

### Naming a type in an expression

A static method is reached through the type, which means writing the arguments
where `<` would otherwise be a comparison.

```mew
let smallest = Smallest<i32>::new(41);
```

Deciding this needs a rule for when `<` opens a type argument list. The
formatter already has one, and it reads the token after the matching `>`.

### Unions

A union takes type parameters the same way. See [Unions](./unions.md) for the
rest of what a union can do, including `match`.

```mew
pub union Result<T, E> {
    Ok(T),
    Err(E),
}
```
