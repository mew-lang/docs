---
sidebar_position: 47
---

# Generics

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
let b = new Box<i32> { value: 41, spares: new i32[0], };
let n = b.get() + 1; // an i32, not a T
```

The choice travels with the type, so `b.value` is an `i32` and assigning a
string to it is an error.

A type takes exactly as many arguments as it declares parameters. Naming it
with the wrong number is an error, and so is naming it with none.

```mew
let a: Box = b;            // Error: 'Box' takes 1 type argument, but 0 were given
let c: Box<i32, i32> = d;  // Error: 'Box' takes 1 type argument, but 2 were given
let e: i32<bool> = 1;      // Error: 'i32' takes no type arguments
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
let n = new Box<Box<i32>> { value: new Box<i32> { value: 3, }, spares: new Box<i32>[0], };
println(itoa(n.value.value)); // 3
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
impl Holder<T> for Box<T> {
    pub fn held() -> T {
        return self.value;
    }
}
```

The target may be written bare when nothing needs to name its parameters. These
two say the same thing:

```mew
impl Describable for Box { }
impl Describable for Box<T> { }
```

Either way the block covers every filling in of the type at once, so a value can
be used through the interface with its arguments chosen.

```mew
pub fn read(holder: Holder<i32>) -> i32 {
    return holder.held();
}

println(itoa(read(new Box<i32> { value: 5, })));
```

### One filling in at a time

`impl Describable for Box<i32>` is an error. There is no way to give one filling
in of a type behaviour that the others do not have.

This is not only a missing feature. A generic type is emitted once, with the
parameters left open, and its interfaces are fixed where it is declared. Giving
`Box<i32>` an interface that `Box<string>` lacks would mean emitting a separate
type per filling in, and answering what happens when a general `impl` and a
specialized one both apply. What that example usually wants is a constraint,
which is [still to come](../future/generics.md#constraints).

### Not yet

Three things the [Generics](../future/generics.md) page describes do not work
yet:

- **Constraints.** `T` has no members, so nothing can be done with a value of
  type `T` beyond storing it, passing it, and handing it back.
- **Generic functions.** Only a type or an interface takes parameters. A free
  function or a method cannot declare its own.
- **Calling a static method on a named type.** `Box<i32>::empty()` does not
  parse, because `<` in an expression is still a comparison.
