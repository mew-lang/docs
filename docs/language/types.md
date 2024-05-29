---
sidebar_position: 80
---

# Types

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
use Mew;

pub type Clock {
    pub fn get_current_time() -> Timestamp {
        return Timestamp::Now;
    }
}
```

```mew
// Usage:
let clock = new Clock();
let now = clock.get_current_time();
```

#### Static methods

```mew
use Mew;

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
        return Person(name: name)
    }
}
```

```mew
// Usage:
let person = Person::new("Patrik");
```