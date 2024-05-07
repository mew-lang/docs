---
sidebar_position: 140
---

# Project files

:::info
This functionality is not yet implemented
:::

A Mew project file has the extension `.mewx`.

Project files are normal `.mew` files, except that they
can have additional metadata that describes what references
to include, etc.

### Example

```mew
#name "Demo"

#reference "../../foo.mewx"
#reference "../../bar.mewx"

#load "utils.mew"
// ...
```