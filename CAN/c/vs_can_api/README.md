C Examples for VSCAN API
========================

Requirements
------------

These examples come with a CMake file `CMakeLists.txt`. CMake usage is optional.

You'll also need vs_can_api library files. You can find the ZIP archive in the
download area of your product. Either put them into this folder or anywhere
else where CMake or the IDE of your choice will be able to find them.

Compilation
-----------

### Linux

1. `git clone https://github.com/visionsystemsgmbh/programming_examples.git`
2. `cd CAN/c/vs_can_api`
3. `cmake -B build`
4. `cmake --build build`

### Windows (Microsoft Visual Studio 2008)

1. `git clone https://github.com/visionsystemsgmbh/programming_examples.git`
2. `cd CAN\c\vs_can_api`
3. `cmake -B build -G "Visual Studio 9 2008"`
4. open generated solution file

Usage
-----

### Linux

For USB-CAN Plus devices invoke (S2 - 50000 bit/s):

    vscandump /dev/ttyUSB0 S2

For NetCAN Plus device with the IP address 192.168.1.10 and Data Port 2001
invoke (S2 - 50000 bit/s):

    vscandump 192.168.1.10:2001 S2

### Windows

For USB-CAN Plus devices invoke (S2 - 50000 bit/s):

    vscandump.exe COM3 S2

For NetCAN Plus device with the IP address 192.168.1.10 and Data Port 2001
invoke (S2 - 50000 bit/s):

    vscandump.exe 192.168.1.10:2001 S2

VSCAN J1939
-----------

Add `vs_can_j1939.h`, `vs_can_j1939.lib` or `vs_can_j1939.so` from this
[archive](http://www.vscom.de/download/multiio/Windows7/driver/VSCAN_J1939.zip)
to your project directory i.e. library search path and invoke:

1. `cmake -B build -DJ1939_EXAMPLES=ON`
2. `cmake --build build`

### Linux

For USB-CAN Plus devices invoke (2 - 50000 bit/s):

    vs_j1939_dump /dev/ttyUSB0 2

For NetCAN Plus device with the IP address 192.168.1.10 and Data Port 2001
invoke (2 - 50000 bit/s):

    vs_j1939_dump 192.168.1.10:2001 2

### Windows

For USB-CAN Plus devices invoke (2 - 50000 bit/s):

    vs_j1939_dump.exe COM3 2

For NetCAN Plus device with the IP address 192.168.1.10 and Data Port 2001
invoke (2 - 50000 bit/s):

    vs_j1939_dump.exe 192.168.1.10:2001 2
