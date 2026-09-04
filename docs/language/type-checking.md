---
sidebar_position: 55
---

# Type Checking

`is` asks what a value is, and produces a `bool`.

```mew
let number: any = 32;
println($"{number is i32}");
println($"{number is string}");
```

```
true
false
```

It reads a [type](./types.md) you declare, a [union](./unions.md), and an
[interface](./interfaces.md) the value's type implements.

```mew
let boxed: any = new Point { x: 32, y: 40 };

println($"{boxed is Point}");
println($"{boxed is Describable}");
println($"{boxed is Circle}");
```

```
true
true
false
```

Asking about something the compiler already knows is a warning, since the
answer cannot be anything else.

```mew
let text = "hello";
let known = text is string;
```

```
Warning [MEW2041]: The given expression is always of the provided ('string') type
```

`is` is how a [cast](./type-casting.md) is made safe, because a cast that is
wrong ends the program.

```mew
if boxed is Circle {
    println($"{(boxed as Circle).radius}");
}
```

:::note
A union case is not a type, so `is` cannot ask which case a value is.
[`match`](./unions.md#reading-a-value) is what asks that.
:::
