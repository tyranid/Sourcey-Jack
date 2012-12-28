/***********************************************************************

Sourcey Jack - A simple SOCKSifying application for Windows
Copyright (C) 2011 James Forshaw

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

***********************************************************************/

using System;
using System.Windows.Forms;
using DetoursLib;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace SourceyJack
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew = false;
            System.Threading.Mutex m = new System.Threading.Mutex(false, "{BE450405-C3CF-45D3-A1F4-29F0C0A0E02D}", out createdNew);

            if (args.Length > 0)
            {
                try
                {
                    if (createdNew)
                    {
                        m.Dispose();
                        Process p = Process.Start(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                        p.WaitForInputIdle(10000);
                    }

                    Thread.Sleep(10000);
                    
                    string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SJackHook.dll");

                    List<string> argList = new List<string>();

                    for (int i = 1; i < args.Length; ++i)
                    {
                        if (args[i].Contains(" "))
                        {
                            argList.Add(String.Format("\"{0}\"", args[i]));
                        }
                        else
                        {
                            argList.Add(args[i]);
                        }
                    }

                    string cmdLine = argList.Count > 0 ? String.Join(" ", argList) : null;

                    Detours.CreateProcessWithDll(args[0], cmdLine, dllPath);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("Already a copy running");
                }
            }
        }
    }
}
