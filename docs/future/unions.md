---
sidebar_position: 80
---

# Unions

:::info
This functionality is not yet implemented
:::

```mew
pub union IpAddress {
    none,
    v4(u8, u8, u8, u8),
    v6(string),
}
```

```mew
let noIp = IpAddress::none;
let ipv4 = IpAddress::v4(192, 168, 1, 1);
let ipv6 = IpAddress::v6("fd1a:1021:aa58:4331:d462:421e:9e3c:1cd7");
```

### Match statements

```mew
match ip {
    .none => {
        print("No IP")
    },
    .v4(a, b, c, d) => {
        print("IPV4");
    },
    .v6(a) => {
        print("IPV6");
    },
}
```

### Match expressions

```mew
let isv4 = match ip {
    .none => false,
    .v4(a, b, c, d) => true,
    .v6(a) => false,
}
```

### Add methods to union

```mew
impl IpAddress {
    fn is_v4() -> bool {
        return match this {
            .none => false,
            .v4(a, b, c, d) => true,
            .v6(a) => false,
        }
    }
}
```