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

#### Escape sequences

A backslash begins an escape sequence.

| Sequence  | Meaning                                 |
| --------- | --------------------------------------- |
| `\0`      | Null                                    |
| `\a`      | Alert                                   |
| `\b`      | Backspace                               |
| `\f`      | Form feed                               |
| `\n`      | New line                                |
| `\r`      | Carriage return                         |
| `\t`      | Tab                                     |
| `\v`      | Vertical tab                            |
| `\\`      | Backslash                               |
| `\'`      | Single quote                            |
| `\"`      | Double quote                            |
| `\uXXXX`  | The character with that hexadecimal code |

```mew
let path = "C:\\folder\\file";
let quoted = "she said \"hello\"";
let tabbed = "left\tright";
let letter = "\u0041";
```

Any other character after a backslash is an error.

#### String interpolation

A string with a `$` in front of it can contain holes, written in curly braces.
Each hole is an expression, and its value becomes part of the text.

```mew
let subject = "World";
let text = $"Hello {subject}";
```

A hole can hold any expression.

```mew
let count = 3;
let text = $"{count} of {count + 1}";
```

A hole must be a type that has a text representation: the integer and floating
point types, `char`, `bool` and `string`. A `bool` appears as `true` or `false`,
the same as in source.

```mew
pub type Point {
    pub field x: i32;
}

let point = new Point { x: 1, };
let text = $"{point}"; // Error: there is no text representation for 'Point'
```

To put a curly brace in the text, double it.

```mew
let text = $"{{ and }}"; // => { and }
```

### Characters

To represent a single UTF-16 character, there is the `char` type.

```mew
let space = ' ';
```

```mew
let space : char = 32;
```

Escape sequences work the same way they do in a string.

```mew
let newline = '\n';
let quote = '\'';
let backslash = '\\';
```
