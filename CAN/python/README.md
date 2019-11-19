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

CANopen Example
---------------

CANopen example uses
[canopen](https://github.com/christiansandberg/canopen)
package that uses python-can package for CAN communication. Hence the
same device parameters also apply to CANopen.

    pip install canopen

This example shows how a CANopen master can access a CANopen node and
read/modify its variables. You will need an ESD file describing your
node, and a path to this file must be passed to the RemoteNode
constructor.

This example was tested against a CANopen node from
[CANopenSocket](https://github.com/CANopenNode/CANopenSocket) project.
