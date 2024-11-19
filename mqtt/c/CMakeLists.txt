cmake_minimum_required(VERSION 3.5)

project(mqtt_gpio C)

set(CMAKE_THREAD_PREFER_PTHREAD TRUE)
find_package(Threads REQUIRED)
find_package(eclipse-paho-mqtt-c REQUIRED)
find_package(PkgConfig REQUIRED)
pkg_check_modules(PC_LIBONRISC REQUIRED libonrisc)

add_executable(mqtt_gpio mqtt_gpio.c)
target_link_libraries(
  mqtt_gpio
  eclipse-paho-mqtt-c::paho-mqtt3c
  ${PC_LIBONRISC_LIBRARIES}
  ${CMAKE_THREAD_LIBS_INIT}
)
