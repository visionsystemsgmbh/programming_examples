#!/usr/bin/env python
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
Example for sending serial data over UDP.
"""

import sys
import time
import socket
import argparse


def main():
    """main routine."""

    parser = argparse.ArgumentParser(description='UDP receive')
    parser.add_argument("-p", "--port",
                        type=int,
                        help="UDP port for data transmission",
                        default=2002)
    parser.add_argument("address",
                        action="store",
                        help="Destination IP address")
    args = parser.parse_args()

    remote_address = (args.address, args.port)
    try:
        sock = socket.socket(socket.AF_INET,
                             socket.SOCK_DGRAM,
                             socket.IPPROTO_UDP)
        while True:
            sock.sendto(b"test", remote_address)
            time.sleep(1)
    except IOError as err:
        print("Failed to send UDP frame")
        print(err)
        sys.exit(1)


if __name__ == '__main__':
    main()
