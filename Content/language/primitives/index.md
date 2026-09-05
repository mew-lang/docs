---
title: Primitives
uid: language.primitives
order: 1
---

Fifteen types are built into the language rather than declared in it. They cover
numbers, text and truth values, the absence of a value, `any`, which holds a value
of any type, and `void`, which holds nothing at all.

Their names are not keywords, so a local or a field may
[take one](xref:language.names#primitive-names-are-not-keywords), though a type
declaration may not.

- [Null](xref:language.primitives.null): the absence of a value, and which types accept one.
- [Numbers](xref:language.primitives.numbers): signed and unsigned integers, floating point, suffixes and coercion.
- [Text](xref:language.primitives.text): strings and characters.
- [Any](xref:language.primitives.any): a value of any type, with nothing reachable through it.
- [Booleans](xref:language.primitives.bool): `true` and `false`, and the operators over them.
- [Void](xref:language.primitives.void): what a function returns when it returns nothing.
