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
                
                Thread.Sleep(400);
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
                Thread.Sleep(200);

                Console.WriteLine(fileToConvert);
                //SendKeys.SendWait(fileToConvert);
                //SendKeys.SendWait("{ENTER}");

                //call the file the same name 
                string filemameWithRemovedExtension = fileToConvert.Substring(fileToConvert.LastIndexOf('\\')+1);
                filemameWithRemovedExtension = filemameWithRemovedExtension.Substring(0, filemameWithRemovedExtension.Length - 4);
                Console.WriteLine(filemameWithRemovedExtension);
                Thread.Sleep(300);

                SendKeys.SendWait(fileToConvert);
                Thread.Sleep(300);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(300);
                //new filename
                SendKeys.SendWait(filemameWithRemovedExtension);
                //select fro common etc 
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{ENTER}");
                //try and wait as long as the video image needs depending on file size
                long length = new System.IO.FileInfo(fileToConvert).Length;
                Console.WriteLine(length);
                int multipleOf384 = (int)(length / (long)3767);
                Console.WriteLine(multipleOf384);

                Thread.Sleep(900);
                //SendKeys.SendWait(filemameWithRemovedExtension);
                //alt tab to change from crop circle section
                SendKeys.SendWait("%" + "{TAB}");
                Thread.Sleep(20);
                SendKeys.SendWait("%" + "{TAB}");
                Thread.Sleep(20);
                

                
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

                //work out file size and multiply this by time sleep i.e filesize/384 * 1000                   
                Thread.Sleep(1000*multipleOf384);

                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(400);
                firstProc.Kill();                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
