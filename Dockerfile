FROM ubuntu:20.04

ENV DEBIAN_FRONTEND noninteractive

RUN apt update && \
    apt install -y build-essential rsync unzip cmake git bc python file cpio texinfo \
       lzop wget libssl-dev libncurses5-dev gawk zlib1g-dev pkg-config u-boot-tools \
       dosfstools libconfuse-dev mtools swig gettext subversion && \
    apt clean

RUN dpkg --add-architecture i386 && \
    apt update && \
    apt install -y libc6-dev:i386

CMD ["/bin/bash"]
