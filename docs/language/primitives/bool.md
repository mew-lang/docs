---
title: Booleans
sidebar_position: 30
---

# Booleans

Mew's boolean type is called `bool`, and represents a value that can be `true` or `false`.

```mew
true
false
```

## Operators

```mew
false && false // => false
false && true  // => false
true && false  // => false
true && true   // => true

false || false // => false
false || true  // => true
true || false  // => true
true || true   // => true
```

Like in most c-like languages, `&&` (logical and) and `||` (logical or) are short circuiting.  

## Negation

A boolean expression can be negated using the NOT operator `!`.

```mew
!true
!false
```