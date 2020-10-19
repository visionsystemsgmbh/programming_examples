project (CANExamples C)

cmake_minimum_required (VERSION 3.0)

# vscansend.exe
add_executable(vscansend vscansend.c)

# vscandump.exe
add_executable(vscandump vscandump.c)

install(TARGETS vscansend RUNTIME DESTINATION bin)
install(TARGETS vscandump RUNTIME DESTINATION bin)
