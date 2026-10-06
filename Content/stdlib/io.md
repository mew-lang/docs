---
title: std.io
uid: stdlib.io
order: 8
---

## Types

| Type                      | Description                     |
| :------------------------ | :------------------------------ |
| [`Directory`](#directory) | A path to a directory.          |
| [`File`](#file)           | A path to a file.               |
| [`Files`](#files)         | Files matched by glob patterns. |

## Functions

| Function                                                           | Description                                                                                                                        |
| :----------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------------------------------- |
| `compress(source: Directory, archive: File) -> Result<(), string>` | Compresses the directory into a zip archive, replacing any existing archive.                                                       |
| `directory(path: string) -> Directory`                             | Creates a `Directory` for the path, without touching the disk.                                                                     |
| `exists(path: string) -> bool`                                     | Whether a file or directory exists at the path.                                                                                    |
| `file(path: string) -> File`                                       | Creates a `File` for the path, without touching the disk.                                                                          |
| `glob(patterns: ...string) -> Files`                               | Finds the files that match the patterns, sorted by path. A pattern starting with `!` excludes what the patterns before it matched. |
| `read_text(source: File) -> Result<string, string>`                | Reads the file as UTF-8 text.                                                                                                      |
| `write_text(target: File, text: string) -> Result<(), string>`     | Writes the text to the file as UTF-8, replacing what was there. The directory must already exist.                                  |

## `Directory`

| Field          | Description                |
| :------------- | :------------------------- |
| `path: string` | The path to the directory. |

## `File`

| Field          | Description                           |
| :------------- | :------------------------------------ |
| `path: string` | The path to the file.                 |
| `name: string` | The file name, with its extension.    |
| `stem: string` | The file name, without its extension. |

## `Files`

Implements `Enumerable<File>`.

| Field                | Description                               |
| :------------------- | :---------------------------------------- |
| `files: File[]`      | The matched files, sorted by path.        |
| `patterns: string[]` | The patterns the files were matched with. |
