---
sidebar_position: 130
---

# FFI

```mew
namespace Foo;

[ffi("mylib")]
pub static external fn bar(first: i8) -> void;
```

An external function has no body, so the library supplies it. It has to be
`static`, since there is no value for it to be called on.
