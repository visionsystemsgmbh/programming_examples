project (CANExamples C)

cmake_minimum_required (VERSION 3.0)

include_directories(BEFORE ${PROJECT_BINARY_DIR})

option(SUPPORT_32_BIT_ON_64_BIT OFF)
set(CMAKE_THREAD_PREFER_PTHREAD TRUE)
find_package(Threads REQUIRED)
find_library(
	VS_CAN_API
	NAMES libvs_can_api.a vs_can_api.lib libvs_can_api.so
	HINTS ${PROJECT_SOURCE_DIR}
	DOC "vs_can_api")

# vscansend.exe
add_executable(vscansend vscansend.c)
if (SUPPORT_32_BIT_ON_64_BIT)
	set_target_properties(
		vscansend PROPERTIES OUTPUT_NAME vscansend
		COMPILE_FLAGS "-m32" LINK_FLAGS "-m32")
endif (SUPPORT_32_BIT_ON_64_BIT)
target_link_libraries (vscansend ${VS_CAN_API} ${CMAKE_THREAD_LIBS_INIT})

install(TARGETS vscansend RUNTIME DESTINATION bin)
