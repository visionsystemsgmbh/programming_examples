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


class UsbCan(object):
    """USB-CAN Plus class."""

    def __init__(self, port):
        self.port = port
        self.ser_port = None

    def close(self):
        """Close serial port."""
        self.ser_port.close()

    def init_serial_port(self):
        """Initialize serial port."""
        ret = True
        try:
            self.ser_port = serial.serial_for_url(self.port,
                                                  baudrate=3000000,
                                                  timeout=1,
                                                  rtscts=True)
        except serial.serialutil.SerialException as err:
            print(err)
            ret = False
        except BrokenPipeError as err:
            print(err)
            ret = False

        return ret

    def close_can_channel(self):
        """Send 'C' to close the CAN channel."""
        try:
            self.ser_port.write("C\r".encode('ascii'))
        except serial.serialutil.SerialException as err:
            print(err)
            return False

        buf = self.ser_port.read(1)

        if buf != VSCAN_KO and buf != VSCAN_OK:
            return False

        return True

    def lsof(self):
        """Check if a port is already open."""
        proc = Popen(["lsof"], stdout=PIPE, stderr=PIPE)
        output = proc.communicate()[0]
        for line in output.decode('ascii').split('\n'):
            if line.find(self.port) != -1:
                print(f"{self.port} is already open:\n{line}")

    def get_serial_number(self):
        """Send 'N' to get the serial number."""
        ser_num = None
        try:
            self.ser_port.write("N\r".encode('ascii'))
        except serial.serialutil.SerialException as err:
            print(err)
            return ser_num

        buf = self.ser_port.read(12)
        if buf[0] != 78:
            print(f"Wrong first character: {buf[0]}")
            return ser_num

        if buf[len(buf) - 1] != 13:
            print(f"Wrong last character: {buf[len(buf) - 1]}")
            return ser_num

        return buf[1:len(buf) - 2]

    def get_version_info(self):
        """Send 'V' to get the firmware version."""
        ver = None
        try:
            self.ser_port.write("V\r".encode('ascii'))
        except serial.serialutil.SerialException as err:
            print(err)
            return ver

        buf = self.ser_port.read(6)
        if buf[0] != 86:
            print(f"Wrong first character: {buf[0]}")
            return ver

        if buf[len(buf) - 1] != 13:
            print(f"Wrong last character: {buf[len(buf) - 1]}")
            return ver

        return buf[1:len(buf) - 1]


def find_all_usb_can_devices():
    """Find all serial ports with FT-X chip."""
    port_list = []
    ports = serial.tools.list_ports.grep("0403:6015")
    for item in ports:
        port_list.append(item.device)

    return port_list


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


def main():
    """main routine."""
    parser = argparse.ArgumentParser(description='VSCAN device tester')
    parser.add_argument("port",
                        action="store",
                        help="Serial port name")
    args = parser.parse_args()

    port_list = []
    if args.port == 'all':
        port_list = find_all_usb_can_devices()
        if not port_list:
            print("No USB-CAN devices found")
    else:
        port_list.append(args.port)

    for item in port_list:
        usbcan = UsbCan(item)
        if sys.platform.startswith('linux'):
            find_port(usbcan.port)
            usbcan.lsof()

        if not usbcan.init_serial_port():
            print("Failed to open serial port")
            sys.exit(1)

        if not usbcan.close_can_channel():
            print("Failed to close the CAN channel")
            print("The port could be opened but this "
                  "device doesn't respond to the ASCII commands")
            sys.exit(1)

        ser_num = usbcan.get_serial_number()
        if not ser_num:
            print("Failed to get the serial number")
            sys.exit(1)

        ver = usbcan.get_version_info()
        if not ver:
            print("Failed to get the firmware version")
            sys.exit(1)

        ver_major = int(ver[2:3], 16)
        ver_minor = int(ver[3:], 16)
        hw_major = int(ver[:1], 16)
        hw_minor = int(ver[1:2], 16)
        print(f"Found VSCAN device with the following info:")
        print(f"{usbcan.port} -> (SN: {ser_num.decode('ascii')}, "
              f"FW: {ver_major}:{ver_minor}, "
              f"HW: {hw_major}:{hw_minor})")

        usbcan.close()


if __name__ == '__main__':
    main()
