---
sidebar_position: 70
---

# Functions

### Defining functions

```mew
pub static fn square(value: i32) -> i32 {
    return value * value;
}

// Static function with optional label
pub static fn log(_ content: any) {
    print(content);
}
```

### Calling functions

```mew
let result = square(value: 32);
log(result);
```