#!/usr/bin/env python
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
Example for sending CAN frames via ASCII protocol (slcan).
"""

import sys
import time

import can
import serial

CAN_DEV = '/dev/ttyUSB0@3000000'


def main():
    """main routine."""
    try:
        bus = can.interface.Bus(bustype='slcan',
                                channel=CAN_DEV,
                                rtscts=True,
                                bitrate=1000000)
    except serial.serialutil.SerialException as err:
        print(err)
        sys.exit(1)

    # send standard frame
    msg = can.Message(arbitration_id=0x100,
                      extended_id=False,
                      data=[0x00, 0x01, 0x02, 0x03])
    bus.send(msg)

    # send extended frame
    msg = can.Message(arbitration_id=0x100,
                      extended_id=True,
                      data=[0x00, 0x01, 0x02, 0x03])
    bus.send(msg)

    # send RTR frame
    msg = can.Message(arbitration_id=0x100,
                      extended_id=True,
                      is_remote_frame=True)
    bus.send(msg)

    time.sleep(5)

    bus.shutdown()


if __name__ == '__main__':
    main()
