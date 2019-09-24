using System;
using System.IO;
using System.IO.Ports;

namespace c.sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort ser = new SerialPort("COM5", 115200);
            Console.WriteLine("Hello World!");
        }
    }
}
