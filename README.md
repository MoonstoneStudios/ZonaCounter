# ZonaCounter
A quick CLI app I created to let me keep track of how much money I spend on Arizona Tea.

The default behaviour when calling with no arguments is to increment "Ginseng and Honey," or what you set ad the default, by 1.

**NOT YET TESTED ON WINDOWS OR MACOS!!**

Usage:

```
> ./ZonaCounter [count] [name] [unit price]
```
All above arguments are optional. If you use an argument, all previous arguments must be also present.
When no arguments are used, the default product will be incremented by 1.
Default unit price = $0.99

OR

```
> ./ZonaCounter clear/help/stats
```
'clear' will delete files, 'help' shows this message, 'stats' shows all data.

OR

```
> ./ZonaCounter default_product (name)
```
'name' here is required.

OR

```
> ./ZonaCounter change_unit_price (name) (unit price)
'name' and 'unit price' here are required.
```
