using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace LzSoft.SysService
{
    public class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Process instance = RunningInstance();
            if (instance == null)
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new main());
            }
        }
        public static Process RunningInstance() 
        { 
            Process current = Process.GetCurrentProcess(); 
            Process[] processes = Process.GetProcessesByName(current.ProcessName); 
            foreach (Process process in processes) 
            { 
                if (process.Id != current.Id) 
                { 
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) 
                    { 
                        return process; 
                    } 
                } 
            } 
            return null; 
        }
    }
}
