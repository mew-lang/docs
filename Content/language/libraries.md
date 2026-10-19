---
title: Libraries
uid: language.libraries
order: 22
---

A library is Mew code you want to use from more than one program, such as a
small game engine that several games share. It lives in a folder of its own and
starts at a file called `lib.mew`. A program pulls it in with `#reference`:

```mew
// file: ../engine/lib.mew
#name "engine"
#load "color.mew"

// file: engine/color.mew
namespace engine;

pub type Color {
    pub field r: u8;
    pub field g: u8;

    pub static fn grey(level: u8) -> Color {
        return new Color { r: level, g: level };
    }
}
```

```mew
// file: main.mew
#reference "local:../engine"

use engine;
use std;

println($"{Color::grey(24).g}");
```

`lib.mew` is to a library what `main.mew` is to a program. The compiler tells
them apart by the file name, so there's no project file to keep next to either.
A folder can hold one or the other, but not both.

## Naming a library

```mew
// file: lib.mew
#name "engine"
```

## What's in a library

A library is its `lib.mew` and every file that `lib.mew` loads, the same way a
program is its `main.mew` and the files it loads. A file sitting in the folder
that nothing loads isn't part of the library.

When you open one of the library's files in the editor, it finds the `lib.mew`
above it and checks the file as part of the whole library. A file can use a type
from a sibling file without loading that file itself, as long as `lib.mew` loads
both.

## Referencing a library

A reference starts with where the library comes from, and `local:` means a
folder on your machine. The path after it is relative to the file it's written
in, and it names the library's folder rather than its `lib.mew`. Leave out the
`local:` and the compiler tells you to add it. A library can reference other
libraries the same way, and a program can use every library its references
reach:

```mew
// file: ../engine/lib.mew
#name "engine"

namespace engine;

pub fn twice(value: i32) -> i32 {
    return value * 2;
}
```

```mew
// file: ../physics/lib.mew
#name "physics"
#reference "local:../engine"

namespace physics;

use engine;

pub fn fall(height: i32) -> i32 {
    return twice(height);
}
```

```mew
// file: main.mew
#reference "local:../physics"

use engine;
use physics;
use std;

println($"{fall(twice(5))}");
```

Two references to the same folder are one library, so it doesn't matter how
many of your libraries name the engine. Two different folders with the same
name are an error, and so is a library that ends up referencing itself.

## What a program can see

A library shows programs its `pub` declarations and nothing else. `internal`
opens a declaration to every file of the library while keeping it away from
the programs that use it, which is what you want for the helpers your library is
built from:

```mew
// file: ../engine/lib.mew
#name "engine"
#load "clock.mew"

namespace engine;

pub fn ticks() -> i32 {
    return frames() * 2;
}

// file: engine/clock.mew
namespace engine;

internal fn frames() -> i32 {
    return 30;
}
```

```mew
// file: main.mew
#reference "local:../engine"

use engine;
use std;

println($"{ticks()}");
```

A program can still add members to a library's types with `impl`, the way it
can to [any type](xref:language.extending), but it can't make one of them
implement an interface. Only the library that declares a type decides what it
implements.

## Building

`mew run main.mew` builds every library the program reaches and puts each one
next to the program, so the engine example leaves `main.dll` and `engine.dll`
side by side in `.mew`. `mew compile engine/lib.mew` builds the library on its
own.
