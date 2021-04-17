# Tesla CLI

This simple application allows you to query and command a Tesla vehicle from the command line. It's a work in progress - not all commands are implemented yet, but it shows how to work with the API.

## Building the app

This is a .NET core app. You can run it using `dotnet run -- {params}`, but a nicer way is to create a self-contained executable with the `dotnet publish` command.

```bash
# Linux
dotnet publish -r linux-x64 -p:PublishSingleFile=true -c Release --self-contained=false -p:PublishTrimmed=true

# windows
dotnet publish -r win-x64 -p:PublishSingleFile=true -c Release --self-contained=true -p:PublishTrimmed=true

# macos
dotnet publish -r osx-x64 -p:PublishSingleFile=true -c Release --self-contained=true -p:PublishTrimmed=true
```

This will place a single executable for the OS into the `/bin/Release/netcoreapp3.1/{os-type}/publish` folder.

## Signing in

Use the `login` command to sign into your Tesla account. The app will save the token information on disk.

```console
teslacli login elon@tesla.com
```

You will be prompted for the password and possibly for a passcode if MFA is enabled. Once you are authenticated, you can execute other commands.

You can also include passwords and passcodes on the command line if logging in from a script or collecting input from another source.

```output
login:
  Sign in to the Tesla API with a username/password.

Usage:
  TeslaCli login [options] <email>

Arguments:
  <email>    Email account

Options:
  -p, --password <password>    Password
  -c, --code <code>            Multifactor auth passcode
  -b, --backup <backup>        Multifactor auth backup code
  -?, -h, --help               Show help and usage information
```

## Queries

Use the `vehicle` command to execute all state and command options.

For example, to list all vehicles, use:

```console
teslacli vehicle list
```

This produces:

```output
Name: MyCarName
   Id: XY0001234567
  VIN: XXXXXXXXXXXXXXX
State: Online
```

### Getting help

The app will report all options if any are missing. You can also pass `--help` to get a help screen at any time.

```console
teslacli vehicle
```

```output
vehicle:
  Perform commands on vehicles

Usage:
  TeslaCli vehicle [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  list          List all the vehicles associated with the account.
  wake          Wake up a sleeping car
  chargeinfo    Get the current charge information for a vehicle.
  location      Get the current driving status and location for a vehicle.
  status        Get the current status for a vehicle.
  config        Get the vehicle configuration.
  honk          Honk the horn
  flash         Flash the lights.
  doors         Lock/Unlock the doors
  climate       Control the climate system.
```

### Targeting a car

To target a specific vehicle, you can pass an `id` parameter - this should match the Id shown in the vehicle list.

```console
teslacli vehicle wake id=XYZ000000000
```

If no `id` is passed, the _first_ car in the vehicle list will be targeted.

### Output types

The app supports different output styles which can be set with the `-o` or `--output` option on any query that returns data.

| Value | Description |
|-------|-------------|
| `Text` | The default, text output |
| `CSV` | Comma-delimited with headers. Unit measurements are not included in the values to make them numbers. |
| `Table` | A table format. This omits less valuable data to keep the table visible. |
| `JSON` | The raw JSON packet returned from the Tesla API |

```console
teslacli vehicle config -o table
```

```output
Car Type Exterior Color Wheel Type Roof Color     Spoiler Type Charge Port Type
-------- -------------- ---------- -------------- ------------ ----------------
model3   SolidBlack     Stiletto20 RoofColorGlass Passive      US
```

```console
teslacli vehicle config -o text
```

```output
        Car Type: model3
  Exterior Color: SolidBlack
      Wheel Type: Stiletto20
      Roof Color: RoofColorGlass
    Spoiler Type: Passive
Charge Port Type: US
```

```console
teslacli vehicle config -o json
```

```output
{
  "response": {
    "can_accept_navigation_requests": true,
    "can_actuate_trunks": true,
    "car_special_type": "base",
    "car_type": "model3",
    "charge_port_type": "US",
    "default_charge_to_max": false,
    "ece_restrictions": false,
    "eu_vehicle": false,
    "exterior_color": "SolidBlack",
    "exterior_trim": "Chrome",
    "has_air_suspension": false,
    "has_ludicrous_mode": false,
    "key_version": 2,
    "motorized_charge_port": true,
    "plg": false,
    "rear_seat_heaters": 1,
    "rear_seat_type": null,
    "rhd": false,
    "roof_color": "RoofColorGlass",
    "seat_type": null,
    "spoiler_type": "Passive",
    "sun_roof_installed": null,
    "third_row_seats": "None",
    "timestamp": 1618536192450,
    "use_range_badging": true,
    "wheel_type": "Stiletto20"
  }
}
```
