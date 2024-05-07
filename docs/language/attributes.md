---
sidebar_position: 130
---

# Attributes

Attributes, indicated by the `[]` symbols, allows attachment of metadata, annotations, or special instructions to various language constructs, such as types, fields, and functions. 

* Metadata: Attributes provide metadata about code elements. This metadata can be used for documentation, code generation, and tooling.

* Configuration: Attributes allow developers to configure the behavior of code elements. They can control aspects like visibility, serialization, and more.

* Annotations: Attributes can annotate code with additional information, aiding in code analysis and understanding.¨

### Example

Let's explore an example to illustrate how attributes work in Mew:

```mew
[foo(a: "arg1", b: "arg2")]
pub type Foo {
    [bar(a: "arg1", b: "arg2")]
    pub field Bar: i32;

    [baz(a: "arg1", b: "arg2")]
    pub fn Baz() {
    }
}
```

In this example:

* `[foo(a: "arg1", b: "arg2")]` is an attribute associated with the type `Foo`.

* `[bar(a: "arg1", b: "arg2")]` is an attribute associated with the field `Bar`.

* `[baz(a: "arg1", b: "arg2")]` is an attribute associated with the method `Baz`.
