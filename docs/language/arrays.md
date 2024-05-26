---
sidebar_position: 60
---

# Arrays

In the provided code snippet, we can see how arrays work:

```mew
let foo = new string[] { "A", "B", "C" };

let bar = i32[4];
bar[0] = 0;
bar[1] = 1;
bar[2] = 2;
bar[3] = 3;

let qux = new i32[4] { 0, 1, 2, 3 };
```

### Array initialization

```mew
let foo = new string[] { "A", "B", "C" };
```

This line declares an array of type `string` and initializes it with three string elements "A", "B", and "C". In Mew, arrays can be initialized with values directly using the curly braces syntax.

```mew
let bar = new i32[4];
```

This line declares an array of type `i32`, specifying the size of the array as 4. The array is created, but it is initially empty.

### Array element assignment

```mew
bar[0] = 0;
```

This line assigns the value 0 to the first element of the integer array bar at index 0. Array indices typically start at 0 in many programming languages.

```mew
bar[1] = 1;
```

Similarly, this line assigns 1 to the second element at index 1, and so on for the subsequent elements.

### Array Initialization with Values

```mew
let qux = new i32[4] { 0, 1, 2, 3 };
```

This line declares an array of type `i32` and initializes it with four integer values: 0, 1, 2, and 3. This is another way to create and initialize an array with specific values at the time of declaration.