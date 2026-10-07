---
title: Unions
uid: language.unions
order: 8
---

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
rule a [type](xref:language.types) follows.

> [!NOTE]
> A union can only be declared at the top level of a file, and it cannot be
> declared inside a function or inside a type.

## Building a value

A case is reached through the union with `::`. One that carries values is
written like a call.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
let nothing = IpAddress::none;
let four = IpAddress::v4(192, 168, 1, 1);
let six = IpAddress::v6("fd1a::1");
```

The values a case carries are checked and converted the way a call's arguments
are, so the `192` above is read as a `u8`.

> [!NOTE]
> `new` does not apply to a union. There are no fields to initialize, and a value
> comes from one of the cases instead.

A case belongs to the union rather than to a value, so it is never reached
through one.

```mew error=MEW2076
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

let ip = IpAddress::none;
let wrong = ip.none;
```

### Leaving the union out

Where the union is already known, the case can be written with just a `.` in
front, the same way a [pattern](#reading-a-value) is.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
pub fn loopback() -> IpAddress {
    return .v4(127, 0, 0, 1);
}
```

The union comes from wherever the value is going, so this works in every place
that already knows what it wants: a `return`, a `let` with a type, an argument,
a field, an array element, an assignment, and the value a `match` produces.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
pub type Route {
    pub field address: IpAddress;
}

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

Where no union is expected, the union cannot be worked out.

```mew error=MEW2087
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

let ip = .none;
```

```
Error [MEW2087]: No union to infer
Nothing here expects a union, so there is no union for 'none' to be a case of
```

> [!NOTE]
> A `let` needs its type written for this, because the initializer is the only
> thing that could say what the variable holds and `.none` does not say. Write
> `IpAddress::none`, or annotate the `let`.

When a function is overloaded, the case name picks between two candidates
taking different unions. Two candidates whose unions both have the case make the
call ambiguous, and writing the union out is how to say which one.

```mew
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

pub union Visibility {
    hidden,
    shown,
}

pub fn announce(ip: IpAddress) -> void { }
pub fn announce(visible: Visibility) -> void { }

announce(.v6("fd1a::1"));  // the IpAddress one, since only it has 'v6'
```

## Reading a value

`match` is how a value says which case it is. In statement form each arm holds
a block.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
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
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
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
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
pub fn first(ip: IpAddress) -> i32 {
    return match ip {
        .none => -1,
        .v4(a, _, _, _) => a,
        .v6(_) => -1,
    };
}
```

`_` is a discard, so nothing can be read back out of it.

```mew error=MEW2009
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

let ip = IpAddress::none;

match ip {
    .v6(_) => {
        println(_);
    },
    _ => { },
}
```

```
Error [MEW2009]: Undeclared variable
Undeclared variable '_'
```

An arm written `_` on its own is the
[catch-all pattern](#every-case-has-to-be-covered), which matches any case.

### Every case has to be covered

A union is closed, so the compiler knows every case and says which one an arm is
missing.

```mew error=MEW2081
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

let ip = IpAddress::none;

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
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
let ip = IpAddress::none;

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

> [!NOTE]
> A case added to a union later lands in an existing `_` without a word from the
> compiler. Naming each case instead makes the compiler point at every match
> that does not handle the new one.

## Matching as a value

The same match reads as a value when every arm is an expression.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
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

## Testing one case

`if let` tests a value against one case, where a `match` would have to name
every case. It stands where the condition of an
[`if`](xref:language.control.conditions) would, and takes the same `else`.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
pub fn show(ip: IpAddress) -> void {
    if let .v4(a, b, c, d) = ip {
        println($"{a}.{b}.{c}.{d}");
    } else if let .v6(text) = ip {
        println(text);
    } else {
        println("no address");
    }
}
```

Each name in the pattern is a new immutable local that exists only inside the
block that follows, so the `else` branch cannot read it. A case that carries
nothing is still a test. `if let .none = ip` runs the block when `ip` is `none`.

## Adding members

An `impl` block gives a union methods, exactly as it does for a type. `self` is
the value the method was called on, and `match self` reads it.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
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
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

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
// [!code exclude-end]
println(IpAddress::v4(10, 0, 0, 1).describe());
println(IpAddress::nothing().describe());
```

```
10.0.0.1
no address
```

A union implements an [interface](xref:language.interfaces) the same way a type does, and
can then be used wherever that interface is expected.

```mew
// [!code exclude-start]
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
// [!code exclude-end]
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

A union takes [type parameters](xref:language.generics) the same way a type does, and a
case's values can name them.

```mew ignore
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
use std;

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

```mew error=MEW2073
use std;

pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}

let ip: IpAddress = null;
```

> [!NOTE]
> An array with a size and no initializer is the one way around this, because its
> elements start out as `null` like any other array of a declared type. Reading
> one before writing it is
> [undefined](xref:language.undefined#reading-an-array-element-that-was-never-written).
> Give the array an initializer, or fill it before reading it.

## Not yet

A pattern is the only way to read what a case carries, in a `match` or an
[`if let`](#testing-one-case). There is no range pattern, no way to name a value
and still look inside it, and no way to give a case's values names in the
declaration and match on those names.

`is` and `as` do not reach a case. A case is not a type, so the only thing to
ask about a value is which case it is, and `match` asks that.
