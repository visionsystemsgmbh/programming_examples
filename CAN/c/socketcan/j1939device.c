#include <errno.h>
#include <inttypes.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <net/if.h>
#include <sys/ioctl.h>
#include <sys/socket.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <unistd.h>

#include <linux/can/j1939.h>

void print_usage()
{
	printf("Usage: j1939device <CAN-Channel>\n"
	       "Example: j1939device slcan0\n");
}

int main (int argc, char *argv[])
{
	int sock = -1;
	int j;
	struct can_frame frame;
	struct ifreq ifr;
	char *ifname;
	int ret = EXIT_FAILURE;
	struct sockaddr_can src;
	int do_broadcast = 1;
	uint8_t dat[8];

	if (argc != 2) {
		print_usage();
		goto error;
	}

	ifname = argv[1];

	/* create CAN socket */
	if((sock = socket(PF_CAN, SOCK_DGRAM, CAN_J1939)) < 0) {
		perror("Error while creating socket");
		goto error;
	}

	if (setsockopt(sock, SOL_SOCKET, SO_BROADCAST,
		       &do_broadcast, sizeof(do_broadcast)) < 0) {
		perror("setsockopt: failed to set broadcast");
		goto error;
	}

	/* get interface index */
	strcpy(ifr.ifr_name, ifname);
	ioctl(sock, SIOCGIFINDEX, &ifr);

	src.can_family = AF_CAN;
	src.can_ifindex = ifr.ifr_ifindex;
	src.can_addr.j1939.name = J1939_NO_NAME;
	src.can_addr.j1939.addr = 0x40;
	src.can_addr.j1939.pgn = 0x0F004;


	/* bind CAN socket to the given interface */
	if(bind(sock, (struct sockaddr *)&src, sizeof(src)) < 0) {
		perror("Error in socket bind");
		goto error;
	}

	j = 20;
	for (;;) {
		dat[0] = j;
		j = j < 30 ? j + 1 : 20;
		/* send CAN data */
		if (send(sock, dat, 1, 0) <= 0) {
			perror("Error sending frame");
			goto error;
		}
		sleep(1);
		//dump_can_frame(&frame);
	}

	ret = EXIT_SUCCESS;

error:
	
	/* close socket */
	if (sock >= 0)
		close(sock);

	return ret;
}
