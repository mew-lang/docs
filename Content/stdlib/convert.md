---
title: std.convert
uid: stdlib.convert
order: 7
---

## Unions

| Union                           | Description                                      |
| :------------------------------ | :----------------------------------------------- |
| [`ConvertError`](#converterror) | Why a string could not be converted to a number. |

## Functions

| Function                                           | Description                      |
| :------------------------------------------------- | :------------------------------- |
| `atoi(value: string) -> Result<i32, ConvertError>` | Converts the string to an `i32`. |
| `itoa(value: i32) -> string`                       | Converts the number to a string. |

## `ConvertError`

| Case       | Description                                  |
| :--------- | :------------------------------------------- |
| `invalid`  | The string is not a number.                  |
| `overflow` | The number is outside the range of an `i32`. |

| Member                 | Description                         |
| :--------------------- | :---------------------------------- |
| `describe() -> string` | Returns a description of the error. |
