---
title: Types
uid: language.types
order: 6
---

A `type` declaration introduces a named type with fields and methods. There is no
inheritance and no subtyping: two declared types are unrelated however similar
their fields look, and the only relation a type has to anything else is that it
[implements an interface](xref:language.interfaces).

```mew
use std;

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

let origin = new Point { x: 0, y: 0 };

println($"{origin.x}, {origin.y}");
```

`pub` makes the type visible to other files. Without it the type belongs to the
file that declares it, and that is narrower than its
[namespace](xref:language.namespaces#visibility): two files sharing a `namespace`
still cannot see each other's private types.

```mew
pub type Shared { }

type Internal { }
```

`pub` is the only modifier a type takes.

> [!NOTE]
> `static` and `external` are not modifiers a type accepts. `static` belongs on a
> method, and `external` on a [function](xref:language.ffi).

A type is declared at the top level of a file. There are no types inside functions
and none inside other types.

## Fields

A field holds one value per value of the type.

```mew
use std;

pub type Person {
    pub field name: string;
    pub field age: i32;
}

let person = new Person { name: "Ada", age: 36 };

println($"{person.name} is {person.age}");
```

There are no field initializers in the declaration and no default values, so every
field is given a value where the value is created. Leaving one out is an error that
names it.

```mew error=MEW2033
pub type Person {
    pub field name: string;
    pub field age: i32;
}

let person = new Person { name: "Ada" };
```

So is naming one twice, or naming something the type does not have. Initializers
run in the order they are written.

A field cannot be `static`. A type holds fields for each of its values, and there
is nowhere for a shared one to live.

A field without `pub` is reachable only from inside its type.

### Mutability

A field is immutable once the value is created, the same way a
[`let`](xref:language.assignment#mutability) is. `mut` makes it assignable.

```mew
use std;

pub type Counter {
    pub mut field total: i32;
    pub field name: string;
}

let counter = new Counter { total: 0, name: "hits" };
counter.total = 1;

println($"{counter.name}: {counter.total}");
```

```mew error=MEW2054
pub type Counter {
    pub field name: string;
}

let counter = new Counter { name: "hits" };
counter.name = "no";
```

That holds inside the type as well. A method that writes a field needs that field
to be `mut`; being on the inside is not a licence.

```mew
use std;

pub type Counter {
    pub mut field total: i32;

    pub fn add(amount: i32) -> void {
        self.total = self.total + amount;
    }
}

let counter = new Counter { total: 0 };
counter.add(2);

println($"{counter.total}");
```

## Methods

A function declared inside a type body is a method, called through a value with
`.`.

```mew
use std;

pub type Counter {
    pub field value: i32;

    pub fn next() -> i32 {
        return self.value + 1;
    }
}

println($"{new Counter { value: 41 }.next()}");
```

A method without `pub` is reachable only from inside its type. Visibility on a
member is scoped to the type rather than to the file, which is why a private
method is private even to another file in the same namespace.

### self

`self` names the value the method was called on. Inside a method a field is read
by name, so `self` is needed only when something else has taken that name.

```mew
use std;

pub type Counter {
    pub field value: i32;

    pub fn plus(value: i32) -> i32 {
        return self.value + value;
    }
}

println($"{new Counter { value: 41 }.plus(1)}");
```

Here `value` is the parameter and `self.value` is the field. Without a parameter
of that name, the two mean the same thing.

`self` is a keyword, so it cannot be used as a name. It is an error outside a type,
and in a `static fn`, which has no value to refer to.

### Static methods

`static` makes a member belong to the type rather than to a value. It has no
`self`, and it is called through the type name with `::`.

```mew
use std;

pub type Counter {
    pub field value: i32;

    pub static fn zero() -> Counter {
        return new Counter { value: 0 };
    }
}

println($"{Counter::zero().value}");
```

Calling a static member through a value is an error, and so is calling an instance
member through the type name. The two are reached differently on purpose.

## Constructors

Mew has no constructors. A value is created with `new`, and a type that wants to
decide how gives itself a static method, by convention called `new`, which works
because `new` is only a keyword at the start of an expression.

```mew
use std;

pub type Person {
    pub field name: string;
    pub field age: i32;

    pub static fn new(name: string) -> Person {
        return new Person { name: name, age: 0 };
    }
}

println(Person::new("Ada").name);
```

That is also how a type keeps a field private and still lets one be set: the
static method is inside the type, so it can reach what a caller cannot.

## Null

A declared type can hold [`null`](xref:language.primitives.null), unlike a
[union](xref:language.unions), which never can.

```mew
use std;

pub type Point {
    pub field x: i32;
}

let missing: Point = null;
let present = new Point { x: 1 };

println($"{present.x}");
```

There is no way to ask whether a value is `null`, so a type that may be absent is
usually better expressed as an [`Option<T>`](xref:stdlib#optiont-and-resultt-e).
