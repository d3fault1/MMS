# MMS

Windows console for edge and media devices. It shows which devices are online, plays and controls media on them, and keeps content, software, and command history in one place.

---

## Overview

- Device dashboard for online, offline, and warning counts.
- Device list for registered machines.
- Control panel for playback, volume, and seek, grouped by floor.
- Content upload and software update.
- Command log for commands already sent.

---

## Tech Stack

| Component | Technology |
| --- | --- |
| **Language** | C# |
| **UI** | WPF, .NET Framework 4.7.2 |
| **Data** | SQLite |

---

## Project Structure

```
MMS/
├── MMS.sln
└── MMS/
    ├── UI/Views/AppPages/     # Screens
    ├── Backend/               # Listener, commands, database
    ├── DataModels/
    └── packages.config
```

---

## Prerequisites

- Windows
- Visual Studio with the .NET desktop workload (.NET Framework 4.7.2)

---

## Setup

Open `MMS.sln`, restore the NuGet packages, and start the project.

On first launch, if `config.ini` is not beside the program, it is created as:

```ini
HTTPServerPort = 2628
DBInstance = "SQLEXPRESS"
DBName = "museum_database"
```

---

## Usage

| Menu | What you do there |
| --- | --- |
| **Device Dashboard** | Counts of online, offline, and warning devices |
| **Total Devices** | List of registered devices |
| **Control Panel** | Pick a floor and control playback on each device |
| **Content Upload** | Send media to devices. Files are kept under `files\media\content` |
| **Software Update** | Send a software package. Files are kept under `files\media\software` |
| **Command Logs** | History of commands and their results |

Devices show up after they connect to this console.
