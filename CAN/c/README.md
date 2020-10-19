C Examples for VSCAN API
========================

Requirements
------------

These examples come with a CMake file `CMakeLists.txt`. CMake usage is optional.

You'll also need vs_can_api library files. Either put them into this folder
or anywhere else where CMake or the IDE of your choice will be able to find
them.

Compilation
-----------

### Linux

1. `git clone https://github.com/visionsystemsgmbh/programming_examples.git`
2. `cd CAN/c`
3. `mkdir build`
4. `cd build`
5. `cmake ..`
6. `make`

### Windows (Microsoft Visual Studio 2008)

1.  `git clone https://github.com/visionsystemsgmbh/programming_examples.git`
2. `cd CAN/c`
3. `mkdir build`
4. `cd build`
5. `cmake .. -G "Visual Studio 9 2008"`
6. open generated solution file

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
