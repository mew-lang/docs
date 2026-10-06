---
title: std.build.engine
uid: stdlib.build.engine
order: 4
---

## Unions

| Union                         | Description          |
| :---------------------------- | :------------------- |
| [`BuildInput`](#buildinput)   | An input of a task.  |
| [`BuildOutput`](#buildoutput) | An output of a task. |

## Types

| Type                                  | Description                                  |
| :------------------------------------ | :------------------------------------------- |
| [`BuildFlags`](#buildflags)           | The engine flags from the command line.      |
| [`BuildGraph<C>`](#buildgraphc)       | The tasks of a build and their dependencies. |
| [`BuildInstance<F>`](#buildinstancef) | One instance of a rule.                      |
| [`BuildValue<T>`](#buildvaluet)       | Holds the value a task returns.              |
| [`BuildVariable`](#buildvariable)     | An environment variable read by `env`.       |

## Functions

| Function                                                                                                                    | Description                                                                    |
| :-------------------------------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------- |
| `concat_infos(first: std.build.BuildTask[], second: std.build.BuildTask[]) -> std.build.BuildTask[]`                        | Concatenates two lists of tasks.                                               |
| `each<T, R>(values: T[], register: fn(T) -> R[]) -> R[]`                                                                    | Calls `register` for each value and concatenates the results.                  |
| `each_output<G, T, R>(instances: BuildInstance<G>[], pick: fn(G) -> T, register: fn(T, std.build.BuildTask) -> R[]) -> R[]` | Calls `register` for one output of each instance and concatenates the results. |
| `files_of<F>(instances: BuildInstance<F>[]) -> F[]`                                                                         | Returns the files of each instance.                                            |
| `infos_of<F>(instances: BuildInstance<F>[]) -> std.build.BuildTask[]`                                                       | Returns the task of each instance.                                             |
| `main<C>(arguments: string[], setup: fn(string[]) -> Result<C, string>, graph: fn(C) -> BuildGraph<C>) -> i32`              | Runs `setup`, builds the graph and runs the targets. Returns the exit code.    |
| `rematch_files(files: std.io.Files) -> std.io.Files`                                                                        | Matches the patterns of `files` again.                                         |

## `BuildInput`

| Case                          | Description                     |
| :---------------------------- | :------------------------------ |
| `file(std.io.File)`           | A file.                         |
| `directory(std.io.Directory)` | A directory.                    |
| `files(std.io.Files)`         | Files matched by glob patterns. |
| `variable(BuildVariable)`     | An environment variable.        |
| `context(string)`             | The context, as text.           |
| `code(string)`                | The hash of the task's code.    |

## `BuildOutput`

| Case                          | Description                                         |
| :---------------------------- | :-------------------------------------------------- |
| `file(std.io.File)`           | A file.                                             |
| `directory(std.io.Directory)` | A directory.                                        |
| `files(std.io.Files)`         | Files matched by glob patterns.                     |
| `later(fn() -> std.io.File)`  | A file whose path is only known when the task runs. |

## `BuildFlags`

| Field           | Description                                                 |
| :-------------- | :---------------------------------------------------------- |
| `verbose: bool` | Shows what `verbose` writes. Set by `--verbose`.            |
| `force: bool`   | Runs tasks even when they are up to date. Set by `--force`. |
| `trace: bool`   | Writes the run to `.mew/trace.json`. Set by `--trace`.      |

## `BuildGraph<C>`

| Member                                                                                                                                                             | Description                                                                      |
| :----------------------------------------------------------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------- |
| `BuildGraph<C>::new(context: C) -> BuildGraph<C>`                                                                                                                  | Creates an empty graph for the context.                                          |
| `action(name: string, run: fn(C) -> Result<(), string>, dependencies: ...std.build.BuildTask) -> std.build.BuildTask`                                              | Adds a task that neither returns a value nor writes files.                       |
| `value(name: string, run: fn(C) -> Result<string, string>, dependencies: ...std.build.BuildTask) -> std.build.BuildTask`                                           | Adds a task that returns a value as text.                                        |
| `files(name: string, inputs: BuildInput[], outputs: BuildOutput[], run: fn(C) -> Result<(), string>, dependencies: ...std.build.BuildTask) -> std.build.BuildTask` | Adds a task that reads or writes files.                                          |
| `group(name: string, dependencies: ...std.build.BuildTask) -> std.build.BuildTask`                                                                                 | Adds a task that only has dependencies.                                          |
| `retry(info: std.build.BuildTask, times: i32)`                                                                                                                     | Retries the task up to `times` times when it fails.                              |
| `when(info: std.build.BuildTask, test: fn() -> bool)`                                                                                                              | Runs the task only when `test` returns true.                                     |
| `when_context(info: std.build.BuildTask, test: fn(C) -> bool)`                                                                                                     | Runs the task only when `test` returns true for the context.                     |
| `teardown(run: fn(C, bool) -> Result<(), string>)`                                                                                                                 | Sets a function to run after the last task, passing whether the build succeeded. |
| `before(run: fn(C, std.build.BuildTask) -> Result<(), string>)`                                                                                                    | Sets a function to run before each task.                                         |
| `after(run: fn(C, std.build.BuildTask, bool) -> Result<(), string>)`                                                                                               | Sets a function to run after each task, passing whether it succeeded.            |
| `execute(targets: string[], flags: BuildFlags) -> i32`                                                                                                             | Runs the targets. Returns the exit code.                                         |

## `BuildInstance<F>`

| Field                       | Description                    |
| :-------------------------- | :----------------------------- |
| `info: std.build.BuildTask` | The task of the instance.      |
| `files: fn() -> F`          | The files the instance writes. |

## `BuildValue<T>`

| Member                                                                             | Description                                            |
| :--------------------------------------------------------------------------------- | :----------------------------------------------------- |
| `keep(result: Result<T, string>, text: fn(T) -> string) -> Result<string, string>` | Stores the value from `result` and returns it as text. |
| `get() -> T`                                                                       | Returns the stored value.                              |

## `BuildVariable`

| Field                   | Description                                            |
| :---------------------- | :----------------------------------------------------- |
| `name: string`          | The name of the variable.                              |
| `value: Option<string>` | The value of the variable, or `none` if it is not set. |
