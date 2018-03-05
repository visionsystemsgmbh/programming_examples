#!/usr/bin/python

# vim: tabstop=8 expandtab shiftwidth=4 softtabstop=4

import sys

from pymodbus.client.sync import ModbusTcpClient

client = ModbusTcpClient(sys.argv[1])

# setup direction of all IOs to input
client.write_register(2, 0x0, unit=0x01)

# read inputs
result = client.read_coils(0, 4, unit=0x01)

for i in range(0, 4):
    print("Bit {}: {}".format(i, result.bits[i]))

# setup direction of all IOs to output
client.write_register(2, 0xf, unit=0x01)

# set pins 0 and 3 to high
client.write_register(0, 0x9, unit=0x01)

# read outputs
result = client.read_coils(0, 4, unit=0x01)

for i in range(0, 4):
    print("Bit {}: {}".format(i, result.bits[i]))

client.close()
