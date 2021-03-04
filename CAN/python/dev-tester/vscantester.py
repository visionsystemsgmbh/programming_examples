#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
CAN device tester.
"""

import argparse
from subprocess import Popen, PIPE
import sys

import serial
import serial.tools.list_ports

VSCAN_OK = b'\r'
VSCAN_KO = b'\x07'


def lsof(port):
    """Check if a port is already open."""
    proc = Popen(["lsof"], stdout=PIPE, stderr=PIPE)
    output = proc.communicate()[0]
    for line in output.decode('ascii').split('\n'):
        if line.find(port) != -1:
            print(f"{port} is already open:\n{line}")


def find_port(port):
    """Find serial port in the list."""
    ports = serial.tools.list_ports.grep(port)
    for item in ports:
        if item.device == port:
            print(f"Serial port found: {item}")
            if item.description.find('USB-CAN Plus') != -1:
                print("This device has a correct description")
            else:
                print(f"Device description is wrong: {item.description}")


def init_serial_port(port):
    """Initialize serial port."""
    ser_port = None
    try:
        ser_port = serial.serial_for_url(port,
                                         baudrate=3000000,
                                         timeout=1,
                                         rtscts=True)
    except serial.serialutil.SerialException as err:
        print(err)
    except BrokenPipeError as err:
        print(err)

    return ser_port


def close_can_channel(port):
    """Send 'C' to close the CAN channle."""
    try:
        port.write("C\r".encode('ascii'))
    except serial.serialutil.SerialException as err:
        print(err)
        return False

    buf = port.read(1)

    if buf != VSCAN_KO and buf != VSCAN_OK:
        return False

    return True


def get_serial_number(port):
    """Send 'N' to get the serial number."""
    ser_num = None
    try:
        port.write("N\r".encode('ascii'))
    except serial.serialutil.SerialException as err:
        print(err)
        return ser_num

    buf = port.read(12)
    if buf[0] != 78:
        print(f"Wrong first character: {buf[0]}")
        return ser_num

    if buf[len(buf) - 1] != 13:
        print(f"Wrong last character: {buf[len(buf) - 1]}")
        return ser_num

    return buf[1:len(buf) - 2]


def get_version_info(port):
    """Send 'V' to get the firmware version."""
    ver = None
    try:
        port.write("V\r".encode('ascii'))
    except serial.serialutil.SerialException as err:
        print(err)
        return ver

    buf = port.read(6)
    if buf[0] != 86:
        print(f"Wrong first character: {buf[0]}")
        return ver

    if buf[len(buf) - 1] != 13:
        print(f"Wrong last character: {buf[len(buf) - 1]}")
        return ver

    return buf[1:len(buf) - 1]


def main():
    """main routine."""
    parser = argparse.ArgumentParser(description='VSCAN device tester')
    parser.add_argument("port",
                        action="store",
                        help="Serial port name")
    args = parser.parse_args()

    if sys.platform.startswith('linux'):
        find_port(args.port)
        lsof(args.port)

    ser_port = init_serial_port(args.port)
    if not ser_port:
        print("Failed to open serial port")
        sys.exit(1)

    if not close_can_channel(ser_port):
        print("Failed to close the CAN channel")
        print("The port could be opened but this "
              "device doesn't respond to the ASCII commands")
        sys.exit(1)

    ser_num = get_serial_number(ser_port)
    if not ser_num:
        print("Failed to get the serial number")
        sys.exit(1)

    ver = get_version_info(ser_port)
    if not ver:
        print("Failed to get the firmware version")
        sys.exit(1)

    ver_major = int(ver[2:3], 16)
    ver_minor = int(ver[3:], 16)
    hw_major = int(ver[:1], 16)
    hw_minor = int(ver[1:2], 16)
    print(f"Found VSCAN device with the following info:")
    print(f"{args.port} -> (SN: {ser_num.decode('ascii')}, "
          f"FW: {ver_major}:{ver_minor}, "
          f"HW: {hw_major}:{hw_minor})")

    ser_port.close()


if __name__ == '__main__':
    main()
