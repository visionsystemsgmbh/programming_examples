using System;
using System.Collections.Generic;
using VSCom.CanApi;

namespace MyApp
{
    class TestDrive
    {
        private static void DecodeFlags(int flags)
        {
            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL) > 0)
            {
                Console.WriteLine("API RX FIFO full");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ARBIT_LOST) > 0)
            {
                Console.WriteLine("Arbitration lost");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_BUS_ERROR) > 0)
            {
                Console.WriteLine("Bus error");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_DATA_OVERRUN) > 0)
            {
                Console.WriteLine("Data overrun");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ERR_PASSIVE) > 0)
            {
                Console.WriteLine("Passive error");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ERR_WARNING) > 0)
            {
                Console.WriteLine("Error warning");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_RX_FIFO_FULL) > 0)
            {
                Console.WriteLine("RX FIFO full");
            }

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_TX_FIFO_FULL) > 0)
            {
                Console.WriteLine("TX FIFO full");
            }
        }



        public static void Main(string[] args)
        {
            VSCAN_MSG[] msgs = new VSCAN_MSG[2];
            VSCAN CanDevice = new VSCAN();
            UInt32 Written = 0;
            UInt32 Read = 0;
            VSCAN_API_VERSION api_ver = new VSCAN_API_VERSION();
            byte Flags = 0x0;

            try
            {
                // set debugging options
                CanDevice.SetDebug(VSCAN.VSCAN_DEBUG_NONE);
                CanDevice.SetDebugMode(VSCAN.VSCAN_DEBUG_MODE_FILE);
                
                // open CAN channel: please specify the name of your device according to User Manual
                CanDevice.Open(VSCAN.VSCAN_FIRST_FOUND, VSCAN.VSCAN_MODE_SELF_RECEPTION);
                
                // set some options
                CanDevice.SetSpeed(VSCAN.VSCAN_SPEED_1M);
                CanDevice.SetTimestamp(VSCAN.VSCAN_TIMESTAMP_ON);
                CanDevice.SetBlockingRead(VSCAN.VSCAN_IOCTL_ON);

                // get API version
                CanDevice.GetApiVersion(ref api_ver);
                Console.WriteLine("");
                Console.WriteLine("API version: " + api_ver.Major + "." + api_ver.Minor + "." + api_ver.SubMinor);

                msgs[0].Id = 0x100;
                msgs[0].Size = 2;
                msgs[0].Data = new byte[8];
                msgs[0].Data[0] = 0xde;
                msgs[0].Data[1] = 0xad;
                msgs[0].Flags = VSCAN.VSCAN_FLAGS_EXTENDED;

                msgs[1].Id = 0x101;
                msgs[1].Size = 2;
                msgs[1].Data = new byte[8];
                msgs[1].Data[0] = 0xbe;
                msgs[1].Data[1] = 0xef;
                msgs[1].Flags = VSCAN.VSCAN_FLAGS_EXTENDED;
           
                // send CAN frames
                CanDevice.Write(msgs, 2, ref Written);

                // send immediately 
                CanDevice.Flush();
                Console.WriteLine("");
                Console.WriteLine("Send CAN frames: " + Written);

                // read CAN frames
                CanDevice.Read(ref msgs, 2, ref Read);
                Console.WriteLine("");
                Console.WriteLine("Read CAN frames: " + Read);

                for (int i = 0; i < Read; i++)
                {
                    Console.WriteLine("");
                    Console.WriteLine("CAN frame " + i);
                    Console.WriteLine("ID: " + msgs[i].Id);
                    Console.WriteLine("Size: " + msgs[i].Size);
                    Console.Write("Data: ");

                    for (int j = 0; j < msgs[i].Size; j++)
                    {
                        Console.Write(msgs[i].Data[j] + " ");
                    }

                    Console.WriteLine("");
                    if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_STANDARD) != 0)
                        Console.WriteLine("VSCAN_FLAGS_STANDARD");
                    if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_EXTENDED) != 0)
                        Console.WriteLine("VSCAN_FLAGS_EXTENDED");
                    if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_REMOTE) != 0)
                        Console.WriteLine("VSCAN_FLAGS_REMOTE");
                    if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_TIMESTAMP) != 0)
                        Console.WriteLine("VSCAN_FLAGS_TIMESTAMP");
                    Console.WriteLine("TS: " + msgs[i].TimeStamp);
                }

                // get extended status and error flags
                CanDevice.GetFlags(ref Flags);
                Console.WriteLine("");
                Console.WriteLine("Extended status and error flags: " + Flags);
                DecodeFlags(Flags);

                // close CAN channel
                CanDevice.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("CAN opened " + e.Message);
            }
        }
    }
}

