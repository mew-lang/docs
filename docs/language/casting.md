---
sidebar_position: 50
---

# Casting

```mew
// This line declares a variable `i` and initializes 
// it with the value 32. The type of `i` is specified as any 
// which means it can hold values of any data type. 
// In this case, it holds an integer value.
let i : any = 32;

// This line attempts to cast the value stored in `i` to the 
// data type `i32` which is a signed 32-bit integer. 
let j = i as i32;
```

In summary, casting in this code snippet involves converting a value from the generic `any` type to the specific `i32` type, ensuring that the variable `j` is of the desired data type. This casting operation is possible because the original value in `i` is compatible with the target data type `i32`.