---
title: Error Propagation
uid: language.error-propagation
order: 18
---

`?` passes an error on to the caller. Put it after a call that returns a
[`Result`](xref:language.options-and-results). If the call succeeded, you get
its value. If it failed, your function returns the error right there:

```mew
pub fn double(text: string) -> Result<i32, std.convert.ConvertError> {
    let n = text.parse_i32()?;
    return .ok(n * 2);
}

println($"{double("21").unwrap()}");
println(double("abc").err().unwrap().describe());
```

```
42
not a number
```

Without `?`, the same function needs a `match`:

```mew
pub fn double(text: string) -> Result<i32, std.convert.ConvertError> {
    match text.parse_i32() {
        .ok(n) => {
            return .ok(n * 2);
        },
        .err(reason) => {
            return .err(reason);
        },
    }
}
```

On an `Option`, `?` works the same way, with `none` in place of the error:

```mew
pub fn first_long_length(words: string[]) -> Option<i32> {
    let word = words.find(|w| w.length() > 3)?;
    return .some(word.length());
}

println($"{first_long_length(new string[] { "a", "mew", "kitten" })}");
println($"{first_long_length(new string[] { "a", "mew" })}");
```

```
some(6)
none
```

## Steps that return nothing

A function that can fail but has nothing to return gives back a
`Result<(), E>`. `()` is the unit type, and its one value is also written
`()`. Put `?` after the call on a line of its own, and the rest of your
function only runs if the call succeeded:

```mew
pub fn check(name: string) -> Result<(), string> {
    if name.is_empty() {
        return .err("a name is needed");
    }

    return .ok(());
}

pub fn greet(name: string) -> Result<(), string> {
    check(name)?;
    println($"Hello, {name}!");
    return .ok(());
}

greet("ada").unwrap();
println(greet("").err().unwrap_or(""));
```

```
Hello, ada!
a name is needed
```

## Where `?` can go

`?` returns from the function it's in, so that function has to return the same
kind of union: a `Result` for a `Result`, and an `Option` for an `Option`.
Anywhere else, including the top level of `main.mew`, it's an error:

```mew error=MEW2122
pub fn report(text: string) {
    let n = text.parse_i32()?;
    println($"{n}");
}
```

To use `?` on an `Option` in a function that returns a `Result`, turn it into a
`Result` first with `ok_or`:

```mew
pub fn first_even(values: i32[]) -> Result<i32, string> {
    let found = values.find(|n| n % 2 == 0).ok_or("no even number")?;
    return .ok(found);
}

println($"{first_even(new i32[] { 3, 4 }).unwrap()}");
println(first_even(new i32[] { 3 }).err().unwrap_or(""));
```

```
4
no even number
```

## When the error types differ

If the error has your function's error type, or converts to it, `?` passes it
on unchanged. An error converts to an [interface](xref:language.interfaces) it
implements, for example.

Otherwise, implement `Into` for the error, and `?` converts it on the way out:

```mew
pub type DiskError {
    pub field code: i32;
}

pub type AppError {
    pub field message: string;
}

impl Into<AppError> for DiskError {
    pub fn into() -> AppError {
        return new AppError { message: $"disk error {self.code}" };
    }
}

pub fn read(code: i32) -> Result<i32, DiskError> {
    if code > 0 {
        return .err(new DiskError { code: code });
    }

    return .ok(40);
}

pub fn load(code: i32) -> Result<i32, AppError> {
    return .ok(read(code)? + 2);
}

println($"{load(0).unwrap()}");
println(load(7).err().unwrap().message);
```

```
42
disk error 7
```

`?` binds as tightly as a method call. In `.ok(read(code)? + 2)`, it applies to
`read(code)`, and the `+ 2` only runs when there is a value.

You can also convert the error yourself with `map_err` before the `?`:

```mew
pub fn read(text: string) -> Result<i32, string> {
    let n = text.parse_i32().map_err(|error| error.describe())?;
    return .ok(n * 2);
}

println(read("abc").err().unwrap_or(""));
```

```
not a number
```

If none of these apply, the code doesn't compile:

```mew error=MEW2123
pub fn read(text: string) -> Result<i32, string> {
    let n = text.parse_i32()?;
    return .ok(n);
}
```
