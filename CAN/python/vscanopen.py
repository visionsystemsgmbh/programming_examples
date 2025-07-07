#!/usr/bin/env python3
# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

"""
Example for reading/writing CANopen objects.
"""

import canopen

CAN_DEV = '/dev/ttyUSB0@3000000'


def main():
    """main routine."""

    print("Starting CANopen master")
    try:
        network = canopen.Network()
        network.connect(bustype='slcan',
                        channel=CAN_DEV,
                        rtscts=True,
                        bitrate=1000000)
        network.check()

        # create and add a remote node using an EDS file
        node = canopen.RemoteNode(4, '/path/to/object_dictionary.eds')
        network.add_node(node)

        # read a variable using SDO
        val = node.sdo[0x1017].raw
        print(val)

        # change a variable using SDO
        node.sdo[0x1017].raw = 4000

        # reread the modified variable
        val = node.sdo[0x1017].raw
        print(val)

    except Exception as err:
        print("Error occurred: {}".format(err))
    finally:
        network.sync.stop()
        network.disconnect()


if __name__ == '__main__':
    main()
