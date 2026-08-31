---
sidebar_position: 60
---

# Arrays

In the provided code snippet, we can see how arrays work:

```mew
let foo = new string[] { "A", "B", "C" };

let mut bar = new i32[4];
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
let mut bar = new i32[4];
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

### Counting the elements

Every array has a `count`, which is the number of elements it holds. It is an
`i32`, and it cannot be assigned to.

```mew
let foo = new string[] { "A", "B", "C" };
let letters = foo.count; // 3

let bar = new i32[8];
let slots = bar.count;   // 8
```

`count` is what lets a function walk an array it was handed, since the length
travels with the array rather than having to be passed alongside it.

```mew
pub fn sum(values: i32[]) -> i32 {
    let mut total = 0;
    let mut index = 0;
    while index < values.count {
        total += values[index];
        index += 1;
    }

    return total;
}
```

:::note
`count` is the only member an array has. Reaching for anything else is an error.
:::

### Array Initialization with Values

```mew
let qux = new i32[4] { 0, 1, 2, 3 };
```

This line declares an array of type `i32` and initializes it with four integer values: 0, 1, 2, and 3. This is another way to create and initialize an array with specific values at the time of declaration.
