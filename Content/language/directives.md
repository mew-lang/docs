---
title: Directives
uid: language.directives
order: 19
---

A directive begins with `#` and tells the compiler something about the
compilation itself rather than about the program. There is one.

## `#load`

`#load` brings another source file into the compilation. The path is relative to
the file the directive is written in.

```mew
// file: helpers.mew
pub fn greet(name: string) -> string {
    return $"Hello, {name}!";
}

// file: stuff/utility.mew
pub fn twice(value: i32) -> i32 {
    return value * 2;
}

// file: main.mew
#load "helpers.mew"
#load "stuff/utility.mew"

use std;

println(greet("world"));
println($"{twice(21)}");
```

A [compilation](xref:getting-started#the-entry-point) is the starting file, every
file its directives reach, and the standard library. Nothing else is in scope, so
a name a program uses always comes from a file it named.

A file loaded more than once is still one member. Files are deduplicated by their
absolute path, so two directives that reach the same file from different
directories do not declare its contents twice.

Naming a path that matches nothing is an error rather than a silently smaller
compilation.

```mew error=MEW1040
#load "nothing.mew"
```

## Loading many files at once

A path may name several files with `*` or `?`. Matches are sorted by full path, so
the same directive always contributes the same files in the same order.

```mew
// file: stuff/first.mew
pub fn one() -> i32 {
    return 1;
}

// file: stuff/second.mew
pub fn two() -> i32 {
    return 2;
}

// file: main.mew
#load "stuff/*.mew"

use std;

println($"{one() + two()}");
```

## Loading is not scoping

`#load` decides which files are in the compilation. It does not decide what a name
means: that is [visibility](xref:language.namespaces#visibility) and
[resolution](xref:language.names#resolution-order).

Loading a file therefore does not reach its private declarations. A `pub` type in
a loaded file is visible; one without `pub` belongs to the file that declares it
however many other files load it.

Nor does loading import a namespace. A loaded file that declares one still needs a
`use`, or a written-out prefix, to be reached without it.

> [!NOTE]
> A directive is trivia rather than a statement, so it may be written anywhere in a
> file and no name resolves at one. Putting the `#load` lines at the top is a
> convention that helps a reader, not a rule.
