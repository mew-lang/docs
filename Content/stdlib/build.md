---
title: std.build
uid: stdlib.build
order: 2
---

## Types

| Type                      | Description        |
| :------------------------ | :----------------- |
| [`BuildTask`](#buildtask) | A task in a build. |

## Functions

| Function                                              | Description                                                                        |
| :---------------------------------------------------- | :--------------------------------------------------------------------------------- |
| `discovered(paths: ...string)`                        | Records files the running task read, so a change to them reruns it.                |
| `env(name: string) -> std.build.engine.BuildVariable` | Reads an environment variable. As an `in` binding, a change to it reruns the task. |
| `error(text: string)`                                 | Writes the text as an error. Does not fail the task.                               |
| `info(text: string)`                                  | Writes the text to the build output.                                               |
| `verbose(text: string)`                               | Writes the text when the build runs with `--verbose`.                              |
| `warn(text: string)`                                  | Writes the text as a warning.                                                      |

## `BuildTask`

| Field                                         | Description                  |
| :-------------------------------------------- | :--------------------------- |
| `name: string`                                | The name of the task.        |
| `mut inputs: std.build.engine.BuildInput[]`   | The inputs the task reads.   |
| `mut outputs: std.build.engine.BuildOutput[]` | The outputs the task writes. |
