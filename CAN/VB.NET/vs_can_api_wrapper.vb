' VS CAN API wrapper class (made for API version 1.6.0)

Option Strict Off

Imports System.Runtime.InteropServices
Imports System.Text

Class VSCANException
    Inherits Exception
    Public Sub New(ByVal str As String)
        Console.WriteLine(str)
    End Sub 'New
End Class 'VSCANException

''' <summary>
''' CAN message structure
''' </summary>
<StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.[Ansi])> _
Public Structure VSCAN_MSG
    ''' <summary>
    ''' CAN frame ID
    ''' </summary>
    Public Id As UInteger
    ''' <summary>
    ''' number of data bytes
    ''' </summary>
    Public Size As Byte
    ''' <summary>
    ''' data bytes
    ''' </summary>
    <MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst:=8)> _
    Public Data() As Byte
    ''' <summary>
    ''' CAN frame flags
    ''' </summary>
    Public Flags As Byte
    ''' <summary>
    ''' time stamp if configured
    ''' </summary>
    Public Timestamp As UShort
End Structure
''' <summary>
''' Hardware Parameter Structure
''' </summary>
<StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.[Ansi])> _
Public Structure VSCAN_HWPARAM
    ''' <summary>
    ''' device serial number
    ''' </summary>
    Public SerialNr As UInteger
    ''' <summary>
    ''' hardware version
    ''' </summary>
    Public HwVersion As Byte
    ''' <summary>
    ''' firmware version
    ''' </summary>
    Public SwVersion As Byte
    ''' <summary>
    ''' hardware type
    ''' </summary>
    Public HwType As Byte
End Structure
''' <summary>
'''  API Version Structure
''' </summary>
<StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.[Ansi])> _
Public Structure VSCAN_API_VERSION
    ''' <summary>
    ''' major number
    ''' </summary>
    Public Major As Byte
    ''' <summary>
    ''' minor number
    ''' </summary>
    Public Minor As Byte
    ''' <summary>
    ''' subminor number
    ''' </summary>
    Public SubMinor As Byte
End Structure
''' <summary>
''' Acceptance Code and Mask Structure
''' </summary>
<StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.[Ansi])> _
Public Structure VSCAN_CODE_MASK
    ''' <summary>
    ''' acceptance code
    ''' </summary>
    Public Code As UInt32
    ''' <summary>
    ''' acceptance mask
    ''' </summary>
    Public Mask As UInt32
End Structure
''' <summary>
''' Bit Timing Register Structure
''' </summary>
<StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.[Ansi])> _
Public Structure VSCAN_BTR
    ''' <summary>
    ''' first bit-timning register
    ''' </summary>
    Public Btr0 As Byte
    ''' <summary>
    ''' second bit-timning register
    ''' </summary>
    Public Btr1 As Byte
End Structure

Public Class VSCAN

    ''' <summary>
    ''' use first found device
    ''' </summary>
    Public Const VSCAN_FIRST_FOUND As String = ""

    ''' <summary>
    ''' set console as debug output
    ''' </summary>
    Public Const VSCAN_DEBUG_MODE_CONSOLE As Integer = 1
    ''' <summary>
    ''' set file as debug output
    ''' </summary>
    Public Const VSCAN_DEBUG_MODE_FILE As Integer = 2

    ''' <summary>
    ''' no debug information
    ''' </summary>
    Public Const VSCAN_DEBUG_NONE As Integer = 0
    ''' <summary>
    ''' errors from the VSCAN API
    ''' </summary>
    Public Const VSCAN_DEBUG_LOW As Integer = -1
    ''' <summary>
    ''' informations from the VSCAN API
    ''' </summary>
    Public Const VSCAN_DEBUG_MID As Integer = -51
    ''' <summary>
    ''' errors from system functions
    ''' </summary>
    Public Const VSCAN_DEBUG_HIGH As Integer = -101

    ''' <summary>
    ''' the normal operation mode
    ''' </summary>
    Public Const VSCAN_MODE_NORMAL As Integer = 0
    ''' <summary>
    ''' the listen only mode, in which
    ''' no CAN interaction will be done from the controller
    ''' </summary>
    Public Const VSCAN_MODE_LISTEN_ONLY As Integer = 1
    ''' <summary>
    ''' the self reception mode, in which the device
    ''' receives also the frames that it sents.
    ''' The firmware version must be 1.4 or greater
    ''' and the DLL version 1.6 or greater
    ''' </summary>
    Public Const VSCAN_MODE_SELF_RECEPTION As Integer = 2

    ''' <summary>
    ''' 1Mb/s
    ''' </summary>
    Public Const VSCAN_SPEED_1M As Integer = 8
    ''' <summary>
    ''' 800Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_800K As Integer = 7
    ''' <summary>
    ''' 500Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_500K As Integer = 6
    ''' <summary>
    ''' 250Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_250K As Integer = 5
    ''' <summary>
    ''' 125Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_125K As Integer = 4
    ''' <summary>
    ''' 100Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_100K As Integer = 3
    ''' <summary>
    ''' 50Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_50K As Integer = 2
    ''' <summary>
    ''' 20Kb/s
    ''' </summary>
    Public Const VSCAN_SPEED_20K As Integer = 1

    ''' <summary>
    ''' unkbown device
    ''' </summary>
    Public Const VSCAN_HWTYPE_UNKNOWN As Integer = 0
    ''' <summary>
    ''' SER-CAN
    ''' </summary>
    Public Const VSCAN_HWTYPE_SERIAL As Integer = 1
    ''' <summary>
    ''' USB-CAN
    ''' </summary>
    Public Const VSCAN_HWTYPE_USB As Integer = 2
    ''' <summary>
    ''' NET-CAN
    ''' </summary>
    Public Const VSCAN_HWTYPE_NET As Integer = 3
    ''' <summary>
    ''' PCI-CAN and OnRISC Alena
    ''' </summary>
    Public Const VSCAN_HWTYPE_BUS As Integer = 4

    ''' <summary>
    ''' disable option
    ''' </summary>
    Public Const VSCAN_IOCTL_OFF As Integer = 0
    ''' <summary>
    ''' enable option
    ''' </summary>
    Public Const VSCAN_IOCTL_ON As Integer = 1

    ''' <summary>
    ''' turn time stamp off
    ''' </summary>
    Public Const VSCAN_TIMESTAMP_OFF As Integer = VSCAN.VSCAN_IOCTL_OFF
    ''' <summary>
    '''  turn time stamp on
    ''' </summary>
    Public Const VSCAN_TIMESTAMP_ON As Integer = VSCAN.VSCAN_IOCTL_ON

    ''' <summary>
    ''' single filter mode
    ''' </summary>
    Public Const VSCAN_FILTER_MODE_SINGLE As Integer = 1
    ''' <summary>
    ''' dual filter mode
    ''' </summary>
    Public Const VSCAN_FILTER_MODE_DUAL As Integer = 2

    ''' <summary>
    ''' RX FIFO full flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_RX_FIFO_FULL As Integer = (1) << (0)
    ''' <summary>
    ''' TX FIFO full flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_TX_FIFO_FULL As Integer = (1) << (1)
    ''' <summary>
    ''' error warning flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_ERR_WARNING As Integer = (1) << (2)
    ''' <summary>
    ''' data overrun flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_DATA_OVERRUN As Integer = (1) << (3)
    ''' <summary>
    ''' unused
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_UNUSED As Integer = (1) << (4)
    ''' <summary>
    ''' passive error flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_ERR_PASSIVE As Integer = (1) << (5)
    ''' <summary>
    ''' arbitration lost flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_ARBIT_LOST As Integer = (1) << (6)
    ''' <summary>
    ''' bus error flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_BUS_ERROR As Integer = (1) << (7)
    ''' <summary>
    ''' API RX FIFO full flag
    ''' </summary>
    Public Const VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL As Integer = (1) << (16)


    ''' <summary>
    ''' receive all frames code
    ''' </summary>
    Public Const VSCAN_IOCTL_ACC_CODE_ALL As Integer = 0
    ''' <summary>
    ''' receive all frames mask
    ''' </summary>
    Public Const VSCAN_IOCTL_ACC_MASK_ALL As Integer = -1

    ''' <summary>
    ''' standard frame
    ''' </summary>
    Public Const VSCAN_FLAGS_STANDARD As Byte = (1) << (0)
    ''' <summary>
    ''' extended frame
    ''' </summary>
    Public Const VSCAN_FLAGS_EXTENDED As Byte = (1) << (1)
    ''' <summary>
    ''' remote frame
    ''' </summary>
    Public Const VSCAN_FLAGS_REMOTE As Byte = (1) << (2)
    ''' <summary>
    ''' time stamp present
    ''' </summary>
    Public Const VSCAN_FLAGS_TIMESTAMP As Byte = (1) << (3)

    ''' <summary>
    ''' set debug verbosity level
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_DEBUG As Integer = 1
    ''' <summary>
    ''' get hardware parameter
    ''' </summary>
    Public Const VSCAN_IOCTL_GET_HWPARAM As Integer = 2
    ''' <summary>
    ''' set CAN speed
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_SPEED As Integer = 3
    ''' <summary>
    ''' set CAN bus timing registers
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_BTR As Integer = 4
    ''' <summary>
    ''' get CAN status flags
    ''' </summary>
    Public Const VSCAN_IOCTL_GET_FLAGS As Integer = 5
    ''' <summary>
    ''' setup CAN frame filter
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_ACC_CODE_MASK As Integer = 6
    ''' <summary>
    '''  enable/disable timestamp
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_TIMESTAMP As Integer = 7
    ''' <summary>
    ''' set debugging mode
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_DEBUG_MODE As Integer = 8
    ''' <summary>
    '''  enable/disable blocking read
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_BLOCKING_READ As Integer = 9
    ''' <summary>
    ''' set filter mode
    ''' </summary>
    Public Const VSCAN_IOCTL_SET_FILTER_MODE As Integer = 10
    ''' <summary>
    '''  get API version
    ''' </summary>
    Public Const VSCAN_IOCTL_GET_API_VERSION As Integer = 11

    ''' CAN channel handle
    Private handle As Integer
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Open", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Open(ByVal SerialNrORComPortORNet As IntPtr, ByVal Mode As UInteger) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Close", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Close(ByVal Handle As Integer) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Ioctl", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Ioctl(ByVal Handle As Integer, ByVal Ioctl As UInteger, ByVal Param As IntPtr) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Flush", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Flush(ByVal Handle As Integer) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Write", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Write(ByVal Handle As Integer, ByVal Buf() As VSCAN_MSG, ByVal Size As UInteger, ByRef Written As UInteger) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_Read", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Function VSCAN_Read(ByVal Handle As Integer, ByVal Buf As IntPtr, ByVal Size As UInteger, ByRef Read As UInteger) As Integer
    End Function
    <DllImportAttribute("vs_can_api", EntryPoint:="VSCAN_GetErrorString", CallingConvention:=CallingConvention.Cdecl)> _
    Private Shared Sub VSCAN_GetErrorString(ByVal Status As Integer, ByVal ErrString As StringBuilder, ByVal MaxLen As UInteger)
    End Sub
    ''' <summary>
    ''' open CAN channel
    ''' </summary>
    ''' <param name="SerialNrORComPortORNet">a char pointer</param>
    ''' <param name="Mode">modes
    ''' <list type="bullet">
    ''' <item><description><see cref="VSCAN_MODE_NORMAL"/></description></item>
    ''' <item><description><see cref="VSCAN_MODE_LISTEN_ONLY"/></description></item>
    ''' <item><description><see cref="VSCAN_MODE_SELF_RECEPTION"/></description></item>
    ''' </list>
    ''' </param>
    Public Sub Open(ByVal SerialNrORComPortORNet As String, ByVal Mode As UInteger)
        If SerialNrORComPortORNet.Length > 0 Then
            Dim Ptr As IntPtr
            Dim encoding As New System.Text.UTF8Encoding()
            Dim tmpStr() As Byte = encoding.GetBytes(SerialNrORComPortORNet)
            Dim zsStr() As Byte = New Byte(SerialNrORComPortORNet.Length) {}

            tmpStr.CopyTo(zsStr, 0)
            zsStr(SerialNrORComPortORNet.Length) = 0
            Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(tmpStr(0)) * (SerialNrORComPortORNet.Length + 1))
            Marshal.Copy(zsStr, 0, Ptr, zsStr.Length)
            handle = VSCAN_Open(Ptr, Mode)
            Marshal.FreeHGlobal(Ptr)
        Else
            handle = VSCAN_Open(IntPtr.Zero, Mode)
        End If
        If handle <= 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + handle.ToString)
        End If

    End Sub
    ''' <summary>
    ''' close CAN channel
    ''' </summary>
    Public Sub Close()
        Dim rc As Integer

        rc = VSCAN_Close(handle)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub

    ''' <summary>
    ''' get and set special information and commands of the CAN device
    ''' </summary>
    ''' <param name="uiIoctl">tells the function which of the following ioctl should be called</param>
    ''' <param name="Param">a pointer to the data for the ioctls</param>
    ''' <remarks></remarks>
    Public Sub Ioctl(ByVal uiIoctl As UInteger, ByVal Param As System.IntPtr)
        Dim rc As Integer

        rc = VSCAN_Ioctl(handle, uiIoctl, Param)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' send all frames immediately out to the CAN bus
    ''' </summary>
    Public Sub Flush()
        Dim rc As Integer

        rc = VSCAN_Flush(handle)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' write CAN frame(s). The frames will be buffered and send out after some time - this time can grow up to one
    ''' time slice of the scheduler (Windows = ~16ms and Linux = ~10ms). If you want to send
    ''' the frames immediately, you must call the function Flush()
    ''' </summary>
    ''' <param name="Buf">a pointer to one element or an array of the structure VSCAN_MSG</param>
    ''' <param name="Size">the number of the array elements in Buf</param>
    ''' <param name="Written">a pointer to a DWORD that will receive the number of frames written</param>
    Public Sub Write(ByVal Buf() As VSCAN_MSG, ByVal Size As UInteger, ByRef Written As UInteger)
        Dim rc As Integer

        rc = VSCAN_Write(handle, Buf, Size, Written)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' read CAN frame(s).
    ''' The read mode of this function is set to non-blocking mode per default. This
    ''' means that Read will return immediately - even when there are no frames at the
    ''' moment. To make the  VSCAN_Read blocking, use the ioctl <see cref="VSCAN_IOCTL_SET_BLOCKING_READ"/>
    ''' then it will return only if frames were received
    ''' </summary>
    ''' <param name="Buf">a pointer to one element or an array of the structure VSCAN_MSG</param>
    ''' <param name="Size">the number of the array elements in Buf</param>
    ''' <param name="uiRead">pointer to a DWORD that will receive the real number of the frames read</param>
    ''' <remarks></remarks>
    Public Sub Read(ByRef Buf() As VSCAN_MSG, ByVal Size As UInteger, ByRef uiRead As UInteger)
        Dim rc As UInteger
        Dim BufPtr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Buf(0)) * CType(Size, Integer))
        Marshal.StructureToPtr(Buf(0), BufPtr, False)

        rc = VSCAN_Read(handle, BufPtr, Size, uiRead)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If

        Dim pCurrent As IntPtr = BufPtr
        Dim i As Integer

        For i = 0 To uiRead - 1
            Buf(i) = CType(Marshal.PtrToStructure(pCurrent, Buf(i).GetType()), VSCAN_MSG)
            pCurrent = New IntPtr(pCurrent.ToInt32() + Marshal.SizeOf(Buf(0)))
        Next
        
        Marshal.FreeHGlobal(BufPtr)
        
    End Sub
    ''' <summary>
    ''' get hardware parameter
    ''' </summary>
    ''' <param name="hw">pointer to VSCAN_HWPARAM structure</param>
    Public Sub GetHwParams(ByRef hw As VSCAN_HWPARAM)
        Dim rc As UInteger
        Dim Ptr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(hw))
        Marshal.StructureToPtr(hw, Ptr, False)

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_GET_HWPARAM, Ptr)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
        hw = CType(Marshal.PtrToStructure(Ptr, hw.GetType()), VSCAN_HWPARAM)
        Marshal.FreeHGlobal(Ptr)
    End Sub
    ''' <summary>
    ''' get VSCAN API version
    ''' </summary>
    ''' <param name="ApiVer">pointer to VSCAN_API_VERSION structure</param>
    Public Sub GetApiVersion(ByRef ApiVer As VSCAN_API_VERSION)
        Dim rc As UInteger
        Dim Ptr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ApiVer))
        Marshal.StructureToPtr(ApiVer, Ptr, False)

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_GET_API_VERSION, Ptr)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
        ApiVer = CType(Marshal.PtrToStructure(Ptr, ApiVer.GetType()), VSCAN_API_VERSION)
        Marshal.FreeHGlobal(Ptr)
    End Sub
    ''' <summary>
    ''' get extended status and error flags
    ''' </summary>
    ''' <param name="Flags">pointer to byte</param>
    Public Sub GetFlags(ByRef Flags As Byte)
        Dim rc As UInteger
        Dim Ptr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Flags))
        Marshal.StructureToPtr(Flags, Ptr, False)

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_GET_FLAGS, Ptr)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
        Flags = CType(Marshal.PtrToStructure(Ptr, Flags.GetType()), Byte)
        Marshal.FreeHGlobal(Ptr)
    End Sub
    ''' <summary>
    ''' set acceptance code and mask
    ''' </summary>
    ''' <param name="Mask"> pointer to <see cref="VSCAN_CODE_MASK"/> structure</param>
    Public Sub SetACCCodeMask(ByVal Mask As VSCAN_CODE_MASK)
        Dim rc As UInteger
        Dim Ptr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Mask))
        Marshal.StructureToPtr(Mask, Ptr, False)

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_ACC_CODE_MASK, Ptr)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
        Marshal.FreeHGlobal(Ptr)
    End Sub
    ''' <summary>
    ''' set bit-timing register
    ''' </summary>
    ''' <param name="Btr">pointer to <see cref="VSCAN_BTR"/> structure</param>
    Public Sub SetBTR(ByVal Btr As VSCAN_BTR)
        Dim rc As UInteger
        Dim Ptr As System.IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Btr))
        Marshal.StructureToPtr(Btr, Ptr, False)

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_BTR, Ptr)
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
        Marshal.FreeHGlobal(Ptr)
    End Sub
    ''' <summary>
    ''' set CAN speed
    ''' </summary>
    ''' <param name="Speed">CAN speed</param>
    Public Sub SetSpeed(ByVal Speed As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_SPEED, New IntPtr(Speed))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' set filter mode
    ''' </summary>
    ''' <param name="Mode">filter mode:
    ''' <list type="bullet">
    ''' <item><description><see cref="VSCAN_FILTER_MODE_SINGLE"/></description></item>
    ''' <item><description><see cref="VSCAN_FILTER_MODE_DUAL"/></description></item>
    ''' </list>
    ''' </param>
    Public Sub SetFilterMode(ByVal Mode As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_FILTER_MODE, New IntPtr(Mode))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' set time stamp mode
    ''' </summary>
    ''' <param name="Mode">time stamp mode:
    ''' <list type="bullet">
    ''' <item><description><see cref="VSCAN_TIMESTAMP_OFF"/></description></item>
    ''' <item><description><see cref="VSCAN_TIMESTAMP_ON"/></description></item>
    ''' </list>
    ''' </param>
    Public Sub SetTimestamp(ByVal Mode As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_TIMESTAMP, New IntPtr(Mode))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' set blocking read mode
    ''' </summary>
    ''' <param name="Mode">blocking read mode:
    ''' <list type="bullet">
    ''' <item><description><see cref="VSCAN_IOCTL_OFF"/></description></item>
    ''' <item><description><see cref="VSCAN_IOCTL_ON"/></description></item>
    ''' </list>
    ''' </param>

    Public Sub SetBlockingRead(ByRef Mode As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_BLOCKING_READ, New IntPtr(Mode))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' set debugging verbosity level
    ''' </summary>
    ''' <param name="Level">
    ''' <list type="bullet">
    ''' <item><description><see cref="VSCAN_DEBUG_NONE"/></description></item>
    ''' <item><description><see cref="VSCAN_DEBUG_LOW"/></description></item>
    ''' <item><description><see cref="VSCAN_DEBUG_MID"/></description></item>
    ''' <item><description><see cref="VSCAN_DEBUG_HIGH"/></description></item>
    ''' </list>
    ''' </param>
    Public Sub SetDebug(ByVal Level As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_DEBUG, New IntPtr(Level))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
    ''' <summary>
    ''' set debug mode
    ''' </summary>
    ''' <param name="FileORConsole">
    ''' <item><description><see cref="VSCAN_DEBUG_MODE_CONSOLE"/></description></item>
    ''' <item><description><see cref="VSCAN_DEBUG_MODE_FILE"/></description></item>
    ''' </param>
    Public Sub SetDebugMode(ByVal FileORConsole As Integer)
        Dim rc As UInteger

        rc = VSCAN_Ioctl(handle, VSCAN_IOCTL_SET_DEBUG_MODE, New IntPtr(FileORConsole))
        If rc < 0 Then
            Dim err_string As New StringBuilder(64)

            VSCAN_GetErrorString(handle, err_string, 64 - 1)
            Throw New VSCANException(err_string.ToString() + " ErrNr: " + rc.ToString)
        End If
    End Sub
End Class
