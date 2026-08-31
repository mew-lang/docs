---
sidebar_position: 110
---

# Namespaces

### Declaring namespaces

Namespaces need to be declared at the top of the 
source file.

```mew
namespace Foo;
```

A namespace can have several segments, separated by `.`.

```mew
namespace Foo.Bar;
```

A file declares at most one namespace. Everything in the file belongs to it.

### Importing namespaces 

Namespace imports need to be declared at the top of the 
source file, after the namespace declaration (if any).

```mew
use Foo;
```

Once imported, everything the namespace makes `pub` is reachable by its own name.

```mew
use Foo;

let value = helper();
let widget = new Widget { n: 1 };
```

### Reaching a namespace without importing it

A name can be qualified with the namespace it lives in, using `.` between the
segments. This works for functions, for types in a `new` expression, and for
types in an annotation.

```mew
let value = Foo.helper();
let widget = new Foo.Widget { n: 1 };
let other: Foo.Widget = null;
```

Nested namespaces are spelled the same way, one segment at a time.

```mew
let value = Foo.Bar.deep();
```

:::note
`.` separates namespace segments and reaches instance members. `::` reaches a
static member of a type, and is covered under [Types](./types.md#static-methods).
:::
