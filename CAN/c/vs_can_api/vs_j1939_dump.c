#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#if defined(__unix__)
#include <unistd.h>
#endif

#include "vs_can_j1939.h"

int ExitPrg = 0;

VSCAN_J1939_ADDR callback()
{
	return 252;
}

void PrintFrame(VSCAN_J1939_MSG *msg, int size)
{
	static FILE *fp = NULL;
	char temp[256];
	int i;

	if (fp == NULL)
		fp = stdout;

	if (fp != NULL)
	{
		for (i = 0; i < size; i++)
		{
			sprintf(temp, "S:%03d,D:%03d,P:%d,PGN:%05d,L:%d,%02X%02X%02X%02X%02X%02X%02X%02X\n",
					  msg->Src,
					  msg->Dst,
					  msg->Priority,
					  msg->PGN,
					  msg->Size,
					  msg->Data[0], msg->Data[1], msg->Data[2], msg->Data[3],
					  msg->Data[4], msg->Data[5], msg->Data[6], msg->Data[7]);

			fputs(temp, fp);
			msg++;
		}
		fflush(fp);
	}
}

long long tobigendian(long long val)
{
	long long big = 0;
	UINT8 *pbig = (UINT8*)&big, *pval = (UINT8*)&val;
	int i;

	for (i = 0; i < 8; i++)
		pbig[i] = pval[7 - i];

	return big;
}

int main (int argc, char *argv[])
{
	VSCAN_HANDLE handle;
	VSCAN_J1939_NAME name;
	VSCAN_J1939_MSG msg[128];
	DWORD param, rv, speed, loop;
	long long count = 1;
	char *channel = argv[1];
	VSCAN_API_VERSION ver_can;
	VSCAN_J1939_VERSION ver_j1939;
	char *help_add = "";

	if (argc != 3)
	{
		printf("Usage: %s <COM Port/IP Address:Port> <Speed-Code>%s\n\n", argv[0], help_add);
		printf("Speed-Code in kb/s:\n1 = 20\n2 = 50\n3 = 100\n4 = 125\n5 = 250\n6 = 500\n7 = 800\n8 = 1000\n"); 
		return -1;
	}

	memset(&name, 0, sizeof(name));
	speed = strtoul(argv[2], NULL, 10);

	if (VSCAN_Ioctl(0, VSCAN_IOCTL_GET_API_VERSION, (VOID*)&ver_can) != VSCAN_ERR_OK)
	{
		printf("Retrieving CAN API version failed\n");
		return -1;
	}
	printf("Version:       1.0\n");
	printf("API Version:   %d.%d.%d\n", ver_can.Major, ver_can.Minor, ver_can.SubMinor);
	ver_j1939 = VSCAN_J1939_ApiVersion();
	printf("J1939 Version: %d.%d.%d\n\n", ver_j1939.Major, ver_j1939.Minor, ver_j1939.SubMinor);
	printf("Source,Destination,Priority,PGN,DataLength,Data\n");

	handle = VSCAN_Open(channel, VSCAN_MODE_NORMAL);
	if (handle <= 0)
	{
		printf("Could not open VSCAN device on %s (handle = %d)\n", channel, handle);
		return -1;
	}

	if (VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_SPEED, (VOID*)speed) != VSCAN_ERR_OK)
	{
		printf("Setting CAN speed failed\n");
		return -1;
	}

	name.IndustryGroup = 0x01;
	name.VehicleSys = 0x01;
	name.Func = 0xff;
	name.ManCode = 0x66;
	rv = VSCAN_J1939_Init(handle, name, callback);
	if (rv != VSCAN_ERR_OK)
	{
		printf("VSCAN_J1939_Init failed (%d)\n", rv);
		return -1;
	}

	do
	{
		rv = VSCAN_J1939_Read_BufferCount(handle);
		if (rv > 0)
		{
			if ((rv = VSCAN_J1939_Read(handle, msg, 128, &param)) != VSCAN_ERR_OK)
				printf("VSCAN_J1939_Read failed (rv = %d)\n", rv);
			PrintFrame(msg, param);
			count += param;
		}
#if defined(__unix__)
		sleep(1);
#else
		Sleep(100);
#endif
	} while (!ExitPrg);

	printf("\nReceived %d frames\n", count);

	VSCAN_J1939_Free(handle);
	VSCAN_Close(handle);

	return 0;
}
