---
sidebar_position: 30
---

# Comments

### Single line comments

```mew
// This is a comment
// This is another comment

fn Foo() {
    let bar = 1; // Inline comments are allowed as well
}
```

### Multi line comments

```mew
/*
This is a
multiline
comment
*/
```

### Doc comments

Doc comments are comments that provide documentation
for a type, method, function, or member.

```mew
/// This is a doc comment for `foo`
fn Foo() {
    /// This is a doc comment for `bar`
    let bar = 1;
}
```