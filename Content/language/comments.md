---
title: Comments
uid: language.comments
order: 5
---

A comment is trivia. It separates tokens and carries no other meaning, so nothing
in a program depends on one.

The compiler keeps comments on the syntax tree rather than throwing them away,
which is how the [formatter](xref:compiler.formatting) can put them back where
they were and how the language server can show a doc comment on hover.

## Line comments

`//` runs to the end of the line.

```mew
use std;

// A comment on its own line.
let total = 1 + 2; // And one after some code.

println($"{total}");
```

## Block comments

`/* */` runs to the first `*/`, across as many lines as it takes.

```mew
use std;

/*
   A comment
   over several lines.
*/
println("done");
```

Block comments do not nest. The first `*/` ends the comment, whatever `/*` came
before it, so commenting out a region that already contains one leaves the rest of
that region as code. A block comment that reaches the end of the file without
closing is an error.

## Doc comments

A third `/` makes a doc comment. It documents the declaration written under it.

```mew
use std;

/// How far something has walked.
pub type Counter {
    /// The number of steps taken so far.
    pub field steps: i32;

    /// Answers the count after one more step.
    pub fn next() -> i32 {
        return self.steps + 1;
    }
}

println($"{new Counter { steps: 41 }.next()}");
```

A doc comment is still trivia, so the compiler accepts one anywhere a comment can
go. A misplaced one is therefore not an error, and documents nothing.

```mew
use std;

pub fn steps() -> i32 {
    /// This documents nothing. A local is not a declaration,
    /// so there is nothing under this comment to attach it to.
    let count = 3;

    return count;
}

println($"{steps()}");
```

Doc comments belong on the things a reader looks up from outside: a type, a union,
an interface, a function, a field, a method. Inside a function body, an ordinary
`//` comment is what you want.
