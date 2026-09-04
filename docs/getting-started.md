---
sidebar_position: 1
---

# Getting Started

A Mew program is a file called `main.mew`.

```mew
use std;

println("Hello, world!");
```

Run it:

```shell
mew main.mew
```

```
Hello, world!
```

`run` is the default command, so a bare path runs the program. `mew run main.mew`
does the same thing.

## The entry point

A program starts at the top of `main.mew` and runs its statements in order.

Only that file may contain them. Every other file in the compilation holds
declarations, and a statement outside a declaration there is an error.

```mew
// helper.mew
use std;

pub fn greet(name: string) -> void {
    println($"Hello, {name}!");
}
```

```mew
// main.mew
#load "helper.mew"

greet("world");
```

:::note
The rule is the filename, and it is compared exactly. `Main.mew` is not an entry
point.
:::

## Exit code

A `return` at the top level ends the program and becomes its exit code. It has to
be an `int`, since that is all an exit code can carry.

```mew
use std;

println("giving up");
return 1;
```

Reaching the end of `main.mew` without a `return` exits with `0`.

## Building without running

`mew build` compiles the program and prints where it put it, without launching
anything.

```shell
mew build main.mew
```

```
/home/you/hello/.mew/main.dll
```

The build goes in a `.mew` directory beside the file the program starts from. The
leading dot matters: a directory whose name starts with one is skipped when
sources are discovered, so a build never becomes part of the next compilation.

A build is skipped entirely when nothing that decides the program has changed, so
running the same unchanged program twice only compiles it once.
