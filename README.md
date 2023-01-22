# wtKST

This software implements a client for the [ON4KST](http://www.on4kst.com/chat/start.php) chat. It is optimized for efficient sked management during
VHF/UHF/SHF contests. It interfaces to Win-Test contest logging software and recently started to implement other
 logging software (e.g. QARTest).

 The software has been written originally by Frank Schmähling DL2ALF (https://github.com/dl2alf) for his team DL0GTH.


This version is licensed under the GPL (v3 or later).
See the file LICENSE for details.

# Features

* Connects to ON4KST chat using the proprietary feed on port 23001
* Supports to connect to one of the available chats
* Filter messages addressed to/from me
* Filter displayed users by distance to station, here/away status and additionally if already in the log
* Sort users alphabetically or by antenna direction
* Display information if a user is QRV probably on a band (based on Airscout database) 
* Support Airplane Scatter status through [Airscout](http://airscout.eu/index.php)
* Supported log (as of now): 
    * [Win-Test v4](http://www.win-test.com/) file and network based
    * [QARTest](https://www.ik3qar.it/software/qartest/en/)
* Make skeds in the Win-Test logging software

# Installation

To install use [this release page](https://github.com/dl8aau/wtkst/releases)

Currently there is no installer. Unzip the archive to a convenient folder and launch
wtkst.exe from there.


# Logging

wtKST logs to %localappdata%\wtKST\wtKST\wtKST_dd.mm.yyyy.log .

Further some debugging information is send through the console output, but currently only
visibule in Visual Studio.
# Bug reports, feedback etc

Please use the [github issues page](https://github.com/dl8aau/wtkst/issues) to report any problems.
Or you can [email me](mailto:a.kurpiers@gmail.com).

# Future plans


# Building from source

You will need Visual Studio Community Edition (2019 or 2022). Load the wtKST.sln and build.

ScoutBase DLLs are currently build in Airscout tree and copied. We could reference them here, too,
but then one would need to checkout Airscout sources, too.
