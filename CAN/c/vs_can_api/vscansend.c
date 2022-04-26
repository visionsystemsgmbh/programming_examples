#include <stdio.h>
#include <string.h>
#if defined(__unix__)
#include <unistd.h>
#endif

#include "vs_can_api.h"

#define GOTO_ERROR(fmt, ...) do { \
		printf(fmt "\n", ##__VA_ARGS__); \
		goto error; \
	} while(0)

void dump_error_flags(const DWORD flags)
{
	printf("The following errors occurred:\n");
	if (flags & VSCAN_IOCTL_FLAG_RX_FIFO_FULL)
		printf("VSCAN_IOCTL_FLAG_RX_FIFO_FULL\n");
	if (flags & VSCAN_IOCTL_FLAG_TX_FIFO_FULL)
		printf("VSCAN_IOCTL_FLAG_TX_FIFO_FULL\n");
	if (flags & VSCAN_IOCTL_FLAG_ERR_WARNING)
		printf("VSCAN_IOCTL_FLAG_ERR_WARNING\n");
	if (flags & VSCAN_IOCTL_FLAG_DATA_OVERRUN)
		printf("VSCAN_IOCTL_FLAG_DATA_OVERRUN\n");
	if (flags & VSCAN_IOCTL_FLAG_ERR_PASSIVE)
		printf("VSCAN_IOCTL_FLAG_ERR_PASSIVE\n");
	if (flags & VSCAN_IOCTL_FLAG_ARBIT_LOST)
		printf("VSCAN_IOCTL_FLAG_ARBIT_LOST\n");
	if (flags & VSCAN_IOCTL_FLAG_BUS_ERROR)
		printf("VSCAN_IOCTL_FLAG_BUS_ERROR\n");
	if (flags & VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL)
		printf("VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL\n");
}

int main (int argc, char *argv[])
{
	char *tty;
	VSCAN_MSG msg;
	VSCAN_HANDLE h = -1;
	VSCAN_API_VERSION version;
	DWORD rv;
	DWORD bitrate;
	DWORD flags;

	if (VSCAN_Ioctl(0, VSCAN_IOCTL_GET_API_VERSION, &version))
		GOTO_ERROR("Failed to get API version");
	
	printf("VSCAN-API Version %d.%d.%d\n", version.Major, version.Minor, version.SubMinor);

	if (argc != 3) 
		GOTO_ERROR("Usage: vscansend.exe <CAN-Channel> <CAN-Bitrate>\n"
			   "Example: vscansend.exe COM3 S2");

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
	if (VSCAN_Ioctl(h, VSCAN_IOCTL_SET_SPEED, (void*)bitrate) != VSCAN_ERR_OK)
		GOTO_ERROR("Setting CAN speed failed");

	/* send standard frame */
	msg.Id = 0x100;
	msg.Size = 4;
	msg.Flags = VSCAN_FLAGS_STANDARD;
	msg.Data[0] = 0x00;
	msg.Data[1] = 0x01;
	msg.Data[2] = 0x02;
	msg.Data[3] = 0x03;

	if (VSCAN_Write(h, &msg, 1, &rv) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Write failed");
	if (VSCAN_Flush(h) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Flush failed");

	/* send extended frame */
	msg.Id = 0x100;
	msg.Size = 4;
	msg.Flags = VSCAN_FLAGS_EXTENDED;
	msg.Data[0] = 0x00;
	msg.Data[1] = 0x01;
	msg.Data[2] = 0x02;
	msg.Data[3] = 0x03;

	if (VSCAN_Write(h, &msg, 1, &rv) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Write failed");
	if (VSCAN_Flush(h) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Flush failed");

	/* send RTR frame */
	msg.Id = 0x100;
	msg.Size = 0;
	msg.Flags = VSCAN_FLAGS_REMOTE;

	if (VSCAN_Write(h, &msg, 1, &rv) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Write failed");
	if (VSCAN_Flush(h) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Flush failed");

	if (VSCAN_Ioctl(h, VSCAN_IOCTL_GET_FLAGS, &flags) != VSCAN_ERR_OK)
		GOTO_ERROR("VSCAN_Ioctl VSCAN_IOCTL_GET_FLAGS failed");

	/* check CAN error flags */
	if (flags)
		dump_error_flags(flags);

#if defined(__unix__)
	sleep(1);
#else
	Sleep(1000);
#endif

	VSCAN_Close(h);
	return 0;
error:
	if (h > 0)
		VSCAN_Close(h);

	return -1;
}
