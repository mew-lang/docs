---
title: std.collections
uid: stdlib.collections
order: 6
---

## Types

| Type                | Description                           |
| :------------------ | :------------------------------------ |
| [`List<T>`](#listt) | A list that grows as items are added. |

## `List<T>`

Implements `Enumerable<T>` and `Display`.

| Member                      | Description                                                          |
| :-------------------------- | :------------------------------------------------------------------- |
| `List<T>::new() -> List<T>` | Creates an empty list.                                               |
| `add(item: T)`              | Adds the item to the end of the list.                                |
| `count() -> i32`            | Returns the number of items.                                         |
| `get(index: i32) -> T`      | Returns the item at the index. Panics if the index is out of range.  |
| `set(index: i32, item: T)`  | Replaces the item at the index. Panics if the index is out of range. |
| `clear()`                   | Removes all items.                                                   |
| `to_array() -> T[]`         | Copies the items into an array.                                      |
