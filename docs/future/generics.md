---
sidebar_position: 100
---

# Generics

Types, interfaces and [functions](../language/generics.md#functions) take type
parameters today, and a parameter can be
[constrained](../language/generics.md#constraints). What remains is naming a
type argument where the compiler cannot work it out.

:::info
This functionality is not yet implemented
:::

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
