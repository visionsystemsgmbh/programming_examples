#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>

#include <net/if.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/ioctl.h>

#include <linux/can.h>
#include <linux/can/raw.h>

void print_usage()
{
	printf("Usage: vscandump <CAN-Channel>\n"
	       "Example: vscandump slcan0\n");
}

void dump_can_frame(struct can_frame *frame)
{
	int i;

	if (frame->can_id & CAN_EFF_FLAG)
		printf("%08X   ", frame->can_id);
	else
		printf("%03X   ", frame->can_id);

	printf("[%d]   ", frame->can_dlc);
	for (i = 0; i < frame->can_dlc; i++) {
		printf("%02X ", frame->data[i]);
	}
	printf("\n");
}

int main (int argc, char *argv[])
{
	int s = -1;
	int nbytes;
	struct sockaddr_can addr;
	struct can_frame frame;
	struct ifreq ifr;
	char *ifname;
	int ret = EXIT_FAILURE;

	if (argc != 2) {
		print_usage();
		goto error;
	}

	ifname = argv[1];

	/* create CAN socket */
	if((s = socket(PF_CAN, SOCK_RAW, CAN_RAW)) < 0) {
		perror("Error while creating socket");
		goto error;
	}

	/* get interface index */
	strcpy(ifr.ifr_name, ifname);
	ioctl(s, SIOCGIFINDEX, &ifr);
	
	addr.can_family  = AF_CAN;
	addr.can_ifindex = ifr.ifr_ifindex;

	/* bind CAN socket to the given interface */
	if(bind(s, (struct sockaddr *)&addr, sizeof(addr)) < 0) {
		perror("Error in socket bind");
		goto error;
	}

	for (;;) {
		/* read CAN frame */
		if (read(s, &frame, sizeof(struct can_frame)) <= 0) {
			perror("Error reading CAN frame");
			goto error;
		}
		dump_can_frame(&frame);
	}

	ret = EXIT_SUCCESS;

error:
	/* close socket */
	if (s >= 0)
		close(s);

	return ret;
}
