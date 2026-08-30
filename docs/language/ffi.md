---
sidebar_position: 130
---

# FFI

```mew
namespace Foo;

[ffi("mylib")]
pub static external fn bar(first: i8) -> void;
```