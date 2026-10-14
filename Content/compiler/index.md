---
title: Compiler
uid: compiler
order: 1
---

The compiler reads Mew source and writes a .NET assembly. Nothing
else is involved: no project file, no SDK, and no other language
in between.

Between reading and writing, the program is held in four shapes,
each one plainer than the last. The first is the code exactly as
it was typed. The last is a flat list of instructions, the shape
the assembly wants. The two in between are where names get their
meaning and where loops and conditions become branches.

```beck
type: architecture
meta:
  direction: LR
  spacing: { rank: 28, node: 32 }
nodes:
  - { id: source, title: Source, subtitle: .mew files, kind: user, shape: pill }
  - { id: ast, title: AST, subtitle: 1. Parsing, shape: pill }
  - { id: hir, title: HIR, subtitle: 2. Binding, shape: pill }
  - { id: mir, title: MIR, subtitle: 3. Lowering, shape: pill }
  - { id: lir, title: LIR, subtitle: 4. Lowering, accent: success, shape: pill }
  - { id: out, title: .mew/, subtitle: 5. Emitting, shape: pill }
edges:
  - { from: source, to: ast }
  - { from: ast, to: hir }
  - { from: hir, to: mir }
  - { from: mir, to: lir }
  - { from: lir, to: out }
flow:
  repeatDelay: 2
  steps:
    - packet: { from: source, to: ast }
    - packet: { from: ast, to: hir }
    - packet: { from: hir, to: mir }
    - packet: { from: mir, to: lir }
    - packet: { from: lir, to: out, color: success }
    - pulse: { node: out, color: success }
    - reset:
```

The four front-end stages are trees and lists the compiler owns.
Only the backend knows how a program is written out, and it is
reached through `LIR`.

Here is a small program at every stage, from the source to the IL the backend wrote:

<MewPipeline>

```mew
use std;

fn square(x: i32) -> i32 {
    return x * x;
}

for i in 1..5 {
    println($"{square(i)}");
}
```

</MewPipeline>

## 1. AST parsing

The parsing step iterates through all source files, and
builds a syntax tree for each of them.  
The syntax tree represents the code as it was written, 
maintaining the trivia such as white space, comments etc.

Each node in the AST has a reference to both its parent
and children. 

Apart from being the basis for `HIR` generation, the AST
is also used to interact with the source code programmatically,
i.e. from the LSP server.

## 2. HIR generation

HIR, short for _High-level Intermediate Representation_, 
is a bound tree, where all types are known.  

The HIR references resolved _symbols_ for the different parts
of Mew (namespaces, types, functions, parameters, variables etc).
For example, two code blocks that call a function will have
the same symbol reference to that function.

1. Build symbol table
   1. Namespaces
   1. Types
   1. Free functions
   1. Type members
1. Binding
   1. Types
   1. Free functions
   1. Top level statements

> [!IMPORTANT]
> HIR might contain errors, represented as error symbols.

Binding is also where the compiler stops being alone. Platform
types are read from .NET metadata, the editor asks the HIR what
could complete a line, and anything that does not resolve is
reported from here.

```beck
type: architecture
meta:
  direction: TB
  spacing: { rank: 48, node: 40 }
  narrate: { wpm: 190, min: 1.8 }
nodes:
  - { id: ast, title: AST, subtitle: one tree per file, icon: code }
  - { id: dotnet, title: .NET metadata, subtitle: reference assemblies, kind: external }
  - id: hir
    title: HIR
    status: may hold error symbols
    icon: search
    items:
      - every name a symbol
      - every expression a type
  - { id: tools, title: Editor, subtitle: completion, kind: external, icon: terminal, variant: subtle }
  - { id: diag, title: Diagnostics, accent: danger, variant: subtle, icon: bell }
edges:
  - { from: ast, to: hir, label: bind }
  - { from: dotnet, to: hir, label: platform types, kind: dependency }
  - { from: hir, to: tools, kind: dependency }
  - { from: hir, to: diag, kind: dependency, color: danger }
flow:
  repeatDelay: 2
  steps:
    - narrate: "Binding gives every name a symbol and every expression a type. Platform types are read from .NET metadata."
    - parallel:
        - packet: { from: ast, to: hir }
        - packet: { from: dotnet, to: hir, color: neutral }
    - working: { node: hir }
    - packet: { from: hir, to: tools, color: neutral, label: completion }
    - narrate: { text: "A name that does not resolve becomes an error symbol. The tree still builds, and the error is reported.", color: danger }
    - packet: { from: hir, to: diag, color: danger, shape: ring }
    - idle: { node: hir }
    - reset:
```

## 3. MIR generation

MIR, short for _Medium-level Intermediate Representation_,
is a lowered HIR, without constructs such as `while`/`loop`/`if`.

* All higher-level constructs such as loops and conditions 
have been lowered into labels and branches.
* Control flow analysis and some optimizations 
are done here as well.

> [!IMPORTANT]
> MIR might contain errors, represented as error symbols.

## 4. LIR generation

LIR, short for _Low-level Intermediate Representation_,
is a lowered MIR, shaped like the instructions that will be
emitted.

Where MIR is still a tree, LIR is a flat list per method over an
evaluation stack. It numbers the locals, makes every conversion
and every box explicit, resolves which member a name meant, and
turns a lambda into a type. It is still typed with the compiler's
own symbols rather than with .NET's, so it can be read back and
compared in a test.

> [!WARNING]
> LIR **MUST NOT** contain any errors.

This is the gate. MIR is the last shape that can carry an error
symbol, and anything it reports stops the program here. A
function the lowering cannot finish is recorded, and the backend
refuses the whole program instead of emitting a module with a
hole in it.

```beck
type: architecture
meta:
  direction: LR
  spacing: { rank: 56, node: 40 }
  narrate: { wpm: 190, min: 1.8 }
nodes:
  - id: mir
    title: MIR
    status: may hold error symbols
    icon: git
  - id: lir
    title: LIR
    status: must be error-free
    accent: success
    icon: stream
    items:
      - a flat list per method, over a stack
      - numbered locals, explicit boxes
  - { id: diag, title: Diagnostics, accent: danger, variant: subtle, icon: bell }
edges:
  - { from: mir, to: lir, label: only when clean }
  - { from: mir, to: diag, kind: dependency, color: danger }
flow:
  repeatDelay: 2
  steps:
    - narrate: { text: "A program with an error stops at MIR. It is reported, and nothing is lowered further.", color: danger }
    - packet: { from: mir, to: diag, color: danger, shape: ring }
    - wait: 1
    - narrate: "A clean program is flattened into a typed instruction list over an evaluation stack."
    - packet: { from: mir, to: lir, color: success }
    - pulse: { node: lir, color: success }
    - reset:
```

## 5. Writing the assembly

The backend turns LIR into a .NET assembly with
`System.Reflection.Metadata`, writing the IL itself rather than
going through another language. It is written to a `.mew`
directory beside the file the program starts from, together with
the runtime configuration the host reads. The .NET SDK is not
involved, and neither is a project file.

```beck
type: architecture
meta:
  direction: LR
  spacing: { rank: 56, node: 32 }
nodes:
  - { id: lir, title: LIR, accent: success, icon: stream }
  - id: emit
    title: Emitter
    icon: function
    items:
      - System.Reflection.Metadata
      - writes the IL itself
  - { id: dll, title: Assembly, subtitle: .dll, kind: external, icon: file }
  - { id: cfg, title: Runtime config, subtitle: what the host reads, kind: external, icon: file }
  - { id: pdb, title: Debug database, subtitle: lines of Mew, kind: external, icon: file }
edges:
  - { from: lir, to: emit, label: write }
  - { from: emit, to: dll }
  - { from: emit, to: cfg }
  - { from: emit, to: pdb }
flow:
  repeatDelay: 2
  steps:
    - packet: { from: lir, to: emit, color: success }
    - working: { node: emit }
    - parallel:
        - packet: { from: emit, to: dll, color: success }
        - packet: { from: emit, to: cfg, color: success }
        - packet: { from: emit, to: pdb, color: success }
    - idle: { node: emit }
    - status: { node: emit, text: written to .mew/, color: success }
    - wait: 2
    - reset:
```

Nothing above LIR knows how a program is written out, so the
language is not defined in terms of what .NET does. The symbol
model does read .NET metadata, so a program can name a platform
type.

A debug database is written beside the assembly, so a debugger
can stop on a line of Mew.