Option Strict Off

Imports System

Module TestDrive
    Sub DecodeFlags(ByVal flags As Integer)
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL) > 0) Then
            Console.WriteLine("API RX FIFO full")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_ARBIT_LOST) > 0) Then
            Console.WriteLine("Arbitration lost")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_BUS_ERROR) > 0) Then
            Console.WriteLine("Bus error")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_DATA_OVERRUN) > 0) Then
            Console.WriteLine("Data overrun")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_ERR_PASSIVE) > 0) Then
            Console.WriteLine("Passive error")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_ERR_WARNING) > 0) Then
            Console.WriteLine("Error warning")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_RX_FIFO_FULL) > 0) Then
            Console.WriteLine("RX FIFO full")
        End If
        If ((flags And VSCAN.VSCAN_IOCTL_FLAG_TX_FIFO_FULL) > 0) Then
            Console.WriteLine("TX FIFO full")
        End If
    End Sub
    Sub Main()
        Dim CanDevice As VSCAN = New VSCAN()
        Dim msgs(2) As VSCAN_MSG
        Dim Written, uiRead As UInteger
        Dim i, j As UInteger
        Dim hw As VSCAN_HWPARAM
        Dim ApiVer As VSCAN_API_VERSION
        Dim flags As Byte

        Try
            ' enable debug output
            CanDevice.SetDebug(VSCAN.VSCAN_DEBUG_HIGH)
            CanDevice.SetDebugMode(VSCAN.VSCAN_DEBUG_MODE_FILE)

            ' open CAN channel
            CanDevice.Open(VSCAN.VSCAN_FIRST_FOUND, VSCAN.VSCAN_MODE_SELF_RECEPTION)

            ' enable blocking read
            CanDevice.SetBlockingRead(VSCAN.VSCAN_IOCTL_ON)

            ' enable timestamps
            CanDevice.SetTimestamp(VSCAN.VSCAN_TIMESTAMP_ON)

            ' get HW params
            CanDevice.GetHwParams(hw)
            Console.WriteLine("Get hardware parameter:")
            Console.WriteLine("HwVersion:" + hw.HwVersion.ToString + " SwVersion:" + (hw.SwVersion >> 4).ToString + "." + (hw.SwVersion And &HF).ToString)
            Console.WriteLine("SerNr:" + hw.SerialNr.ToString + " HwType:" + hw.HwType.ToString)

            ' get API version
            CanDevice.GetApiVersion(ApiVer)
            Console.WriteLine("Api: " + ApiVer.Major.ToString + "." + ApiVer.Minor.ToString + "." + ApiVer.SubMinor.ToString)

            msgs(0).Id = 256
            msgs(0).Size = 2
            msgs(0).Data = New Byte(8) {}
            msgs(0).Data(0) = 1
            msgs(0).Data(1) = 2

            msgs(1).Id = 256
            msgs(1).Size = 2
            msgs(1).Data = New Byte(8) {}
            msgs(1).Data(0) = 3
            msgs(1).Data(1) = 4

            ' send CAN frames
            CanDevice.Write(msgs, 2, Written)
            Console.WriteLine("Send CAN frames: " + Written.ToString)

            ' send immediately 
            CanDevice.Flush()

            ' read CAN frames
            CanDevice.Read(msgs, 2, uiRead)

            For i = 0 To (uiRead - 1)
                Console.WriteLine("ID: " + msgs(i).Id.ToString)
                Console.WriteLine("Size: " + msgs(i).Size.ToString)
                For j = 0 To msgs(i).Size - 1
                    Console.Write(msgs(i).Data(j).ToString + " ")
                Next
                Console.WriteLine("")
                If (msgs(i).Flags And VSCAN.VSCAN_FLAGS_TIMESTAMP) > 0 Then
                    Console.WriteLine("TS: " + msgs(i).Timestamp.ToString)
                End If
                If (msgs(i).Flags And VSCAN.VSCAN_FLAGS_STANDARD) > 0 Then
                    Console.WriteLine("Flag: VSCAN_FLAGS_STANDARD")
                End If
                If (msgs(i).Flags And VSCAN.VSCAN_FLAGS_EXTENDED) > 0 Then
                    Console.WriteLine("Flag: VSCAN_FLAGS_EXTENDED")
                End If
                If (msgs(i).Flags And VSCAN.VSCAN_FLAGS_REMOTE) > 0 Then
                    Console.WriteLine("Flag: VSCAN_FLAGS_REMOTE")
                End If
            Next

            ' get extended status and error flags
            CanDevice.GetFlags(flags)
            Console.WriteLine("flags: " + flags.ToString)
            DecodeFlags(flags)

            ' close CAN channel
            CanDevice.Close()

        Catch e As Exception
            System.Console.WriteLine(e.Message)
        End Try
    End Sub
End Module
