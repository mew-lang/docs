---
sidebar_position: 130
---

# Attributes

Attributes, indicated by the `[]` symbols, allows attachment of metadata, annotations, or special instructions to various language constructs, such as types, fields, and functions. Currently, only the built-in `ffi` attribute is supported.

### Syntax

An attribute is written before the construct it applies to, and can take
arguments.

```
[name]
[name("arg1", "arg2")]
```

### Example

The `ffi` attribute names the library an external function is found in.

```mew
[ffi("mylib")]
pub static external fn bar(first: i8) -> void;
```

An attribute that is not built in is an error, so there is nothing else to
put here yet.