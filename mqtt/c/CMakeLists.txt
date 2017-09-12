project(mqtt_gpio C)

cmake_minimum_required(VERSION 2.6)

set(CMAKE_THREAD_PREFER_PTHREAD TRUE)
find_package(Threads REQUIRED)
find_package(PkgConfig REQUIRED)
pkg_check_modules(PC_LIBONRISC REQUIRED libonrisc)

add_executable(mqtt_gpio mqtt_gpio.c)
target_link_libraries(mqtt_gpio ${PC_LIBONRISC_LIBRARIES} paho-mqtt3c ${CMAKE_THREAD_LIBS_INIT})
