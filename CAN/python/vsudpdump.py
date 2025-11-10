#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
Example for receiing CAN frames via ASCII protocol (UDP).
"""

import sys
import socket
import argparse


def setup_can_channel(ip_port, ip_dest, speed):
    """Setup CAN speed and open the channel."""
    buf = f"C\rS{speed}\rO\r"
    send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
    send_sock.sendto(bytes(buf, "ascii"), (ip_dest, ip_port))


def main():
    """main routine."""
    parser = argparse.ArgumentParser(description="VSCAN UDP dump")
    parser.add_argument(
        "-r", "--receive", type=int, help="Receive UDP port", default=2002
    )
    parser.add_argument("-s", "--send", type=int, help="Send UDP port", default=2002)
    parser.add_argument("-a", "--address", help="NetCAN Plus IP address", default=None)
    parser.add_argument(
        "-b",
        "--bitrate",
        type=int,
        help="CAN bitrate(2 - 20000b/s till 8 - 1Mbit/s)",
        default=8,
    )
    args = parser.parse_args()

    try:
        setup_can_channel(args.send, args.address, args.bitrate)
    except IOError as err:
        print(f"Failed to setup CAN channel: {err}")
        sys.exit(1)

    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
        sock.bind(("0.0.0.0", args.receive))
        while True:
            data, addr = sock.recvfrom(1024)
            print(data.decode("ascii"))
    except IOError as err:
        print(f"Failed to receive CAN frame: {err}")
        sys.exit(1)


if __name__ == "__main__":
    main()
