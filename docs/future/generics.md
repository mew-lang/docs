---
sidebar_position: 100
---

# Generics

:::info
This functionality is not yet implemented
:::

### Types

```mew
pub type Smallest<T : Comparable<T>> {
    mut field smallest : Optional<T>;

    pub static fn new() -> MyList<T> {
        smallest = Optional<T>.none;
    }

    pub fn add_smallest(item: T) {
        if item == .none {
            return;
        }

        if self.smallest == null || 
           item.CompareTo(self.smallest) == -1 
        {
            self.smallest = item;
        }
    }

    pub fn GetSmallest() -> Optional<T> {
        return smallest;
    }
}
```

### Unions

```mew
pub union Result<T, V : Exception> {
    Ok(T),
    Err(V),
}
```
