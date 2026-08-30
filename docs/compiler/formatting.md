---
sidebar_position: 10
---

# Formatting

Mew has one formatter, and it is the same code whether it runs in an editor or
on the command line. There is nothing to configure.

## What it decides

The formatter decides indentation and the spacing between tokens. It does not
decide what goes on which line, so a line break you wrote is a line break it
keeps.

```mew
// Before
pub type Point{
pub field x:i32;
}
```

```mew
// After
pub type Point {
    pub field x: i32;
}
```

Runs of blank lines collapse to one, and a file that has any content at all
ends with a single newline.

## Running it

```shell
mew fmt                # every .mew file under the current directory
mew fmt src            # or under a directory you name
mew fmt main.mew       # or a single file
mew fmt --check .      # report what is unformatted, write nothing, exit 1
```

`--check` is what a build runs. Mew's own build uses it to hold the sample
programs to the style the formatter produces.

## Why it is safe to run

The formatter lexes its own output and compares it token by token against the
input. If anything differs it returns the original unchanged, so a formatting
rule cannot drop or reorder code.

:::info
A file that does not parse still formats. The formatter works off the token
stream rather than the syntax tree, which is the normal case while you are
still typing.
:::
