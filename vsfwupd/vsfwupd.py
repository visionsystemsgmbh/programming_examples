#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

""" SNMP CLI utility for updating NetCom/NetCAN Plus devices."""

import argparse
import pathlib
import socket
import sys
import zipfile

from pysnmp.hlapi import (CommunityData, ContextData, Integer, ObjectIdentity,
                          ObjectType, OctetString, SnmpEngine,
                          UdpTransportTarget, getCmd, setCmd)

SNMP_CTRL = ".1.3.6.1.4.1.12695.1.10.0"
SNMP_MODEL = ".1.3.6.1.4.1.12695.1.11.0"
SNMP_FW_VER = ".1.3.6.1.4.1.12695.1.2.0"
SNMP_CTRL_START_UPDATE = 3
SNMP_CTRL_FINISH_UPDATE = 4


class VsSnmpError(Exception):
    """Internal SNMP exception class."""
    def __init__(self, message):
        self.message = message
        super().__init__(self.message)

    def __str__(self):
        message = 'VsSnmpError has been raised'
        if self.message:
            message = f'VsSnmpError: {self.message}'

        return message


class SnmpManager():
    """SNMP Manager Class."""

    def __init__(self, ip_addr, verbose=False):
        self.ip_addr = ip_addr
        self.verbose = verbose

    def write(self, oid, value):
        """Write value."""
        error_str = ''
        if isinstance(value, int):
            real_val = Integer(value)
        else:
            real_val = OctetString(value)
        error_indication, error_status, error_index, var_binds = next(
            setCmd(SnmpEngine(),
                   CommunityData('root', mpModel=0),
                   UdpTransportTarget((self.ip_addr, 161), timeout=120),
                   ContextData(),
                   ObjectType(ObjectIdentity(oid), real_val))
        )

        if error_indication:
            error_str = error_indication
        elif error_status:
            error_str = ('%s at %s' % (
                error_status.prettyPrint(),
                error_index and var_binds[int(error_index) - 1][0] or '?'))
        else:
            for item in var_binds:
                if self.verbose:
                    print(item)

        if error_str:
            raise VsSnmpError(error_str)

    def read(self, oid):
        """Read value."""
        error_str = ''
        error_indication, error_status, error_index, var_binds = next(
            getCmd(SnmpEngine(),
                   CommunityData('root', mpModel=0),
                   UdpTransportTarget((self.ip_addr, 161), timeout=120),
                   ContextData(),
                   ObjectType(ObjectIdentity(oid)))
        )

        if error_indication:
            error_str = error_indication
        elif error_status:
            error_str = ('%s at %s' % (
                error_status.prettyPrint(),
                error_index and var_binds[int(error_index) - 1][0] or '?'))
        else:
            for item in var_binds:
                if self.verbose:
                    print(item)

        if error_str:
            raise VsSnmpError(error_str)

        return var_binds[0][1]


def perform_fw_update(ip_addr, data):
    """Perform firmware update."""
    snmp_mgr = SnmpManager(ip_addr)

    # get model
    model = snmp_mgr.read(SNMP_MODEL)
    model_str = "NetCom"
    if '110' in str(model):
        model_str = "NetCAN"

    # get version
    fw_ver = int(snmp_mgr.read(SNMP_FW_VER))
    fw_ver_patch = fw_ver & 0xff
    fw_ver_minor = (fw_ver >> 8) & 0xff
    fw_ver_major = (fw_ver >> 16) & 0xff

    # show device information
    print(f"The following device was detected: {model_str} {model} with "
          f"firmware version {fw_ver_major}.{fw_ver_minor}.{fw_ver_patch}")

    print("Starting firmware update")
    snmp_mgr.write(SNMP_CTRL, SNMP_CTRL_START_UPDATE)

    print("Transferring firmware file")
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.connect((ip_addr, 2400))
        sock.sendall(data)

    print("Flashing firmware")
    snmp_mgr.write(SNMP_CTRL, SNMP_CTRL_FINISH_UPDATE)

    print("Firmware successfully updated")


def main():
    """Main routine."""
    print("NetCom/NetCAN Plus firmware update utility")
    parser = argparse.ArgumentParser(
        description="NetCom/NetCAN Plus firmware update utility")
    parser.add_argument("-f", "--firmware",
                        help="Update firmware (either *.b64 or *.zip file)",
                        required=True,
                        default=None)
    parser.add_argument('ip_addr', action='store', help='Device IP address')
    args = parser.parse_args()

    if (pathlib.Path(args.firmware).suffix != ".b64"
            and not zipfile.is_zipfile(args.firmware)):
        print(f"{args.firmware} is not a ZIP or *.b64 file")
        sys.exit(-1)

    fw_buf = None
    if zipfile.is_zipfile(args.firmware):
        with zipfile.ZipFile(args.firmware, 'r') as zfile:
            for item in zfile.namelist():
                if "b64" in item:
                    fw_buf = zfile.read(item)
                    break
    else:
        with open(args.firmware, 'rb') as fw_file:
            fw_buf = fw_file.read()

    try:
        perform_fw_update(args.ip_addr, fw_buf)
    except VsSnmpError as err:
        print(err)
        sys.exit(-1)
    except Exception as err:
        print(err)
        sys.exit(-1)


if __name__ == "__main__":
    main()
