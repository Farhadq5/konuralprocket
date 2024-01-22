using System;
using System.IO.Ports;

namespace konuralprocketGS
{
    internal class serialportmaneger
    {
        private SerialPort port;
        public serialportmaneger(string portname, int baudwidth)
        {
            port=new SerialPort();
            port.PortName=portname;
            port.BaudRate=baudwidth;
        }

        private void openport()
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                    Console.WriteLine($"Serial port {port.PortName} is already open.");
                }
                else
                    Console.WriteLine($"Serial port {port.PortName} is already open.");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
