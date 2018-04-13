CD/DVD/HD Indexer : A work-in-progress.

1. Disk read is now faster.
1. Using SQLite as persistence is partially complete. Current speed to load is much faster! (Missing Logical Folders, Compressed files)
See a ![benchmark](docs/bench/bench.md)
1. Added "show file in windows explorer".
1. Added folder consumption statistics to the tree.
1. Replaced CRC32 hash with MD5 - built-in and faster.
1. Charting is in-progress.

*Note*: this fork is dependent on my matching fork of 'Igorary'.

## Project Description ##

This application creates indexes (table of contents) of your floppies, CD/DVD disks, hard/external/network disks, pendrives and other removables media etc. It helps you organize and maintain your collection of files and provides reasonable search capabilities. It is designed to be simple, small, effective and easy to use. Written in C#.

Former name: _Blue Mirror CD/DVD Indexer_.

### System Requirements ###

* For use: .NET Framework 4.5
* For development: Visual Studio 2013

## Screenshots ##

Main window:

![Main window](docs/img/MainWindow.png)

Search window:

![Search window](docs/img/SearchWindow.png)

Search window (advanced view):

![Search window (advanced view)](docs/img/SearchWindow2.png)

Read volume dialog:

![Read volume dialog](docs/img/ReadingVolume2.png)

Reading options:

![Reading options](docs/img/ReadingOptions.png)

![Reading options](docs/img/ReadingOptions2.png)
