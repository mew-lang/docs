---
title: std
uid: stdlib.std
order: 1
---

## Unions

| Union                        | Description                               |
| :--------------------------- | :---------------------------------------- |
| [`Option<T>`](#optiont)      | A value that may be missing.              |
| [`Result<T, E>`](#resultt-e) | The result of an operation that can fail. |

## Interfaces

| Interface                       | Description                                      |
| :------------------------------ | :----------------------------------------------- |
| [`Display`](#display)           | Formats a value as text when it is interpolated. |
| [`Enumerable<T>`](#enumerablet) | Exposes an enumerator over a collection of `T`.  |
| [`Enumerator<T>`](#enumeratort) | Iterates over an `Enumerable<T>`.                |
| [`Into<T>`](#intot)             | Converts a value into a `T`.                     |

## Types

| Type                                        | Description                                                                                                |
| :------------------------------------------ | :--------------------------------------------------------------------------------------------------------- |
| [`ArrayEnumerable<T>`](#arrayenumerablet)   | Wraps an array as an `Enumerable<T>`.                                                                      |
| [`ArrayEnumerator<T>`](#arrayenumeratort)   | Iterates over an array.                                                                                    |
| [`FilterEnumerable<T>`](#filterenumerablet) | What `filter` returns.                                                                                     |
| [`FilterEnumerator<T>`](#filterenumeratort) | Iterates over a `FilterEnumerable<T>`.                                                                     |
| [`MapEnumerable<T, U>`](#mapenumerablet-u)  | What `map` returns.                                                                                        |
| [`MapEnumerator<T, U>`](#mapenumeratort-u)  | Iterates over a `MapEnumerable<T, U>`.                                                                     |
| [`Range`](#range)                           | The `i32` values from `start` up to, and not including, `end`. [`struct`](xref:language.attributes#struct) |

## Functions

| Function                  | Description                                                                                                                                                                                |
| :------------------------ | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `args() -> string[]`      | Returns the arguments the program was run with, without the program name.                                                                                                                  |
| `eprint(value: string)`   | Writes the text to standard error.                                                                                                                                                         |
| `eprintln(value: string)` | Writes the text to standard error, then a line break.                                                                                                                                      |
| `exit(code: i32 = 0)`     | Ends the program with the specified code. [`noreturn`](xref:language.attributes#noreturn)                                                                                                  |
| `panic(reason: string)`   | Writes the reason to standard error and ends the program with code 1. With `MEW_BACKTRACE=1` set, it also writes the calls that led to it. [`noreturn`](xref:language.attributes#noreturn) |
| `print(value: string)`    | Writes the text to standard output.                                                                                                                                                        |
| `println(value: string)`  | Writes the text to standard output, then a line break.                                                                                                                                     |

## Extensions

| Type                | Description                 |
| :------------------ | :-------------------------- |
| [`string`](#string) | What `std` adds to `string` |

## `Option<T>`

| Case      | Description |
| :-------- | :---------- |
| `none`    | No value.   |
| `some(T)` | A value.    |

| Member                                            | Description                                                               |
| :------------------------------------------------ | :------------------------------------------------------------------------ |
| `is_some() -> bool`                               | Whether there is a value.                                                 |
| `is_none() -> bool`                               | Whether there is no value.                                                |
| `unwrap() -> T`                                   | Returns the value, or panics if there is none.                            |
| `unwrap_or(fallback: T) -> T`                     | Returns the value, or `fallback` if there is none.                        |
| `or(other: Option<T>) -> Option<T>`               | Returns this option if it has a value, otherwise `other`.                 |
| `map<U>(f: fn(T) -> U) -> Option<U>`              | Applies `f` to the value, if there is one.                                |
| `and_then<U>(f: fn(T) -> Option<U>) -> Option<U>` | Calls `f` with the value, if there is one, and returns its result.        |
| `filter(f: fn(T) -> bool) -> Option<T>`           | Returns this option if `f` returns true for its value, otherwise `none`.  |
| `ok_or<E>(error: E) -> Result<T, E>`              | Converts the option into a `Result`, with `error` when there is no value. |

## `Result<T, E>`

| Case     | Description                              |
| :------- | :--------------------------------------- |
| `ok(T)`  | The operation succeeded, with its value. |
| `err(E)` | The operation failed, with its error.    |

| Member                                                  | Description                                                              |
| :------------------------------------------------------ | :----------------------------------------------------------------------- |
| `is_ok() -> bool`                                       | Whether the result is `ok`.                                              |
| `is_err() -> bool`                                      | Whether the result is `err`.                                             |
| `unwrap() -> T`                                         | Returns the value, or panics if the result is `err`.                     |
| `unwrap_or(fallback: T) -> T`                           | Returns the value, or `fallback` if the result is `err`.                 |
| `map<U>(f: fn(T) -> U) -> Result<U, E>`                 | Applies `f` to the value, if the result is `ok`.                         |
| `map_err<F>(f: fn(E) -> F) -> Result<T, F>`             | Applies `f` to the error, if the result is `err`.                        |
| `and_then<U>(f: fn(T) -> Result<U, E>) -> Result<U, E>` | Calls `f` with the value, if the result is `ok`, and returns its result. |
| `ok() -> Option<T>`                                     | Returns the value as an `Option`.                                        |
| `err() -> Option<E>`                                    | Returns the error as an `Option`.                                        |

## `Display`

| Member                | Description                          |
| :-------------------- | :----------------------------------- |
| `display() -> string` | Required. Returns the value as text. |

## `Enumerable<T>`

| Member                                          | Description                                                          |
| :---------------------------------------------- | :------------------------------------------------------------------- |
| `iter() -> Enumerator<T>`                       | Required. Returns an enumerator positioned before the first element. |
| `map<U>(apply: fn(T) -> U) -> Enumerable<U>`    | Returns a sequence that applies `apply` to each element.             |
| `filter(keep: fn(T) -> bool) -> Enumerable<T>`  | Returns a sequence of the elements that `keep` returns true for.     |
| `fold<A>(seed: A, combine: fn(A, T) -> A) -> A` | Combines the elements with `combine`, starting from `seed`.          |
| `find(keep: fn(T) -> bool) -> Option<T>`        | Returns the first element that `keep` returns true for.              |
| `any(keep: fn(T) -> bool) -> bool`              | Whether `keep` returns true for any element.                         |
| `count() -> i32`                                | Returns the number of elements.                                      |
| `to_array() -> T[]`                             | Copies the elements into an array.                                   |

## `Enumerator<T>`

| Member           | Description                                                                  |
| :--------------- | :--------------------------------------------------------------------------- |
| `next() -> bool` | Required. Moves to the next element. Returns false when there are none left. |
| `current() -> T` | Required. Returns the current element.                                       |

## `Into<T>`

| Member        | Description                                       |
| :------------ | :------------------------------------------------ |
| `into() -> T` | Required. Returns the value converted into a `T`. |

## `ArrayEnumerable<T>`

Implements `Enumerable<T>`.

| Field        | Description        |
| :----------- | :----------------- |
| `items: T[]` | The wrapped array. |

## `ArrayEnumerator<T>`

Implements `Enumerator<T>`.

| Field         | Description                                                 |
| :------------ | :---------------------------------------------------------- |
| `items: T[]`  | The array to iterate over.                                  |
| `mut at: i32` | The index of the current element, or `-1` before the first. |

## `FilterEnumerable<T>`

Implements `Enumerable<T>`.

| Field                   | Description                            |
| :---------------------- | :------------------------------------- |
| `source: Enumerable<T>` | The sequence to filter.                |
| `keep: fn(T) -> bool`   | Returns true for the elements to keep. |

## `FilterEnumerator<T>`

Implements `Enumerator<T>`.

| Field                   | Description                            |
| :---------------------- | :------------------------------------- |
| `source: Enumerator<T>` | The enumerator to filter.              |
| `keep: fn(T) -> bool`   | Returns true for the elements to keep. |

## `MapEnumerable<T, U>`

Implements `Enumerable<U>`.

| Field                   | Description                            |
| :---------------------- | :------------------------------------- |
| `source: Enumerable<T>` | The sequence to map.                   |
| `apply: fn(T) -> U`     | The function to apply to each element. |

## `MapEnumerator<T, U>`

Implements `Enumerator<U>`.

| Field                   | Description                            |
| :---------------------- | :------------------------------------- |
| `source: Enumerator<T>` | The enumerator to map.                 |
| `apply: fn(T) -> U`     | The function to apply to each element. |

## `Range`

Implements `Enumerable<i32>`.

| Field        | Description                       |
| :----------- | :-------------------------------- |
| `start: i32` | The first value.                  |
| `end: i32`   | The value the range stops before. |

| Member                         | Description                                                        |
| :----------------------------- | :----------------------------------------------------------------- |
| `contains(value: i32) -> bool` | Whether `value` is at least `start` and less than `end`.           |
| `count() -> i32`               | Returns the number of values, or `0` when `end` is before `start`. |

## `string`

| Member                                                                 | Description                                                     |
| :--------------------------------------------------------------------- | :-------------------------------------------------------------- |
| `is_empty() -> bool`                                                   | Whether the string is empty.                                    |
| `parse_i32() -> Result<i32, std.convert.ConvertError>`                 | Parses the string into an `i32`.                                |
| `length() -> i32`                                                      | Gets the length of the string, in Unicode code points.          |
| `starts_with(prefix: string) -> bool`                                  | Whether the string begins with `prefix`.                        |
| `skip(count: i32) -> string`                                           | Returns what is left after the first `count` characters.        |
| `split(separator: char) -> string[]`                                   | Splits the string at each `separator`, keeping empty parts.     |
| `lines() -> string[]`                                                  | Splits the string into lines.                                   |
| `trim_end() -> string`                                                 | Removes trailing spaces, tabs and line breaks.                  |
| `pad_left(width: i32) -> string`                                       | Pads the string with spaces on the left to `width` characters.  |
| `pad_right(width: i32) -> string`                                      | Pads the string with spaces on the right to `width` characters. |
| `string::join(separator: string, parts: ...string) -> string`          | Joins the parts with `separator` between them.                  |
| `string::join(separator: string, items: Enumerable<string>) -> string` | Joins the items with `separator` between them.                  |
