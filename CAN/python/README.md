Python based CAN examples
=========================

Requirements
------------

You'll need a `python-can` module newer than 2.2.1.

    pip install python-can

Usage
-----

### Linux

Replace `CAN_DEV` variable with required device name:

* `/dev/ttyUSB0@3000000` for USB-CAN Plus device
* `socket://192.168.254.254:2001` for NetCAN Plus device

### Windows

Replace `CAN_DEV` variable with required device name:

* `COM32@3000000` for USB-CAN Plus device
* `socket://192.168.254.254:2001` for NetCAN Plus device
