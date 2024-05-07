---
sidebar_position: 90
---

# Interfaces

```mew
use Mew;

interface Distance {
    fn get_distance() -> f64;
}

type Point {
    pub field x : i32;
    pub field y : i32;
};

impl Distance for Point {
    fn get_distance() -> f64 {
        return Math::sqrt(
            first: self.X * self.X +
            second: self.Y * self.Y);
    }
}
```

```mew
// Usage:
let point = Point(x: 32, y: 40);
let dist = point.get_distance();
```