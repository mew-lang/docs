---
sidebar_position: 46
---

# Unions

A union names a closed set of alternatives, and a value of one is always exactly
one of them. Each alternative is a case, and a case either carries nothing or
carries a fixed list of values.

```mew
pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
```

Case names are lowercase, the way function and field names are.

A union declaration takes `pub` and nothing else. Without it the union belongs
to the file that declares it, which is narrower than its namespace, the same
rule a [type](./types.md) follows.

:::note
A union can only be declared at the top level of a file, and it cannot be
declared inside a function or inside a type.
:::

## Building a value

A case is reached through the union with `::`. One that carries values is
written like a call.

```mew
let nothing = IpAddress::none;
let four = IpAddress::v4(192, 168, 1, 1);
let six = IpAddress::v6("fd1a::1");
```

The values a case carries are checked and converted the way a call's arguments
are, so the `192` above is read as a `u8`.

:::note
`new` does not apply to a union. There are no fields to initialize, and a value
comes from one of the cases instead.
:::

A case belongs to the union rather than to a value, so it is never reached
through one.

```mew
let ip = IpAddress::none;
let wrong = ip.none;  // Error: 'IpAddress::none' is reached through the union
```

### Leaving the union out

Where the union is already known, the case can be written with just a `.` in
front, the same way a [pattern](#reading-a-value) is.

```mew
pub fn loopback() -> IpAddress {
    return .v4(127, 0, 0, 1);
}
```

The union comes from wherever the value is going, so this works in every place
that already knows what it wants: a `return`, a `let` with a type, an argument,
a field, an array element, an assignment, and the value a `match` produces.

```mew
let nothing: IpAddress = .none;
let addresses = new IpAddress[] { .none, .v6("fd1a::1") };
let holder = new Route { address: .v4(10, 0, 0, 1) };

let mut current: IpAddress = .none;
current = .v6("fd1a::1");

pub fn cleared(ip: IpAddress) -> IpAddress {
    return match ip {
        .none => .v4(0, 0, 0, 0),
        _ => .none,
    };
}
```

Where nothing says which union is wanted, there is nothing to work it out
from.

```mew
let ip = .none;
```

```
Error [MEW2087]: No union to infer
Nothing here expects a union, so there is no union for 'none' to be a case of
```

:::note
A `let` needs its type written for this, because the initializer is the only
thing that could say what the variable holds and `.none` does not say. Write
`IpAddress::none`, or annotate the `let`.
:::

When a function is overloaded, the case name is what picks between two
candidates taking different unions. Two candidates whose unions both have the
case is ambiguous, and writing the union out is how to say which one.

```mew
pub fn announce(ip: IpAddress) -> void { }
pub fn announce(visible: Visibility) -> void { }

announce(.v6("fd1a::1"));  // the IpAddress one, since only it has 'v6'
```

## Reading a value

`match` is how a value says which case it is. In statement form each arm holds
a block.

```mew
use std;

pub fn show(ip: IpAddress) -> void {
    match ip {
        .none => {
            println("no address");
        },
        .v4(a, b, c, d) => {
            println($"{a}.{b}.{c}.{d}");
        },
        .v6(text) => {
            println(text);
        },
    }
}
```

```
no address
192.168.1.1
fd1a::1
```

A pattern is `.` followed by a case name. A case that carries values names them,
one name per value, and each name is a new immutable local that only exists
inside its own arm.

`break`, `continue` and `return` inside an arm mean what they mean anywhere
else, so a match inside a loop can leave the loop.

### Discarding what an arm does not read

A binding written `_` names nothing. A case still has to be given one binding
per value it carries, so `_` is how an arm says it matched on the case without
reading what came with it.

```mew
use std;

pub fn is_v4(ip: IpAddress) -> bool {
    return match ip {
        .none => false,
        .v4(_, _, _, _) => true,
        .v6(_) => false,
    };
}
```

`_` can be repeated in one pattern, which an ordinary name cannot, and it can
sit beside names that are read.

```mew
use std;

pub fn first(ip: IpAddress) -> i32 {
    return match ip {
        .none => -1,
        .v4(a, _, _, _) => a,
        .v6(_) => -1,
    };
}
```

It is a discard rather than a name, so nothing can be read back out of it.

```mew
match ip {
    .v6(_) => {
        println(_);
    },
}
```

```
Error [MEW2009]: Undeclared variable
Undeclared variable '_'
```

An arm written `_` on its own is a different thing: that is the
[catch-all pattern](#every-case-has-to-be-covered), which matches any case.

### Every case has to be covered

A union is closed, so the compiler knows every case and says which one an arm is
missing.

```mew
match ip {
    .none => { },
}
```

```
Error [MEW2081]: Not exhaustive
This match on 'IpAddress' does not handle 'v4' or 'v6'
```

`_` covers every case no arm above it named.

```mew
match ip {
    .none => {
        println("no address");
    },
    _ => {
        println("has an address");
    },
}
```

An arm written after `_` can never run, and the compiler says so.

:::note
A case added to a union later lands in an existing `_` without a word from the
compiler. Naming each case instead is what makes the compiler point at every
match that has not thought about the new one.
:::

## Matching as a value

The same match reads as a value when every arm is an expression.

```mew
pub fn is_v4(ip: IpAddress) -> bool {
    return match ip {
        .none => false,
        .v4(a, b, c, d) => true,
        .v6(a) => false,
    };
}
```

Every arm has to produce the same type, and the first arm that produces one
decides what the match is. A block arm is an error here, because Mew has no
block that produces a value.

## Adding members

An `impl` block gives a union methods, exactly as it does for a type. `self` is
the value the method was called on, and `match self` reads it.

```mew
impl IpAddress {
    pub fn describe() -> string {
        return match self {
            .none => "no address",
            .v4(a, b, c, d) => $"{a}.{b}.{c}.{d}",
            .v6(text) => text,
        };
    }

    pub static fn nothing() -> IpAddress {
        return IpAddress::none;
    }
}
```

```mew
// Usage:
println(IpAddress::v4(10, 0, 0, 1).describe());
println(IpAddress::nothing().describe());
```

```
10.0.0.1
no address
```

A union implements an [interface](./interfaces.md) the same way a type does, and
can then be used wherever that interface is expected.

```mew
pub interface Describable {
    fn describe() -> string;
}

impl Describable for IpAddress {
    pub fn describe() -> string {
        return match self {
            .none => "no address",
            .v4(a, b, c, d) => $"{a}.{b}.{c}.{d}",
            .v6(text) => text,
        };
    }
}

pub fn announce(item: Describable) -> void {
    println(item.describe());
}
```

## Type parameters

A union takes [type parameters](./generics.md) the same way a type does, and a
case's values can name them.

```mew
pub union Result<T, E> {
    ok(T),
    err(E),
}
```

```mew
// Usage:
let read = Result<i32, string>::ok(41);
let failed = Result<i32, string>::err("no such file");
```

Each filling in is its own type, so a `Result<i32, string>` is not a
`Result<string, string>`, and a pattern's names are the types the arguments
supplied.

```mew
pub fn value(read: Result<i32, string>, fallback: i32) -> i32 {
    return match read {
        .ok(number) => number,
        .err(reason) => fallback,
    };
}
```

## A union is never null

`null` can be assigned to a `string`, an array, `any`, and a type you declare. A
union refuses it, because a value is always one of its cases and a match should
not be able to fail.

```mew
let ip: IpAddress = null;  // Error: 'IpAddress' is a union, so it can never be null
```

:::note
An array with a size and no initializer is the one way around this, because its
elements start out as `null` like any other array of a declared type. A `match`
on one of them fails at run time. Give the array an initializer, or fill it
before reading it.
:::

## Not yet

A pattern is the only way to read what a case carries. There is no form that
tests one case and binds its values without a match.

`match` works on a union and on nothing else, so matching a number against
literals is not something it does.

An arm names one case. There is no way to write one arm for several, and no way
to give a case's values names in the declaration and match on those names.

`is` and `as` do not reach a case. A case is not a type, so the only thing to
ask about a value is which case it is, and `match` is what asks.
