unit vs_can_api;

(*!
 * \version
 * 1.6
 *
 * \brief
 * VSCAN API implementation (header file)
 *
 * This is the implementation of the VSCAN API - a wrapper for the specialized ASCII command set.
 * The library is available for Windows and Linux.
 *
 * Copyright (c) 2011 by VScom
 *)

interface

//#include <semaphore.h>
const
    VSCAN_DLL = 'vs_can_api.dll';

type
    VOID   =  Char;
    pVOID  = pChar;
    DWORD  =  Cardinal;
    pDWORD = ^DWORD;
    UCHAR  =  Byte;
    USHORT =  Word;
    UINT   =  Cardinal;
    ULONG  =  Cardinal;

    UINT8  =  UCHAR;
    UINT16 =  USHORT;
    UINT32 =  UINT;

    int    =  Integer;

    VSCAN_HANDLE  = int;
    VSCAN_STATUS  = int;

// Hardware Parameter Structure
   pVSCAN_HWPARAM = ^tVSCAN_HWPARAM;
   tVSCAN_HWPARAM = Record
     SerialNr:  UINT32;
           HwVersion: UINT8;
     SwVersion: UINT8;
     HwType:    UINT8;
     end;

// Message Structure
   pVSCAN_MSG =^tVSCAN_MSG;
   tVSCAN_MSG = Record
     Id:        UINT32;
     Size:      UINT8;
     Data:      Array[0..7]of UINT8 ;
     Flags:     UINT8 ;
     Timestamp: UINT16 ;
     end;

// Bit Timing Register Structure
   pVSCAN_BTR =^tVSCAN_BTR;
   tVSCAN_BTR = Record
     Btr0: UINT8 ;
     Btr1: UINT8 ;
     end;

// Acceptance Code and Mask Structure
   pVSCAN_CODE_MASK =^tVSCAN_CODE_MASK;
   tVSCAN_CODE_MASK = Record
     Code: UINT32 ;
     Mask: UINT32 ;
     end;

// API Version Structure
   pVSCAN_API_VERSION =^tVSCAN_API_VERSION;
   tVSCAN_API_VERSION = Record
     Major:    UINT8 ;
     Minor:    UINT8 ;
     SubMinor: UINT8 ;
     end;


const
    NULL               = Pointer(0);
    VSCAN_FIRST_FOUND  = NULL;

// Debug Mode
    VSCAN_DEBUG_MODE_CONSOLE = pVOID(1);
    VSCAN_DEBUG_MODE_FILE    = pVOID(2);
// Debug Level
    VSCAN_DEBUG_NONE         = pVOID(0);
    VSCAN_DEBUG_LOW          = pVOID(-1);
    VSCAN_DEBUG_MID          = pVOID(-51);
    VSCAN_DEBUG_HIGH         = pVOID(-101);

// Status / Errors
    VSCAN_ERR_OK                =        0;
    VSCAN_ERR_ERR               = int(VSCAN_DEBUG_LOW);                // Debug Level Low
    VSCAN_ERR_NO_DEVICE_FOUND   = int(VSCAN_DEBUG_LOW - 1);
    VSCAN_ERR_SUBAPI            = int(VSCAN_DEBUG_LOW - 2);
    VSCAN_ERR_NOT_ENOUGH_MEMORY = int(VSCAN_DEBUG_LOW - 3);
    VSCAN_ERR_NO_ELEMENT_FOUND  = int(VSCAN_DEBUG_LOW - 4);
    VSCAN_ERR_INVALID_HANDLE    = int(VSCAN_DEBUG_LOW - 5);
    VSCAN_ERR_IOCTL             = int(VSCAN_DEBUG_LOW - 6);
    VSCAN_ERR_MUTEX             = int(VSCAN_DEBUG_LOW - 7);
    VSCAN_ERR_CMD               = int(VSCAN_DEBUG_LOW - 8);
    VSCAN_ERR_LISTEN_ONLY       = int(VSCAN_DEBUG_LOW - 9);
    VSCAN_ERR_NOT_SUPPORTED     = int(VSCAN_DEBUG_LOW - 10);
    VSCAN_ERR_GOTO_ERROR        = int(VSCAN_DEBUG_HIGH);                // Debug Level High

// Mode
    VSCAN_MODE_NORMAL           = 0;
    VSCAN_MODE_LISTEN_ONLY      = 1;
    VSCAN_MODE_SELF_RECEPTION   = 2;

// Speed
    VSCAN_SPEED_1M              = pVOID(8);
    VSCAN_SPEED_800K            = pVOID(7);
    VSCAN_SPEED_500K            = pVOID(6);
    VSCAN_SPEED_250K            = pVOID(5);
    VSCAN_SPEED_125K            = pVOID(4);
    VSCAN_SPEED_100K            = pVOID(3);
    VSCAN_SPEED_50K             = pVOID(2);
    VSCAN_SPEED_20K             = pVOID(1);
// generally not possible with the TJA1050
//    VSCAN_SPEED_10K           = pVOID(0);

// Device Types
    VSCAN_HWTYPE_UNKNOWN        =  0;
    VSCAN_HWTYPE_SERIAL         =  1;
    VSCAN_HWTYPE_USB            =  2;
    VSCAN_HWTYPE_NET            =  3;
    VSCAN_HWTYPE_BUS            =  4;

    VSCAN_IOCTL_OFF             = pVOID(0);
    VSCAN_IOCTL_ON              = pVOID(1);

// Timestamp
    VSCAN_TIMESTAMP_OFF         = VSCAN_IOCTL_OFF;
    VSCAN_TIMESTAMP_ON          = VSCAN_IOCTL_ON;

// Filter Mode
    VSCAN_FILTER_MODE_SINGLE    = pVOID(1);
    VSCAN_FILTER_MODE_DUAL      = pVOID(2);

// Ioctls
    VSCAN_IOCTL_SET_DEBUG         =  1;
    VSCAN_IOCTL_GET_HWPARAM       =  2;
    VSCAN_IOCTL_SET_SPEED         =  3;
    VSCAN_IOCTL_SET_BTR           =  4;
    VSCAN_IOCTL_GET_FLAGS         =  5;
    VSCAN_IOCTL_SET_ACC_CODE_MASK =  6;
    VSCAN_IOCTL_SET_TIMESTAMP     =  7;
    VSCAN_IOCTL_SET_DEBUG_MODE    =  8;
    VSCAN_IOCTL_SET_BLOCKING_READ =  9;
    VSCAN_IOCTL_SET_FILTER_MODE   = 10;
    VSCAN_IOCTL_GET_API_VERSION   = 11;

// Bits for VSCAN_IOCTL_GET_FLAGS
    VSCAN_IOCTL_FLAG_RX_FIFO_FULL     = (1 shl 0);
    VSCAN_IOCTL_FLAG_TX_FIFO_FULL     = (1 shl 1);
    VSCAN_IOCTL_FLAG_ERR_WARNING      = (1 shl 2);
    VSCAN_IOCTL_FLAG_DATA_OVERRUN     = (1 shl 3);
    VSCAN_IOCTL_FLAG_UNUSED           = (1 shl 4);
    VSCAN_IOCTL_FLAG_ERR_PASSIVE      = (1 shl 5);
    VSCAN_IOCTL_FLAG_ARBIT_LOST       = (1 shl 6);
    VSCAN_IOCTL_FLAG_BUS_ERROR        = (1 shl 7);
    VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL = (1 shl 16);

// Masks for VSCAN_IOCTL_SET_ACC_CODE_MASK
    VSCAN_IOCTL_ACC_CODE_ALL = Cardinal($00000000);
    VSCAN_IOCTL_ACC_MASK_ALL = Cardinal($FFFFFFFF);

// Flags
    VSCAN_FLAGS_STANDARD     = (1 shl 0);
    VSCAN_FLAGS_EXTENDED     = (1 shl 1);
    VSCAN_FLAGS_REMOTE       = (1 shl 2);
    VSCAN_FLAGS_TIMESTAMP    = (1 shl 3);


// If the function succeeds, the return value is greater zero (handle)
// If the function fails, the return value is one of VSCAN_STATUS
Function VSCAN_Open(SerialNrORComPortORNet: PChar; Mode: DWORD): VSCAN_HANDLE; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_Close(Handle: VSCAN_HANDLE): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_Ioctl(Handle: VSCAN_HANDLE; Ioctl: DWORD; Param: pVOID): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_Flush(Handle: VSCAN_HANDLE): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_Write(Handle: VSCAN_HANDLE; Buf: pVSCAN_MSG; Size: DWORD; Written: pDWORD): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_Read(Handle: VSCAN_HANDLE; Buf: pVSCAN_MSG; Size: DWORD; Read: pDWORD): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// The return value is one of VSCAN_STATUS
Function VSCAN_SetRcvEvent(Handle: VSCAN_HANDLE; Event: THANDLE): VSCAN_STATUS; cdecl; external VSCAN_DLL;
// No return value for this function
Procedure VSCAN_GetErrorString(Handle: VSCAN_HANDLE; errString: PChar; MaxLen: DWORD); cdecl; external VSCAN_DLL;


implementation

end.