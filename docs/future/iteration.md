---
sidebar_position: 110
---

# Iteration

[`for`](../language/control/loops.md#for) walks an array today. What remains is
letting it walk anything else.

:::info
This functionality is not yet implemented
:::

### An iteration interface

A type should be able to say that it can be walked, and `for` should accept
anything that says so. What that interface looks like has not been decided:
whether it hands out one element at a time or a position that moves, and what it
answers when there is nothing left.

```mew
pub type Range {
    pub field from: i32;
    pub field to: i32;
}

for value in new Range { from: 0, to: 10, } {
    println(itoa(value));
}
```

Until then, `chars()` and the other sequence-producing operations hand back an
array, which `for` already accepts. That works, but it builds the whole sequence
before the loop starts, which an interface would not have to.
