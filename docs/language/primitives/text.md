---
title: Text
sidebar_position: 20
---

# Text

### Strings

Mew's string type is called `string` and is written as text surrounded by quotes.
A string is a sequence of characters. How those are stored is not part of the
language, and a compiler is free to hold them however it likes.

```
"Hello World"
"Hello 🌍"
```

```mew
let text = "Hello World";
```

#### Joining strings

`+` joins two strings into a new one.

```mew
let greeting = "Hello, " + "world" + "!";
```

Only strings can be joined this way. To put a number or a `bool` into text, use
[interpolation](#string-interpolation).

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
| `\uXXXX`  | The character with that four digit hexadecimal code |
| `\u{XXXXXX}` | The character with that hexadecimal code, up to six digits |

```mew
let path = "C:\\folder\\file";
let quoted = "she said \"hello\"";
let tabbed = "left\tright";
let letter = "\u0041";
```

`\uXXXX` takes exactly four digits, which only reaches the first 65,536
characters. The braced form reaches the rest.

```mew
let world = "\u{1F30D}";
let letter = "\u{41}";
```

A braced escape that names no character is an error: there is nothing above
`\u{10FFFF}`, and the surrogate range `\u{D800}` to `\u{DFFF}` exists only to
encode other characters rather than to be one.

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

#### Walking a string

A string cannot be indexed. `chars()` hands back its characters as an array,
which can be indexed, counted and walked.

```mew
let text = "Hello";

println($"{text.chars().count}"); // 5
println($"{text.chars()[0]}");     // H

for letter in text.chars() {
    println($"{letter}");
}
```

Indexing counts characters rather than storage, so it does not depend on how a
compiler holds the text.

```mew
let world = "Hello 🌍";
let last = world.chars()[6]; // '🌍'
```

#### Building a string

Adding a character to a string gives a string, so text can be built up a
character at a time. An array of characters converts back with a cast.

```mew
let text = "Hello 🌍";
let letters = text.chars();

let mut reversed = "";
let mut i = letters.count - 1;
while i >= 0 {
    reversed += letters[i];
    i -= 1;
}

println(reversed);              // 🌍 olleH
println(letters as string);     // Hello 🌍
println("x" + 'y');             // xy
println('a' as string);         // a
```

A character does not become a string on its own, only where `+` says the
answer is text or where a cast asks for it. So `let text: string = 'a';` is an
error, and comparing a string with a character is undefined rather than false.

### Characters

The `char` type holds a single character. Any character, including one outside
the first 65,536.

```mew
let space = ' ';
let world = '🌍';
```

A character is also a number, its Unicode code point, so it takes part in
arithmetic and comparison like the integer types do.

```mew
let space : char = 32;
let next = 'a' + 1;
```

:::note
A character is one code point, which is not always one thing a reader would
point at. A flag or a letter followed by a combining accent is written with more
than one, and counts as more than one.
:::

Escape sequences work the same way they do in a string.

```mew
let newline = '\n';
let quote = '\'';
let backslash = '\\';
```
