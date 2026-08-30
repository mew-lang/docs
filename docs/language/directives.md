---
sidebar_position: 120
---

# Directives

### Import source files

`#load` brings another source file into the compilation. The path is relative
to the file the directive appears in.

```mew
#load "helpers.mew"
#load "stuff/utility.mew"
```

A path can name several files at once with `*` or `?`. Matches are sorted, so
the same sources always compile in the same order.

```mew
#load "stuff/*.mew"
```

A path that matches nothing is an error.
