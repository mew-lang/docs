---
sidebar_position: 15
---

# Completion

An editor asks what could go where the cursor is, and the answer arrives while
the line is still half written. That is the only state that matters: a
suggestion after the code is already complete is a suggestion nobody needed.

Completion is offered on `.` and on `::`, and is triggered by typing either.

## After a dot

A value's own members, whether or not the member has been typed yet.

```mew
pub type Point {
    pub field x: i32;
    pub field y: i32;

    pub static fn origin() -> Point { return new Point { x: 0, y: 0 }; }

    pub fn shifted(dx: i32) -> Point { return new Point { x: self.x + dx, y: self.y }; }
}
```

| Written        | Offered                       |
| :------------- | :---------------------------- |
| `p.`           | `x`, `y`, `shifted`           |
| `self.`        | `x`, `y`, `shifted`           |
| `Point::`      | `origin`                      |

A type on the left is a static access, so only what can be reached that way
comes up. `self` is a value even though it names its own type, so it offers the
instance members.

A [union](../language/unions.md) offers its cases through `::`, since that is
how a case is reached.

```mew
pub union Slot {
    empty,
    filled(i32),
}
```

| Written        | Offered              |
| :------------- | :------------------- |
| `Slot::`       | `empty`, `filled`    |

## A dot with no union named

A [pattern](../language/unions.md#reading-a-value) and a
[case written without its union](../language/unions.md#leaving-the-union-out)
both start with a bare `.`, and both offer the cases of the union in view.

```mew
match slot {
    .        // empty, filled
}
```

Where a case is written without its union, the union comes from whatever the
value is going into, and completion reads it the same way the compiler does.

```mew
pub fn chosen() -> Slot {
    return .          // empty, filled
}

pub fn held(slot: Slot) -> i32 { return 0; }
```

| Written                     | Offered where the union is        |
| :-------------------------- | :-------------------------------- |
| `return .`                  | the function's return type        |
| `let s: Slot = .`           | the annotation                    |
| `s = .`                     | the variable being assigned to    |
| `held(.`                    | the parameter                     |
| `new Holder { slot: . }`     | the field                         |
| `new Slot[] { . }`           | the array's element type          |

Where nothing says which union is wanted, there are no cases to offer and the
ordinary list of names in scope comes up instead. `let s = .` with no
annotation is one such place, and so is `let n: i32 = .`.

:::note
Where two overloads take different unions, the case name is what picks between
them, so `held(.` offers the cases of whichever union that call could want.
Where both could want a case of that name, one of them is offered. Writing the
union out says which.
:::

## Everywhere else

Anything that is not after a dot offers what is in scope: locals declared above
the cursor, the enclosing function's parameters, the enclosing type's members,
the file's own declarations, everything the
[standard library](../stdlib/index.md) puts in the global namespace, then
keywords and the primitive type names.

A local is not offered on the line that declares it, and a local declared below
the cursor is not offered at all.
