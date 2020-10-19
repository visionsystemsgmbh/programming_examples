C Examples for SocketCAN (Linux)
================================

## Requirements

These examples come with a CMake file `CMakeLists.txt`. CMake usage is optional.

## Compilation with CMake

1. `git clone https://github.com/visionsystemsgmbh/programming_examples.git`
2. `cd CAN/c/socketcan`
3. `mkdir build`
4. `cd build`
5. `cmake ..`
6. `make`

## CAN Interface Setup

SocketCAN interfaces will configured via `ip` utility from the `iproute2`
package.

### OnRISC Baltos

On Baltos systems you have an embedded CAN interface called `can0`. To set it to 1Mbit/s bitrate, invoke:

1. `ip link set can0 down`
2. `ip link set can0 type can bitrate 1000000`
3. `ip link set can0 up`

Please note that the interface can be configured in the `down` state only.

### USB-CAN Plus

These devices provide a serial port device i.e. `/dev/ttyUSB0` that has
to be attached to the SocketCAN interface. The following commands set up
USB-CAN Plus to 1Mbit/s bitrate and attach it to the `slcan0` interface:

1. `slcand -o -s8 -t hw -S 3000000 /dev/ttyUSB0`
2. `ip link set up slcan0`

### NetCAN Plus

These devices work over TCP/IP and need a pseudo device that simulates a
serial port i.e. `/dev/netcan0`. Then it can be attached to the
SocketCAN interface. The following commands set up NetCAN Plus to
1Mbit/s bitrate and attach it to the `slcan0` interface:

1. `socat pty,link=/dev/netcan0,raw tcp:192.168.254.254:2001&`
2. `slcand -c -o -s8 /dev/netcan0`
2. `ip link set up slcan0`

## Usage examples

For Baltos invoke:

    ./vscandump can0

For USB-CAN Plus or NetCAN Plus invoke:

    ./vscandump slcan0
