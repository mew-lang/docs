---
sidebar_position: 30
---

# Assignment

A value can be given a name using `let`. 

```mew
let x = 1;
```

In the example above, the type of `x` was inferred 
to be an integer `i32`, but in cases where you 
want to be explicit, you can specify the type.

```mew
let x: i16 = 1;
```

### Mutability

Variables in Mew are immutable by default.  
To be able to update an already assigned variable,
use the `mut` keyword.

```mew
let mut x = 1;
x = 2; // Variable can now be updated
```

