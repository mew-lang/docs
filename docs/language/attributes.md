---
sidebar_position: 130
---

# Attributes

Attributes, indicated by the `[]` symbols, allows attachment of metadata, annotations, or special instructions to various language constructs, such as types, fields, and functions. Currently, only the built-in `ffi` attribute is supported.

### Example

```mew
[foo("arg1", "arg2")]
pub type Foo {
    [bar("arg1", "arg2")]
    pub field Bar: i32;

    [baz("arg1", "arg2")]
    pub fn Baz() {
    }
}
```