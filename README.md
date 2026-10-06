# Documentation

This is the documentation for Mew, built with [Pennington](https://usepennington.github.io/pennington/).

## Running

```console
$ dotnet run
```

The dev server hot-reloads on edits under `Content/`.

## Building

```console
$ dotnet run -- build
```
## The standard library pages

The pages under `/stdlib/` are not written by hand. `resources/stdlib.xml` describes
the library, and `Stdlib/` turns it into one page per namespace at startup: the
loader reads the XML, the content service publishes a route, a sidebar entry, a
search entry and an `xref` uid (`stdlib.io` and so on) for each namespace, and
`NamespacePage.razor` renders it with the type names in signatures linked to where
they are declared. `Content/stdlib/index.md` is the one hand-written page in the area.

To update the reference, regenerate `resources/stdlib.xml` and restart the dev server.
