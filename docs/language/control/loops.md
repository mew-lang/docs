---
title: Loops
sidebar_position: 20
---

# Loops

### `loop`

```mew
let mut foo = 0;
loop {
    if foo < 100 {
        foo = foo + 1;
        continue;
    }
    
    break;
}
```

### `while`

```mew
let mut foo = 0;
while foo < 100 {
    foo = foo + 1;
}
```