#!/bin/sh

cd CAN/c/socketcan
cmake -B build -DJ1939_EXAMPLES=ON
cmake --build build 
