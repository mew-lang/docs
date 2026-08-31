---
title: "Null"
sidebar_position: 1
---

# Null

The `null` keyword represents a value indicating that there is no value.

```mew
let text: string = null;
```

### What can be null

A `string`, an array, `any`, and a type you declare can hold `null`. The integer
and floating point types, `bool` and `char` cannot, so assigning `null` to one is
an error.

```mew
let text: string = null;  // ok
let items: i32[] = null;  // ok
let value: any = null;    // ok

let count: i32 = null;    // Error: cannot convert type 'null' to 'i32'
```

### Null needs a type

`null` says nothing about the type a variable should have, so a `let` with only
`null` to go on is an error. Write the type down.

```mew
let text = null;  // Error: cannot assign null to an implicitly-typed variable
```

```mew
let text: string = null;  // ok
```
