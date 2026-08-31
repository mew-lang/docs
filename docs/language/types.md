---
sidebar_position: 40
---

# Types

A type declaration takes `pub` and nothing else. `pub` makes the type visible to
other files. Without it the type belongs to the file that declares it, and that is
narrower than its namespace: two files sharing a `namespace` still cannot see each
other's private types.

```mew
pub type Point { }

type Internal { }
```

:::note
`static` and `external` are not modifiers a type accepts, and using one is an
error. `static` belongs on a method, and `external` on a function.
:::

### Fields

:::note
All fields must be initialized when creating a class.
:::

```mew
pub type Person {
    pub field name: string;
}
```

```mew
// Usage:
let person = new Person { 
    name: "Patrik" 
};
```

### Methods

```mew
pub type Clock {
    pub fn get_current_time() -> Timestamp {
        return Timestamp::Now;
    }
}
```

```mew
// Usage:
let clock = new Clock { };
let now = clock.get_current_time();
```

#### self

Inside a method, a field is read by name. `self` names the value the method was
called on, and is only needed when something else has taken the name.

```mew
pub type Counter {
    pub field value: i32;

    pub fn plus(value: i32) -> i32 {
        return self.value + value;
    }
}
```

Here `value` is the parameter and `self.value` is the field. Without a parameter of
that name, `value` and `self.value` mean the same thing.

:::note
`self` is a reserved word, so it cannot be used as a name. It is an error outside a
type, and in a `static fn`, which has no value to refer to.
:::

#### Static methods

```mew
pub type Clock {
    pub static fn get_current_time() -> Timestamp {
        return Timestamp::Now;
    }
}
```

```mew
// Usage:
let now = Clock::get_current_time();
```

### Constructors

Mew does not have constructors per se, but uses
one or more static methods; by convention called `new`.

```mew
pub type Person {
    pub field name: string;

    pub static fn new(name: string) -> Person {
        return new Person { name: name };
    }
}
```

```mew
// Usage:
let person = Person::new("Patrik");
```