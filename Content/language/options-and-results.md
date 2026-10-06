---
title: Options and Results
uid: language.options-and-results
order: 17
---

An `Option<T>` holds a value or nothing. A `Result<T, E>` holds a value or an
error. Both are [unions](xref:language.unions) from the
[standard library](xref:stdlib.std):

```mew ignore
pub union Option<T> {
    none,
    some(T),
}

pub union Result<T, E> {
    ok(T),
    err(E),
}
```

Use them instead of returning `null` or a placeholder like `-1`.

## Making one

In a `return`, you can leave out the union's name, because the function's
return type already says which union it is:

```mew
pub fn find_user(id: i32) -> Option<string> {
    if id == 1 {
        return .some("ada");
    }

    return .none;
}

pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
```

Elsewhere, write the type on the variable or on the case:

```mew
let held: Option<i32> = .some(3);
let failed = Result<i32, string>::err("no such file");
```

## Reading one

Use `match` to handle both cases:

```mew
// [!code exclude-start]
pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
// [!code exclude-end]
match divide(10, 2) {
    .ok(value) => {
        println($"{value}");
    },
    .err(reason) => {
        println(reason);
    },
}
```

```
5
```

If you only care about one case, use `if let`. The name it binds only exists
inside the block:

```mew
// [!code exclude-start]
pub fn find_user(id: i32) -> Option<string> {
    if id == 1 {
        return .some("ada");
    }

    return .none;
}
// [!code exclude-end]
if let .some(name) = find_user(1) {
    println($"found {name}");
}
```

```
found ada
```

## Getting the value out

You don't always need a `match`. Both unions have methods for the common cases,
and the [standard library](xref:stdlib.std#optiont) lists all of them.

`unwrap_or` returns the value, or a fallback if there isn't one:

```mew
// [!code exclude-start]
pub fn find_user(id: i32) -> Option<string> {
    if id == 1 {
        return .some("ada");
    }

    return .none;
}

pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
// [!code exclude-end]
println(find_user(2).unwrap_or("nobody"));
println($"{divide(10, 0).unwrap_or(0)}");
```

```
nobody
0
```

`unwrap` returns the value and [panics](xref:stdlib.std#functions) if there
isn't one. Use it where a missing value means a bug in your program:

```mew
// [!code exclude-start]
pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
// [!code exclude-end]
println($"{divide(10, 2).unwrap()}");
println($"{divide(10, 0).unwrap()}");
```

```
5
Unhandled error: unwrapped a result that failed
```

The message doesn't include the error, since `E` could be any type. Print
`err()` yourself if you need it.

## Changing the value

`map` runs a function on the value, if there is one, and leaves a `none` or an
error alone. `and_then` takes a function that can fail too, which lets you
chain steps:

```mew
// [!code exclude-start]
pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
// [!code exclude-end]
println($"{divide(10, 2).map(|n| n * 3).unwrap_or(0)}");
println($"{divide(100, 5).and_then(|n| divide(n, 2)).unwrap_or(0)}");
println($"{divide(100, 0).and_then(|n| divide(n, 2)).unwrap_or(-1)}");
```

```
15
10
-1
```

`map_err` runs a function on the error instead. `ok_or` turns an `Option` into
a `Result`, with the error to use when there is no value:

```mew
// [!code exclude-start]
pub fn find_user(id: i32) -> Option<string> {
    if id == 1 {
        return .some("ada");
    }

    return .none;
}

pub fn divide(left: i32, right: i32) -> Result<i32, string> {
    if right == 0 {
        return .err("cannot divide by zero");
    }

    return .ok(left / right);
}
// [!code exclude-end]
let checked = divide(1, 0).map_err(|reason| $"checking failed: {reason}");
println(checked.err().unwrap_or(""));

let user = find_user(2).ok_or("no such user");
println(user.err().unwrap_or(""));
```

```
checking failed: cannot divide by zero
no such user
```

See [Error Propagation](xref:language.error-propagation) for passing a `none`
or an error on to the caller with `?`.
