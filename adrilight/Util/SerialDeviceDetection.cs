using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace adrilight.Util
{
    internal class SerialDeviceDetection : ISerialDeviceDetection
    {

        public SerialDeviceDetection()
        {

        }


        public void RefreshDevice()
        {
            List<string> names = ComPortNames("1209", "c550");
            List<string> devices = new List<string>();
            if (names.Count > 0)
            {
                int counter = 0;
                foreach (String s in SerialPort.GetPortNames())
                {
                    if (names.Contains(s))
                    {
                        counter++;
                        devices.Add(s);
                        
                    }
                        
                    
                }
                 if(counter==1)
                {
                    HandyControl.Controls.MessageBox.Show("Phát hiện Ambino Basic Rev 2 đã kết nối ở " + devices[0], "Ambino Device", MessageBoxButton.OK, MessageBoxImage.Information);
                }
               else if (counter > 1)
                {
                    string delimiter = ",";
                    var alldevices = string.Join(delimiter, devices);
                    HandyControl.Controls.MessageBox.Show("Phát hiện Ambino Basic Rev 2 đã kết nối ở " + alldevices, "Ambino Device", MessageBoxButton.OK, MessageBoxImage.Information);
                }

               else  if (counter==0)//no device detected in the list
                {

                        HandyControl.Controls.MessageBox.Show("Không tìm thấy thiết bị nào của Ambino, kiểm tra lại kết nối hoặc thêm thiết bị theo cách thủ công", "Ambino Device", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
            else
                Console.WriteLine("Không tìm thấy thiết bị nào của Ambino, hãy thêm thiết bị theo cách thủ công");
        }


        List<string> ComPortNames(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

    }

}
