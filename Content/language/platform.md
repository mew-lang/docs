---
title: Platform
uid: language.platform
order: 24
---

A Mew program is built into a .NET assembly, and it can name what that platform
declares. A `use` of one of its namespaces brings in the types there, the same
way a `use` of a Mew namespace does.

```mew
use std;
use System.Text;

let builder = new StringBuilder { };
builder.Append("counted: ");
builder.Append("2");

println(builder.ToString());
```

A `use` only shortens the name. `new System.Text.StringBuilder { }` works too.

## What can be named

Only a public type declared directly in a namespace is imported. A type inside
another type is not, and neither is a generic one, so `List<T>` cannot be named.

A method is imported when its parameters and return type are types Mew can name.
The numbers, `bool`, `string`, `any` and one-dimensional arrays of those all
cross, and so does another imported type. A 16-bit character does not: Mew's
`char` is a whole code point and the platform's is half of one, so a method
taking or returning one cannot be reached. Neither can a generic method, a
pointer, or a parameter passed by reference other than the one below.

An instance method is called through a value with `.`, and a static one through
the type with `::`.

```mew
use std;
use System;

if String::IsNullOrEmpty("") {
    println("empty");
}
```

A property with a public getter reads like a field, and one with a setter can be
assigned. It keeps the name the platform gave it.

## Writing into an argument

The platform may declare a parameter the call writes into. An argument filling
one is written `out`, and has to name a variable the call can write to.

```mew
use std;
use std.convert;
use System;

let mut value = 0;
if Int32::TryParse("42", out value) {
    println(itoa(value));
}
```

The variable is handed over by address, so its type has to be exactly the
parameter's, nothing is converted on the way in or out, and it has to be `mut`.
Leaving `out` off where the call writes is an error, and writing it where the
call only reads is another.

What the variable holds afterwards is whatever the call wrote, including when
the call reports that it failed.

`out` is not a reserved word. It is read as one only directly before the name or
literal it applies to, so a variable called `out` is still a variable.

## Constructing one

`new T { }` calls the constructor that takes no arguments. There is no way to
pass one, so a type without such a constructor cannot be built this way. An
imported type has no fields to initialize, so the braces are always empty.
