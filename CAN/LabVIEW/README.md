LabVIEW Example for VSCAN API
=============================

Requirements
------------

LabVIEW example was made with LabVIEW 8.6.1. Hence you'll need LabVIEW 8.6.1 or
newer.

Wrapper
-------
`CanRead.vi`, `CanWrite.vi` and `OpenChannel.vi` files provide wrapper VI's
for `vs_can_api.dll` calls. The detailed description can be found in the VSCAN
User Manual.

Example
-------
`MainPanel.vi` file provides a working example showing, how to send/receive CAN
frames using the above mentioned wrapper VI's. The detailed description can be
found in the VSCAN User Manual.
