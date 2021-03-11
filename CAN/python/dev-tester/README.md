VSCAN Device Tester
===================

This is a small utility that checks whether a device behind a given
serial port is a VSCAN device.

Requirements
------------

The script requires Python 3.6 or later and a PySerial module.

    pip3 install pyserial

Usage
-----

    python vscantester.py /dev/ttyUSB0

or

    python vscantester.py /dev/COM5

If the device is a correctly functioning VSCAN device, you'll get its
serial number otherwise, the program will return an error message
saying what went wrong.

You can also use `vscantester.py` to search for available VSCAN devices:

    python vscantester.py all
