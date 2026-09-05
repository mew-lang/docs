---
title: Namespaces
uid: language.namespaces
order: 18
---

A namespace is a name attached to a file. Everything the file declares belongs to
it.

```mew
namespace Geometry.Shapes;

pub type Point {
    pub field x: i32;
    pub field y: i32;
}
```

The declaration is the first thing in the file, before even a `use`, and a file
declares at most one. A namespace has no body and is not itself a declaration, so
several files may name the same one and all contribute to it.

A file with no `namespace` line contributes to the global namespace, which is where
most small programs live.

## Reaching a namespace without importing it

A name may be written with the namespace it lives in, one segment at a time,
separated by `.`. That works for a function, for a type in a `new` expression, and
for a type in an annotation.

```mew
// file: geometry.mew
namespace Geometry;

pub type Point {
    pub field x: i32;
}

pub fn origin() -> Point {
    return new Point { x: 0 };
}

// file: main.mew
#load "geometry.mew"

use std;

let made = Geometry.origin();
let built = new Geometry.Point { x: 1 };
let held: Geometry.Point = built;

println($"{made.x + held.x}");
```

Importing a namespace that no file in the compilation declares is an error, so a
typo in a `use` is caught at the line that wrote it.

```mew error=MEW2053
use std;
use Missing;
```

A written-out prefix that leads nowhere is reported against the name it failed to
find rather than against the namespace.

> [!NOTE]
> `.` separates namespace segments and reaches an instance member. `::` reaches a
> [static member](xref:language.types#static-methods) of a type and a
> [union case](xref:language.unions#building-a-value). The two are not
> interchangeable, and which one applies follows from what is on the left.

## Importing with `use`

`use` drops the prefix for one namespace. Everything that namespace makes `pub` is
then reachable by its own name.

```mew
// file: geometry.mew
namespace Geometry;

pub fn area(side: i32) -> i32 {
    return side * side;
}

// file: main.mew
#load "geometry.mew"

use std;
use Geometry;

println($"{area(4)}");
```

Every `use` comes after the namespace declaration and before the first
declaration in the file. One written later is an error.

A `use` is per file. Importing a namespace in one file does nothing for any
other, so each file says for itself what it depends on.

## Visibility

`pub` is the only visibility modifier, and what it means depends on what carries
it.

| On | Without `pub` | With `pub` |
| :- | :------------ | :--------- |
| A type, union or interface | The file that declares it | Every file in the compilation |
| A free function | The file that declares it | Every file in the compilation |
| A field or a method | The type that declares it | Every file in the compilation |

The first two rows are the ones that surprise people: a declaration without `pub`
belongs to its file, which is narrower than its namespace. Two files sharing a
namespace still cannot see each other's private declarations.

```mew error=MEW2047
// file: geometry.mew
namespace Geometry;

type Hidden {
    pub field x: i32;
}

// file: main.mew
#load "geometry.mew"

namespace Geometry;

let value = new Hidden { x: 1 };
```

Reaching something that exists but is not visible is reported as exactly that,
rather than as a name that does not exist. Saying a name is missing when it is
right there would send a reader hunting for a typo.

A [field or method](xref:language.types) is scoped to its type rather than to its
file, so `pub` on one opens it to everything. Interface members are public by
definition, and writing `pub` inside an `interface` block is an error.
