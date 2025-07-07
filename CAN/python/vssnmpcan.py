#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

""" SNMP CLI utility for NetCAN Plus devices."""

import argparse
import sys

from pysnmp.hlapi import (CommunityData, ContextData, Integer, ObjectIdentity,
                          ObjectType, OctetString, SnmpEngine,
                          UdpTransportTarget, setCmd)

SNMP_PORT_MODE = ".1.3.6.1.4.1.12695.1.101.1.7.1"
SNMP_BRIDGE_SERVER = 64
SNMP_BRIDGE_CLIENT = 64
SNMP_CTRL = ".1.3.6.1.4.1.12695.1.10.0"
SNMP_CTRL_SAVE = 2
SNMP_BRIDGE_CLIENT_IP = ".1.3.6.1.4.1.12695.1.103.1.3.1.1"
SNMP_BRIDGE_CLIENT_PORT = ".1.3.6.1.4.1.12695.1.103.1.5.1.1"
SNMP_BRIDGE_CAN_SPEED = ".1.3.6.1.4.1.12695.1.101.1.60.1"


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


class SnmpManager(object):
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
                   UdpTransportTarget((self.ip_addr, 161)),
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


def setup_bridge_server(ip_addr, bitrate):
    """Set up CAN Bridge Server."""

    print("Set up CAN Bridge Server")
    snmp_mgr = SnmpManager(ip_addr)

    snmp_mgr.write(SNMP_PORT_MODE, SNMP_BRIDGE_SERVER)
    snmp_mgr.write(SNMP_BRIDGE_CLIENT_IP, "")
    snmp_mgr.write(SNMP_BRIDGE_CLIENT_PORT, 0)
    snmp_mgr.write(SNMP_BRIDGE_CAN_SPEED, bitrate)
    snmp_mgr.write(SNMP_CTRL, SNMP_CTRL_SAVE)


def setup_bridge_client(ip_addr, srv_ip_addr, srv_tcp_port, bitrate):
    """Set up CAN Bridge Client."""

    print("Set up CAN Bridge Client")
    snmp_mgr = SnmpManager(ip_addr)

    snmp_mgr.write(SNMP_PORT_MODE, SNMP_BRIDGE_CLIENT)
    snmp_mgr.write(SNMP_BRIDGE_CLIENT_IP, srv_ip_addr)
    snmp_mgr.write(SNMP_BRIDGE_CLIENT_PORT, srv_tcp_port)
    snmp_mgr.write(SNMP_BRIDGE_CAN_SPEED, bitrate)
    snmp_mgr.write(SNMP_CTRL, SNMP_CTRL_SAVE)


def main():
    """Main routine."""
    print("NetCAN Plus SNMP Bridge Configurator")
    parser = argparse.ArgumentParser(
        description="NetCAN Plus SNMP Bridge Configurator")
    parser.add_argument("-s", "--server",
                        help="IP address of the NetCAN Plus bridge server",
                        default=None)
    parser.add_argument("-c", "--client",
                        help="IP address of the NetCAN Plus bridge client",
                        default=None)
    parser.add_argument("-p", "--port",
                        help="TCP port for the NetCAN Plus bridge client",
                        type=int,
                        default=2001)
    parser.add_argument("--bs",
                        help="Bitrate for the NetCAN Plus bridge server",
                        type=int,
                        default=20000)
    parser.add_argument("--bc",
                        help="Bitrate for the NetCAN Plus bridge client",
                        type=int,
                        default=20000)
    args = parser.parse_args()

    try:
        setup_bridge_server(args.server, args.bs)

        if args.client:
            setup_bridge_client(args.client, args.server, args.port, args.bc)
    except VsSnmpError as err:
        print(err)
        sys.exit(-1)

    print("Setup successful")


if __name__ == "__main__":
    main()
