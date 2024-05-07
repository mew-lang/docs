---
title: Text
sidebar_position: 20
---

# Text

### Strings

Mew's string type is called `string` and is written as text surrounded by quotes.  
Strings are represented as UTF-8 under the hood.

```
"Hello World"
"Hello 🌍"
```

```mew
let text = "Hello World";
```

#### String interpolation

```mew
let subject = "World";
let text = $"Hello {subject}";
```

### Characters

To represent a single UTF-16 character, there is the `char` type.

```mew
let space = ' ';
```

```mew
let space : char = 32;
```