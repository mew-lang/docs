---
title: Directives
uid: language.directives
order: 21
---

A directive begins with `#` and tells the compiler something about the
compilation itself. There are three: `#load` brings in another file, and
`#name` and `#reference` are for [libraries](xref:language.libraries).

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

## `#name`

`#name` names what a file builds. A library has to have one, and it's also the
namespace the library's declarations live in. In `main.mew` it's optional and
names the program's output, so `#name "snake"` builds `snake.dll`.

```mew
#name "snake"

use std;

println("Hello from snake");
```

## `#reference`

`#reference` brings a library into the compilation. Its string starts with where
the library comes from. `local:` means a folder on your machine. The path after
it is relative to the file and names the folder the library's `lib.mew` is in.

```mew
// file: engine/lib.mew
#name "engine"

namespace engine;

pub fn twice(value: i32) -> i32 {
    return value * 2;
}

// file: main.mew
#reference "local:engine"

use engine;
use std;

println($"{twice(21)}");
```

`#name` and `#reference` only go in the file a build starts from: `main.mew`,
`lib.mew` or a build file. [Libraries](xref:language.libraries) covers the rest.
