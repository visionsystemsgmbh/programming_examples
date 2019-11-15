#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "vs_can_api.h"

#define GOTO_ERROR(fmt, ...) do { \
		printf(fmt "\n", ##__VA_ARGS__); \
		goto error; \
	} while(0)

void dump_frame(VSCAN_MSG *frame)
{
	int i;

	if (frame->Flags & VSCAN_FLAGS_STANDARD)
		printf("%03X   ", frame->Id);
	else if(frame->Flags & VSCAN_FLAGS_EXTENDED)
		printf("%08X   ", frame->Id);

	printf("[%d]   ", frame->Size);
	for (i = 0; i < frame->Size; i++) {
		printf("%02X ", frame->Data[i]);
	}
	printf("\n");
}

int main (int argc, char *argv[])
{
	char *tty;
	VSCAN_MSG msg;
	VSCAN_HANDLE h = -1;
	VSCAN_API_VERSION version;
	DWORD rv;
	DWORD bitrate;

	if (VSCAN_Ioctl(0, VSCAN_IOCTL_GET_API_VERSION, &version))
		GOTO_ERROR("Failed to get API version");
	
	printf("VSCAN-API Version %d.%d.%d\n", version.Major, version.Minor, version.SubMinor);

	if (argc != 3) 
		GOTO_ERROR("Usage: vscandump.exe <CAN-Channel> <CAN-Bitrate>\n"
			   "Example: vscandump.exe COM3 S2");

	tty = argv[1];

	/* handle bitrate parameter */
	if (strlen(argv[2]) > 8)
		GOTO_ERROR("CAN bitrate must look like 'Sxxx', where x can be an index\n"
			   "from 1 to 8 or bitrate as a number like 125000");
	if (sscanf(argv[2], "S%7ld", &bitrate) != 1)
		GOTO_ERROR("Failed to parse CAN bitrate");
	if (bitrate < 1 || bitrate > 1000000)
		GOTO_ERROR("Wrong bitrate %ld\n", bitrate);

	h = VSCAN_Open(tty, VSCAN_MODE_NORMAL);
	if (h <= 0)
		GOTO_ERROR("VSCAN_Open failed for %s with %d", tty, h);
	if (VSCAN_Ioctl(h, VSCAN_IOCTL_SET_BLOCKING_READ, VSCAN_IOCTL_ON) != VSCAN_ERR_OK)
		GOTO_ERROR("Set blocking read failed");
	if (VSCAN_Ioctl(h, VSCAN_IOCTL_SET_SPEED, (void*)bitrate) != VSCAN_ERR_OK)
		GOTO_ERROR("Setting CAN speed failed");

	for (;;)
	{
		if (VSCAN_Read(h, &msg, 1, &rv) != VSCAN_ERR_OK)
			GOTO_ERROR("VSCAN_Read failed");
		dump_frame(&msg);
		
	}
	VSCAN_Close(h);
	return 0;
error:
	if (h > 0)
		VSCAN_Close(h);

	return -1;
}
