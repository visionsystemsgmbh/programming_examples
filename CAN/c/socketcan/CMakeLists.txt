project (CANExamples C)

cmake_minimum_required (VERSION 3.12)

option(J1939_EXAMPLES "Build J1939 examples" OFF)

# vscansend.exe
add_executable(vscansend vscansend.c)

# vscandump.exe
add_executable(vscandump vscandump.c)

if (J1939_EXAMPLES)
	# j1939device.exe
	add_executable(j1939device j1939device.c)

	# j1939logger.exe
	add_executable(j1939logger j1939logger.c)
endif (J1939_EXAMPLES)

install(TARGETS vscansend RUNTIME DESTINATION bin)
install(TARGETS vscandump RUNTIME DESTINATION bin)
if (J1939_EXAMPLES)
	install(TARGETS j1939device RUNTIME DESTINATION bin)
	install(TARGETS j1939logger RUNTIME DESTINATION bin)
endif (J1939_EXAMPLES)
