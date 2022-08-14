using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JRA
{
    
    class redneckBinCreator
    {
        [DllImport("User32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
        public void startShow(string fileToConvert)
        {
            
            try
            {
                string exePath = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\3dFan\\3D.exe";

                //string resourcePath = System.IO.File.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\3dFan\\3D.exe");
                Process firstProc = new Process();
                firstProc.StartInfo.FileName = exePath;
                //firstProc.EnableRaisingEvents = true;
                firstProc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                


                firstProc.Start();
                
                //we need to navigate their software here to be able to get a bin file out

                
                //we need to hide or move this new window
                SendKeys.SendWait("{ENTER}");
                
                //eneter if not connected
                //SendKeys.SendWait("{ENTER}");
                
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                
                //enter decode button
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(20);

                Console.WriteLine(fileToConvert);
                //SendKeys.SendWait(fileToConvert);
                //SendKeys.SendWait("{ENTER}");

                //call the file the same name 
                string filemameWithRemovedExtension = fileToConvert.Substring(fileToConvert.LastIndexOf('\\')+1);
                filemameWithRemovedExtension = filemameWithRemovedExtension.Substring(0, filemameWithRemovedExtension.Length - 4);
                Console.WriteLine(filemameWithRemovedExtension);
                Thread.Sleep(25);

                SendKeys.SendWait(fileToConvert);
                Thread.Sleep(20);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(200);
                //new filename
                SendKeys.SendWait(filemameWithRemovedExtension);
                //select fro common etc 
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{ENTER}");

                Thread.Sleep(900);
                //SendKeys.SendWait(filemameWithRemovedExtension);
                //alt tab to change from crop circle section
                SendKeys.SendWait("%" + "{TAB}");
                SendKeys.SendWait("%" + "{TAB}");
                SendKeys.SendWait("%" + "{TAB}");
                Thread.Sleep(200);

                
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(10);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(10);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(10);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(10);
                SendKeys.SendWait("{ENTER}");
                //close finished dialoug
                Thread.Sleep(10);
                //SendKeys.SendWait("{ENTER}");
                //really we need to wait until complete not just sleep 1 sec as vid will take longer
                Thread.Sleep(1000);
                firstProc.Kill();


                //firstProc.Close();


                //SendKeys.Send(Keys.LWin.ToString() + "{DOWN}");


                //firstProc.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
