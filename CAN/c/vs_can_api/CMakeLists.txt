project (CANExamples C)

cmake_minimum_required (VERSION 3.0)

option(SUPPORT_32_BIT_ON_64_BIT OFF)
option(J1939_EXAMPLES "Build J1939 examples" OFF)

include_directories(BEFORE ${PROJECT_BINARY_DIR})

find_library(
	VS_CAN_API
	NAMES libvs_can_api.a vs_can_api.lib libvs_can_api.so
	HINTS ${PROJECT_SOURCE_DIR}
	DOC "vs_can_api")

if (J1939_EXAMPLES)
	find_library(
		VS_CAN_J1939
		NAMES libvs_can_j1939.a vs_can_j1939.lib libvs_can_j1939.so
		HINTS ${PROJECT_SOURCE_DIR}
		DOC "vs_can_j1939")
endif (J1939_EXAMPLES)

# vscansend.exe
add_executable(vscansend vscansend.c)
if (SUPPORT_32_BIT_ON_64_BIT)
	set_target_properties(
		vscansend PROPERTIES OUTPUT_NAME vscansend
		COMPILE_FLAGS "-m32" LINK_FLAGS "-m32")
endif (SUPPORT_32_BIT_ON_64_BIT)
target_link_libraries (vscansend ${VS_CAN_API})

# vscandump.exe
add_executable(vscandump vscandump.c)
if (SUPPORT_32_BIT_ON_64_BIT)
	set_target_properties(
		vscandump PROPERTIES OUTPUT_NAME vscandump
		COMPILE_FLAGS "-m32" LINK_FLAGS "-m32")
endif (SUPPORT_32_BIT_ON_64_BIT)
target_link_libraries (vscandump ${VS_CAN_API})

if (J1939_EXAMPLES)
	# vs_j1939_dump.exe
	add_executable(vs_j1939_dump vs_j1939_dump.c)
	if (SUPPORT_32_BIT_ON_64_BIT)
		set_target_properties(
			vs_j1939_dump PROPERTIES OUTPUT_NAME vs_j1939_dump
			COMPILE_FLAGS "-m32" LINK_FLAGS "-m32")
	endif (SUPPORT_32_BIT_ON_64_BIT)
	target_link_libraries (
		vs_j1939_dump
		${VS_CAN_API}
		${VS_CAN_J1939}
	)
	# vs_j1939_send.exe
	add_executable(vs_j1939_send vs_j1939_send.c)
	if (SUPPORT_32_BIT_ON_64_BIT)
		set_target_properties(
			vs_j1939_send PROPERTIES OUTPUT_NAME vs_j1939_send
			COMPILE_FLAGS "-m32" LINK_FLAGS "-m32")
	endif (SUPPORT_32_BIT_ON_64_BIT)
	target_link_libraries (
		vs_j1939_send
		${VS_CAN_API}
		${VS_CAN_J1939}
	)
endif (J1939_EXAMPLES)

install(TARGETS vscansend RUNTIME DESTINATION bin)
install(TARGETS vscandump RUNTIME DESTINATION bin)
if (J1939_EXAMPLES)
	install(TARGETS vs_j1939_dump RUNTIME DESTINATION bin)
	install(TARGETS vs_j1939_send RUNTIME DESTINATION bin)
endif (J1939_EXAMPLES)
