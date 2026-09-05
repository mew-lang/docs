---
title: "Null"
uid: language.primitives.null
order: 2
---

The `null` keyword represents a value indicating that there is no value.

```mew
let text: string = null;
```

### What can be null

A `string`, an array, `any`, and a type you declare can hold `null`. The integer
and floating point types, `bool` and `char` cannot, so assigning `null` to one is
an error.

```mew
let text: string = null;
let items: i32[] = null;
let value: any = null;
```

An integer, a float, a `bool` or a `char` has no such value.

```mew error=MEW2007
let count: i32 = null;
```

### Null needs a type

`null` says nothing about the type a variable should have, so a `let` with only
`null` to go on is an error. Write the type down.

```mew error=MEW2014
let text = null;
```

```mew
let text: string = null;
```
