#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
CAN device tester.
"""

import argparse
import sys

import serial

VSCAN_OK = b'\r'
VSCAN_KO = b'\x07'


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


def main():
    """main routine."""
    parser = argparse.ArgumentParser(description='VSCAN device tester')
    parser.add_argument("port",
                        action="store",
                        help="Serial port name")
    args = parser.parse_args()
    ser_port = init_serial_port(args.port)
    if not ser_port:
        print("Failed to open serial port")
        sys.exit(1)

    if not close_can_channel(ser_port):
        print("Failed to close the CAN channel")
        print("The port could be opened but this "
              "device doesn't respond to the ASCII protocol")
        sys.exit(1)

    ser_num = get_serial_number(ser_port)
    if not ser_num:
        print("Failed to get the serial number")
        sys.exit(1)

    print(f"Found VSCAN device with the serial number "
          f"{ser_num.decode('ascii')}")

    ser_port.close()


if __name__ == '__main__':
    main()
