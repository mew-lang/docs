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

let builder = StringBuilder::new();
builder.Append("counted: ");
builder.Append("2");

println(builder.ToString());
```

A `use` only shortens the name. `System.Text.StringBuilder::new()` works too.

## What can be named

Only a public type declared directly in a namespace is imported. A type inside
another type is not, and neither is a generic one, so `List<T>` cannot be named.

A method is imported when its parameters and return type are types Mew can name.
The numbers, `bool`, `string`, `any` and one-dimensional arrays of those all
cross, and so does another imported type. `Func` and `Action` cross as
[function types](#passing-a-function). A 16-bit character does not: Mew's
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

A public field works the same way, and you can assign it unless the platform
made it `readonly`. Constants come along too, so you don't have to spell out
pi yourself:

```mew
use std;
use System;

println($"{MathF::PI} {Int32::MaxValue}");
println(ConsoleColor::Red.ToString());
```

```
3.1415927 2147483647
Red
```

An enum's members are constants, and `ConsoleColor::Red` has the type
`ConsoleColor`, so you can hand it to anything that takes one.

## Passing a function

A method that takes a callback takes a lambda, or the name of a function. The
platform's function types are `Func` and `Action`, and Mew reads each one as the
`fn` type of the same shape: `Action<Task>` is `fn(Task) -> void`, and
`Func<bool>` is `fn() -> bool`.

```mew
use std;
use System.Threading.Tasks;

let first = Task::Run(|| println("first"));
first.ContinueWith(|done| println($"then {done.IsCompleted}")).Wait();
```

```
first
then true
```

The lambda handed to `ContinueWith` doesn't say what `done` is. It's a `Task`,
because `ContinueWith` takes an `Action<Task>`.

You can't name `Action` as a type yourself. Write `fn() -> void` instead, which
is what the platform's `Action` is.

Only `Func` and `Action` work this way. Other delegates the platform declares,
like `WaitCallback`, stay types of their own even when they have the same shape,
so a lambda can't be passed where one is expected.

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

You create a platform type by calling its `new`. Where C# writes
`new DateTime(2026, 10, 8)`, Mew writes `DateTime::new(2026, 10, 8)`. There's a
`new` for each public constructor the type has, and the arguments you pass pick
one, the same way they pick between a method's overloads.

```mew
use std;
use System;
use System.Text;

let date = DateTime::new(2026, 10, 8);
let builder = StringBuilder::new("year ");
builder.Append($"{date.Year}");

println(builder.ToString());
```

```
year 2026
```

A struct always has a `new` that takes no arguments, even when the platform
doesn't declare one. It gives you the value with every field zero, so
`TimeSpan::new()` is a span of no time at all.

`new StringBuilder { }` doesn't compile. Braces build a Mew type out of its
fields, and the error tells you to write `StringBuilder::new()` instead. A type
with no public constructor, like `Console`, or an abstract one, like `Stream`,
has no `new` at all.
