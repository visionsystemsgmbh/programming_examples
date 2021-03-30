VSCAN Device Tester
===================

This is a small utility that checks whether a device behind a given
serial port is a VSCAN device.

Requirements
------------

The script requires Python 3.6 or later, PySerial and netifaces modules.

    pip3 install pyserial netifaces

Usage
-----

    python vscantester.py /dev/ttyUSB0

or

    python vscantester.py /dev/COM5

If the device is a correctly functioning VSCAN device, you'll get its
serial number otherwise, the program will return an error message
saying what went wrong.

You can also use `vscantester.py` to search for available VSCAN devices:

    python vscantester.py

If the script fails to find any device, it will check for the presence of the
`ftdi_sio.ko` driver in the system in Linux.

To search for the NetCAN Plus devices, invoke (under Windows you'll be asked
to allow this script to listen on a special UDP port):

    python vscantester.py -u

On Linux you can get the system information showing the kernel version and driver
related information:

    python vscantester.py -s

Convert the Script to an Executable in Windows (Experimental)
----------------------------------------------

You can convert the script to a stand-alone executable using PyInstaller:

    pip3 install pyserial netifaces pyinstaller
    pyinstaller --onefile vscantester.py

The `exe` can be found in the `dist` folder.
