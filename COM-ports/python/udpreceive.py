#!/usr/bin/env python
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
Example for receiving serial data over UDP.
"""

import sys
import socket
import argparse


def main():
    """main routine."""

    parser = argparse.ArgumentParser(description='UDP receive')
    parser.add_argument("-r", "--receive",
                        type=int,
                        help="Receive UDP port",
                        default=2002)
    args = parser.parse_args()

    try:
        sock = socket.socket(socket.AF_INET,
                             socket.SOCK_DGRAM,
                             socket.IPPROTO_UDP)
        sock.bind(('0.0.0.0', args.receive))
        while True:
            data, addr = sock.recvfrom(1024)
            print('Received message from {}'.format(addr))
            print('Data: {}'.format(data))
    except IOError as err:
        print("Failed to receive UDP frame")
        print(err)
        sys.exit(1)


if __name__ == '__main__':
    main()
