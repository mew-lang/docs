---
sidebar_position: 100
---

# Generics

A type and an interface [take type parameters](../language/generics.md) today,
and a parameter can be [constrained](../language/generics.md#constraints). What
remains is letting a function declare its own.

:::info
This functionality is not yet implemented
:::

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
