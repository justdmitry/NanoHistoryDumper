# NANO History Dumper 

Reads all account histrory from node via RPC and print it on screen.

## Features

* Auto-detecting of nano/banano nodes, auto nano/ban conversion from raw
* Cross-platform (install [.NET Core SDK](https://dotnet.microsoft.com/download) for your OS)


## Installation

1. Download and install [.NET Core SDK](https://dotnet.microsoft.com/download) 2.2 or above (but not ".NET Framework"!)
2. Clone this repo
3. `cd NanoHistoryDumper`
4. `dotnet build` (should see green "Build succeeded" text)


## How to use

1. Edit `appsettings.json`: put correct ip and port of your node RPC (if not default `127.0.0.1:7072`)
3. Run `dotnet run <account>` from console
