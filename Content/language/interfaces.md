---
title: Interfaces
uid: language.interfaces
order: 7
---

An interface names a set of methods. A type implements one with an `impl`
block, and a method reaches the value it was called on through `self`. An
`impl` block that names no interface adds members instead, which
[Extending a type](xref:language.extending) covers.

```mew
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

let point = new Point { x: 32, y: 40 };
println(point.describe());
```

> [!NOTE]
> Interface members are public by definition, so `pub` is not written inside an
> `interface` block. The implementing methods are declared like any other method.

A member the interface declares and the type does not supply is an error, and so
is a method in an `impl` block that the interface never declared.

A member is a signature and nothing more, so no modifier belongs on one.

```mew error=MEW2084
pub interface Maker {
    static fn make() -> i32;
}
```

```
Error [MEW2084]: Unexpected modifier
The interface member 'make' cannot be static
```

## Members the interface supplies

An `impl` block on the interface itself gives every implementing type a member
it does not have to write. A member with a body is supplied; one without is
still required.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}
// [!code exclude-end]
impl Describable {
    pub fn shouted() -> string {
        return $"{self.describe()}!";
    }
}

println(new Point { x: 32, y: 40 }.shouted());
```

```
(32, 40)!
```

`self` inside one is the interface, so it can call the members the interface
requires and nothing else.

A type that wants its own version declares the member, and that replaces what
the interface supplied.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

pub type Circle {
    pub field radius: i32;
}

impl Describable {
    pub fn shouted() -> string {
        return $"{self.describe()}!";
    }
}
// [!code exclude-end]
impl Describable for Circle {
    pub fn describe() -> string {
        return $"a circle of {self.radius}";
    }

    pub fn shouted() -> string {
        return "ROUND";
    }
}

println(new Circle { radius: 3 }.shouted());
```

```
ROUND
```

> [!NOTE]
> An `impl` block on a generic interface covers every filling in of it, so write
> `Seq<T>` rather than `Seq<i32>`, the same rule an
> `impl ... for` block follows.

## Using a value through its interface

A type that implements an interface can be used wherever that interface is
expected, which lets one piece of code work for every type implementing it.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}
// [!code exclude-end]
pub type Circle {
    pub field radius: i32;
}

impl Describable for Circle {
    pub fn describe() -> string {
        return $"a circle of {self.radius}";
    }
}

pub fn announce(item: Describable) -> void {
    println(item.describe());
}

announce(new Point { x: 1, y: 2 });
announce(new Circle { radius: 3 });
```

The same holds for a variable, a field, a return type and an array.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}

pub type Circle {
    pub field radius: i32;
}

impl Describable for Circle {
    pub fn describe() -> string {
        return $"a circle of {self.radius}";
    }
}

pub fn announce(item: Describable) -> void {
    println(item.describe());
}
// [!code exclude-end]
let shapes = new Describable[] {
    new Point { x: 1, y: 2 },
    new Circle { radius: 3 }
};

let mut index = 0;
while index < shapes.count {
    announce(shapes[index]);
    index += 1;
}
```

Only the methods the interface declares are reachable through it. To get back to
the type, check with `is` and convert with `as`.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}
// [!code exclude-end]
let first: Describable = new Point { x: 32, y: 40 };

if first is Point {
    let point = first as Point;
    println($"{point.x}");
}
```

> [!NOTE]
> A type either implements an interface or it does not, and there is no
> inheritance to change that later. Assigning a type that does not implement one
> is an error, and a cast cannot rescue it.

## Implementing more than one

A type can implement any number of interfaces, one `impl` block each. Naming the
same interface twice for the same type is an error.

```mew
// [!code exclude-start]
use std;

pub interface Describable {
    fn describe() -> string;
}

pub type Point {
    pub field x: i32;
    pub field y: i32;
}

impl Describable for Point {
    pub fn describe() -> string {
        return $"({self.x}, {self.y})";
    }
}
// [!code exclude-end]
pub interface Countable {
    fn size() -> i32;
}

pub type Bag {
    pub field items: i32[];
}

impl Countable for Bag {
    pub fn size() -> i32 {
        return self.items.count;
    }
}

impl Describable for Bag {
    pub fn describe() -> string {
        return $"a bag of {self.size()}";
    }
}
```

## Associated types

Some interfaces need a type that each implementation chooses for itself. A
container holds items, but what the items are depends on the container. Declare
that type in the interface with `type`, and bind it in each `impl` block:

```mew
use std;

pub interface Container {
    type Item;
    fn get(index: i32) -> Item;
}

pub type Bag {
    pub field values: i32[];
}

impl Container for Bag {
    type Item = i32;

    pub fn get(index: i32) -> Item {
        return self.values[index];
    }
}

println($"{new Bag { values: new i32[] { 3, 1 } }.get(0)}");
```

```
3
```

Inside both blocks, `Item` works like any other type name. In the `impl` block
for `Bag` it is `i32`, so `get` could return `i32` just as well.

When you use the interface as the type of a value, say which item type you
mean:

```mew
// [!code exclude-start]
use std;

pub interface Container {
    type Item;
    fn get(index: i32) -> Item;
}

pub type Bag {
    pub field values: i32[];
}

impl Container for Bag {
    type Item = i32;

    pub fn get(index: i32) -> Item {
        return self.values[index];
    }
}
// [!code exclude-end]
pub fn sum_two(source: Container<Item = i32>) -> i32 {
    return source.get(0) + source.get(1);
}

println($"{sum_two(new Bag { values: new i32[] { 3, 1 } })}");
```

```
4
```

If the interface also takes type parameters, they come first:
`Parse<string, Output = Version>`.

Generic code doesn't have to name the item type at all. Constrain a type
parameter by the interface, and write `C::Item` wherever the item type goes:

```mew
// [!code exclude-start]
use std;

pub interface Container {
    type Item;
    fn get(index: i32) -> Item;
}

pub type Bag {
    pub field values: i32[];
}

impl Container for Bag {
    type Item = i32;

    pub fn get(index: i32) -> Item {
        return self.values[index];
    }
}
// [!code exclude-end]
pub fn first<C: Container>(source: C) -> C::Item {
    return source.get(0);
}

let head = first(new Bag { values: new i32[] { 3, 1 } });
println($"{head + 1}");
```

```
4
```

`first` returns an `i32` here because that is what `Bag` binds `Item` to. You
can write `Bag::Item` directly as well, and it is `i32` too.

> [!NOTE]
> An associated type can't have a constraint of its own. If a function needs one,
> give the item type a name with a type parameter and constrain that:
> `fn show<T: Display, C: Container<Item = T>>(source: C)`.

### Compared with a type parameter

You could write the interface as `Container<T>` instead, with the item type as a
type parameter. Then `Bag` could implement `Container<i32>` and
`Container<string>` at the same time, and Mew can't tell which `get` a call like
this one means:

```mew error=MEW2120
use std;

pub interface Container<T> {
    fn get(index: i32) -> T;
}

pub type Bag {
    pub field values: i32[];
}

impl Container<i32> for Bag {
    pub fn get(index: i32) -> i32 {
        return self.values[index];
    }
}

impl Container<string> for Bag {
    pub fn get(index: i32) -> string {
        return "three";
    }
}

let item = new Bag { values: new i32[] { 3 } }.get(0);
```

```
Error [MEW2120]: 'get' can produce more than one type here, and nothing says which
```

The item type also has to travel with the container. Every generic function or
type that takes a container needs a second type parameter for it, and you write
it out every time you name the type:

```mew
// [!code exclude-start]
use std;

pub interface Container<T> {
    fn get(index: i32) -> T;
}

pub type Bag {
    pub field values: i32[];
}

impl Container<i32> for Bag {
    pub fn get(index: i32) -> i32 {
        return self.values[index];
    }
}
// [!code exclude-end]
pub fn first<T, C: Container<T>>(source: C) -> T {
    return source.get(0);
}

pub type Cursor<T, C: Container<T>> {
    pub field source: C;
}

let bag = new Bag { values: new i32[] { 3 } };
let cursor = new Cursor<i32, Bag> { source: bag };
println($"{cursor.source.get(0)}");
```

With `type Item`, the same function is
`fn first<C: Container>(source: C) -> C::Item`, and the cursor is a
`Cursor<Bag>`.

A type parameter is still the right tool when one type should implement the
interface several times, the way a type can implement `Into<T>` once for each
type it converts into.
