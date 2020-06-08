#include <windows.h>
#include <string>
#include <iostream>
#include <stdlib.h>
#include <time.h>
#include <stdio.h>

HANDLE open_serial_port(char *szPort, char *ser_settings)
{
	HANDLE hComm;
	COMMTIMEOUTS timeouts;
	DCB dcb = {0};
	char szCom[256];

	sprintf(szCom, "\\\\.\\%s", szPort);

	hComm = CreateFile(szCom,  
			GENERIC_READ | GENERIC_WRITE, 
			0, 
			NULL, 
			OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL,
			NULL);

	if (hComm == INVALID_HANDLE_VALUE) {
		printf("ERROR: opening port: err: %d\n", GetLastError());
		return INVALID_HANDLE_VALUE;
	}
	
	GetCommTimeouts(hComm, &timeouts);
	timeouts.ReadIntervalTimeout = 0; 
	timeouts.ReadTotalTimeoutMultiplier = 0;
	timeouts.ReadTotalTimeoutConstant = 0;
	timeouts.WriteTotalTimeoutMultiplier = 0;
	timeouts.WriteTotalTimeoutConstant = 0;

	if (!SetCommTimeouts(hComm, &timeouts)) {
		printf("ERROR: Setting timeouts\n");
		goto ser_error;
	}
	
	if (!GetCommState(hComm, &dcb)) {
		printf("ERROR: Getting settings\n");
		goto ser_error;
	}

	
	FillMemory(&dcb, sizeof(dcb), 0);
	dcb.DCBlength = sizeof(dcb);
	if (!BuildCommDCB(ser_settings, &dcb)) {
		printf("ERROR: Building DCB");
		goto ser_error;
	}
	
	if (!SetCommState(hComm, &dcb)) {
		printf("ERROR: Setting settings\n");
		goto ser_error;
	}
	
	return hComm;

ser_error:
	CloseHandle(hComm);
	return INVALID_HANDLE_VALUE;
}

void print_help()
{
	printf("Usage:\n\n");
	printf("uarttest COMx baudrate,parity,bits,stopbits\n");
	printf("Example: uarttest COM8 115200,n,8,1\n\n");
}

int main(int argc, char *argv[])
{
	HANDLE hComm;
	DWORD bytesWritten;

	if(argc != 3) {
		print_help();
		return -1;
	}


	hComm = open_serial_port(argv[1], argv[2]);
	if (hComm == INVALID_HANDLE_VALUE) {
		return -1;
	}

	if (!WriteFile(hComm, "test", strlen("test"), &bytesWritten, NULL)) {
		printf("ERROR: writing to a COM port: err: %d\n", GetLastError());
		goto ser_error;
	}

	CloseHandle(hComm);

	printf("COM port successfully opened and test string could be sent\n");

	return 0;

ser_error:
	CloseHandle(hComm);
	return -1;
}
