---
title: Generics
uid: language.generics
order: 9
---

A type can leave one of the types it uses open, and have it chosen where the
type is named. The open ones are its type parameters, written in angle brackets
after the name.

```mew
pub type Box<T> {
    pub field value: T;
}
```

`T` is a type like any other inside the declaration. It names a field, a
parameter, a return type, or the element type of an array.

```mew
pub type Box<T> {
    pub field value: T;
    pub field spares: T[];

    pub fn get() -> T {
        return self.value;
    }
}
```

A type parameter is a name only inside the type that declares it. Nothing
outside can refer to `T`.

### Filling them in

Naming the type anywhere else means choosing what each parameter stands for.

```mew
// [!code exclude-start]
use std;

pub type Box<T> {
    pub field value: T;
    pub field spares: T[];

    pub fn get() -> T {
        return self.value;
    }
}
// [!code exclude-end]
let b = new Box<i32> { value: 41, spares: new i32[0], };
let n = b.get() + 1; // an i32, not a T
```

The choice travels with the type, so `b.value` is an `i32` and assigning a
string to it is an error.

A type takes exactly as many arguments as it declares parameters. Naming it
with the wrong number is an error, and so is naming it with none.

```mew error=MEW2062
pub type Box<T> {
    pub field value: T;
}

let held: Box = new Box<i32> { value: 1 };
```

```mew error=MEW2063
let value: i32<bool> = 1;
```

### More than one

Parameters are separated by commas, and each is chosen independently.

```mew
pub type Pair<A, B> {
    pub field first: A;
    pub field second: B;
}

let p = new Pair<i32, string> { first: 7, second: "seven", };
```

An argument can itself be a type with arguments, to any depth.

```mew
// [!code exclude-start]
use std;

pub type Box<T> {
    pub field value: T;
    pub field spares: T[];

    pub fn get() -> T {
        return self.value;
    }
}
// [!code exclude-end]
let n = new Box<Box<i32>> { value: new Box<i32> { value: 3, spares: new i32[0] }, spares: new Box<i32>[0] };
println($"{n.value.value}"); // 3
```

A type may also name itself, which is what a list or a tree needs.

```mew
pub type Node<T> {
    pub field value: T;
    pub field next: Node<T>;
}
```

### Interfaces

An interface takes parameters the same way.

```mew
pub interface Holder<T> {
    fn held() -> T;
}
```

An `impl` block can fill them in with a type, which is what a non-generic type
implementing a generic interface looks like.

```mew
// [!code exclude-start]
pub interface Holder<T> {
    fn held() -> T;
}
// [!code exclude-end]
pub type Counter {
    pub field total: i32;
}

impl Holder<i32> for Counter {
    pub fn held() -> i32 {
        return self.total;
    }
}
```

It can also fill them in with the parameters of the type it is implementing for.
Write those after the target, matching what the type declares.

```mew
// [!code exclude-start]
use std;

pub type Box<T> {
    pub field value: T;
}

pub interface Holder<T> {
    fn held() -> T;
}
// [!code exclude-end]
impl Holder<T> for Box<T> {
    pub fn held() -> T {
        return self.value;
    }
}
```

The target may be written bare when nothing needs to name its parameters. These
two say the same thing:

```mew ignore
impl Describable for Box { }
impl Describable for Box<T> { }
```

Either way the block covers every filling in of the type at once, so a value can
be used through the interface with its arguments chosen.

```mew
// [!code exclude-start]
use std;

pub type Box<T> {
    pub field value: T;
}

pub interface Holder<T> {
    fn held() -> T;
}

impl Holder<T> for Box<T> {
    pub fn held() -> T {
        return self.value;
    }
}
// [!code exclude-end]
pub fn read(holder: Holder<i32>) -> i32 {
    return holder.held();
}

println($"{read(new Box<i32> { value: 5, })}");
```

### Constraints

A parameter with nothing said about it can only be stored, passed and handed
back, because nothing is known about what it can do. A constraint says it
implements an interface, and everything that interface declares becomes
available on a value of that type.

```mew
pub interface Describable {
    fn describe() -> string;
}

pub type Box<T: Describable> {
    pub field value: T;

    pub fn show() -> string {
        return self.value.describe();
    }
}
```

Filling the parameter in with a type that does not implement the interface is an
error, reported where the type is named rather than inside the declaration.

```mew error=MEW2065
pub interface Describable {
    fn describe() -> string;
}

pub type Box<T: Describable> {
    pub field value: T;
}

let held = new Box<i32> { value: 1 };
```

Only an interface can be a constraint. Naming a type is an error, since a type
has no implementers.

A constraint may name the parameters it constrains, which is how an interface
says something about the type implementing it.

```mew
pub interface Comparable<T> {
    fn compare_to(other: T) -> i32;
}

pub type Smallest<T: Comparable<T>> {
    pub mut field current: T;

    pub fn add(item: T) -> void {
        if item.compare_to(self.current) < 0 {
            self.current = item;
        }
    }
}
```

`Comparable<T>` is filled in along with the parameter, so `Smallest<Score>`
requires `Score` to implement `Comparable<Score>` rather than `Comparable<T>`.

```mew
// [!code exclude-start]
use std;

pub interface Comparable<T> {
    fn compare_to(other: T) -> i32;
}

pub type Score {
    pub field points: i32;
}
// [!code exclude-end]
impl Comparable<Score> for Score {
    pub fn compare_to(other: Score) -> i32 {
        return self.points - other.points;
    }
}
```

### Functions

A function takes type parameters the same way, and they are worked out from the
arguments rather than written at the call.

```mew
pub fn first<T>(items: T[]) -> T {
    return items[0];
}

let number = first(new int[] { 3, 1, 2, });   // an i32
let word = first(new string[] { "a", "b", }); // a string
```

A parameter that appears nowhere in the argument types cannot be worked out.
Name it at the call instead.

```mew error=MEW2066
pub fn empty<T>() -> T[] {
    return new T[0];
}

let values = empty();
```

Naming it at the call is what says which type was meant.

```mew
use std;

pub fn empty<T>() -> T[] {
    return new T[0];
}

let values = empty<i32>();

println($"{values.count}");
```

Type arguments are written the same way to reach a static method through the
type that declares it.

```mew ignore
let smallest = Smallest<i32>::new(41);
```

`<` still compares two values everywhere else. It opens a type argument list
only when a matching `>` is followed by a call or a `::`, so `f(a < b, c > d)`
stays the two comparisons it looks like.

A method may declare its own, separately from the type's. They may even share a
name, in which case the method's wins for as long as it lasts.

```mew
pub type Box<T> {
    pub field value: T;

    pub fn map<U>(other: U) -> U {
        return other;
    }
}
```

Constraints work as they do on a type.

```mew error=MEW2065
pub interface Describable {
    fn describe() -> string;
}

pub fn show<T: Describable>(value: T) -> string {
    return value.describe();
}

let text = show(42);
```

### One filling in at a time

`impl Describable for Box<i32>` is an error. There is no way to give one filling
in of a type behaviour that the others do not have.

This is not only a missing feature. A generic type is emitted once, with the
parameters left open, and its interfaces are fixed where it is declared. Giving
`Box<i32>` an interface that `Box<string>` lacks would mean emitting a separate
type per filling in, and answering what happens when a general `impl` and a
specialized one both apply. What that example usually wants is a
[constraint](#constraints).

### Not yet

Generics do not carry variance, defaults, or more than one constraint per
parameter. A [union](xref:language.unions) takes type parameters the same way a type
does.
