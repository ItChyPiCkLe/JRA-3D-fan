using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;


    class Fan
    {

        private Stream stream;
        private TcpClient client;
        private string EOF = "B2DDDDEDC0EEBDF9E5B7";
        private byte[] EOFbytes = Encoding.ASCII.GetBytes("B2DDDDEDC0EEBDF9E5B7");
        bool isConnected = false;
        string fileNamesString = "";
        List<string> fileNamesOnDevice = new List<string>();
        bool isPaused = false;

        private string makeFnamePacket(string input)
        {
            //take the full filename input and strip out the end (just the filname and not path)
            string Fname = filenameStrpper(input);
            //append filenmae size code
            string filenameCode = lookupFnameSizeCode(Fname);
            Console.WriteLine("length of filename is >>> " + Fname.Length);
            Console.WriteLine(filenameCode);
            string output = "B2DDDDED\0" + filenameCode + Fname + "C0EEBDF9E5B7";

            return output;
        }
        //function used to append a code that the device uses for the size of the filename, needs adding to
        private string lookupFnameSizeCode(string fname)
        {
            string code = "";
            //remove extension
            fname = fname.Substring(0,fname.Length - 4);
            if(fname.Length == 8)
            {
                code = "nc";
            }
            else if (fname.Length == 2)
            {
            //
                code = "lc";
            }
            else if(fname.Length == 3)
            {
                code = "ld";
            }
            else if (fname.Length == 9)
            {
                code = "nd";
            }
            else if (fname.Length == 5)
            {
                code = "mc";
            }
            else if (fname.Length == 4)
            {
                code = "le";
            }
            else if (fname.Length == 6)
            {
                code = "md";
            }
            else if (fname.Length == 7)
            {
                code = "me";
            }
        return code;
        }
        private string filenameStrpper(string input)
        {
            string output ="";
            //last bit of file path
            output = input.Substring(input.LastIndexOf('\\')+1);
            return output;
        }
        public void sendFile(string fileToSend)
        {
            //if (stream != null)
            {
            string prepareForFile = "B2DDDDEDC0EEBDF9E5B7";
            //strip out filename using method
            string filenameToSend = makeFnamePacket(fileToSend);
            initiliseStream();

            Thread.Sleep(100);
            //send the request so the device knows incoming data
            sendRequest(prepareForFile);
            //client.ReceiveTimeout = 1;
            Thread.Sleep(200);
            //recieveAck();
            Thread.Sleep(100);
            //send the filename so device can save it s this
            sendRequest(filenameToSend);

            //make the byte array from the provided function string fileToSend is full file path
            byte[] sendFile = makeFile(fileToSend);
            recieveAck();
            sendData(sendFile);
            }
        }
        //on their software they must pause as currently this skips the next one
        public void nextImage()
        {
        string nextCmd = "C0EEB7C9BAA3\0cccC0EEBDF9E5B7";        
            sendRequest(nextCmd);
            reciveFilenames();            
        }
        public void pauseImage()
        {
        string pauseCmd = "C0EEB7C9BAA3\0cceC0EEBDF9E5B7";
        if (!isPaused)
        {            
            sendRequest(pauseCmd);
            sendAck();
            isPaused = true;
        }
        else
        {
            sendRequest(pauseCmd);
            isPaused = false;
        }
        }

        private Stream getStream()
        {
            return stream;
        }
        private byte[] makeFile(string file)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(file);
            string fileContents = System.IO.File.ReadAllText(file);
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            byte[] newBytes = iso.GetBytes(fileContents);
            Encoding utf8 = Encoding.UTF8;
            byte[] isoBytes = Encoding.Convert(utf8, iso, bytes);
            return bytes;
        }
        public void getFilenames()
        {
            //make the request in byte form for sending
            string requyestForFileList = "C0EEB7C9BAA3C0EEBDF9E5B7";
            // Send the message to the connected TcpServer. 
            sendRequest(requyestForFileList);            
            reciveFilenames();
        }
        private TcpClient getClient()
        {
            return client;
        }
        private void writeToStream(byte [] stuffToWrite)
        {
            stream.Write(stuffToWrite, 0, stuffToWrite.Length);
            client.ReceiveTimeout = 1;
        }
        private void sendRequest(string request)
        {

        try
        {
            if (request.Length > 0 && request != null && stream != null)
            {
                Byte[] data = Encoding.ASCII.GetBytes(request);
                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
            }
        }
        catch (System.NullReferenceException r)
        {

        }
        catch
        {

        }
        }
        //function that will be used to populate gui with names of files on device
        public List<string> getListOfFilenamesOnDevie()
        {
            return fileNamesOnDevice;
        }
        private void sendData(byte[] data)
        {       
            stream.Flush();
            //chop and send
            fileChoppa(data);
            stream.Flush();
            stream.Close();
        }
        private void fileChoppa(byte[] file)
        {
            //use diff encodeing???
            byte[] EofArr = Encoding.ASCII.GetBytes(EOF);
            //chop into packets 1460 bytes with overhead to 1514            
            byte[] temp = new byte[1460];
            int count = 0;
            //go thru al bytes
            foreach (byte by in file)
            {
                //adding bytes to temp []
                temp[count] = by;
                //if the count is packet size, add packet to list to send
                if (count == 1459)
                {                    
                    //write packets as we make them
                    writeToStream(temp);
                    count = 0;
                    Array.Clear(temp, 0, temp.Length);
                }
                else
                {
                    count++;
                }                
            }
            //make new array with extra 20 bytes at end for EOF
            byte[] eofSendWithData = new byte[count + 20];
            //we need to add the eof to the last packet
            eofSendWithData = temp.Concat(EofArr).ToArray();
            //write final packet to stream
            writeToStream(eofSendWithData);       
            
        }
        //recieve filenames from device, needed as the device follows a particualr pattern
        public void reciveFilenames()
        {
            if (stream != null)
            {
                Byte[] rdata = new byte[14000];
                Int32 bytes = stream.Read(rdata, 0, rdata.Length);
                Encoding enc = Encoding.GetEncoding("iso-8859-1");
                Console.WriteLine("Received: {0}", enc.GetString(rdata, 0, rdata.Length)); 
                //store responce in variable for splitting
                fileNamesString = enc.GetString(rdata, 0, rdata.Length);
                //make list
                fileNamesOnDevice = filesList(fileNamesString);



            //test
            foreach (string c in fileNamesOnDevice)
                {
                    Console.WriteLine(c);
                    foreach(char t in c)
                    {
                    int hexVal = ((byte)t);
                        Console.WriteLine(hexVal + " which is >> " + t);
                    }
                }


            }

        }
        //delete the contents of the devices sd card
        public void deleteSD()
        {
            //command the device uses as instruction to delete sd card contents
            string deleteSDcontenrs = "C0EEB7C9BAA3\0ccjC0EEBDF9E5B7";
            sendRequest(deleteSDcontenrs);
            reciveFilenames();

        }
        //send acknowledgements to the device from my laptop
        public void sendAck()
        {
            Byte[] empty = new byte[0];
            stream.Write(empty, 0, empty.Length);
        }
        //receve responce from device
        public void recieveAck()
        {
        try
        {
            if (stream != null)
            {
                client.ReceiveTimeout = 1;
                Byte[] rdata = new byte[14000];
                stream.Flush();
                Int32 bytes = stream.Read(rdata, 0, rdata.Length);
                client.ReceiveTimeout = 50;
                Encoding enc = Encoding.GetEncoding("iso-8859-1");
                Console.WriteLine("Received Ack: {0}", enc.GetString(rdata, 0, rdata.Length));
            }
        }
        catch(System.IO.IOException e)
        {

        }
        }
        //connect to 3d holoFan device
        public void initiliseStream()
        {
        try
        {
            Int32 port = 20320;
            client = new TcpClient("192.168.4.1", port);
            stream = client.GetStream();
        }
        catch 
        {

        }
            
        }
        public bool getIsConnected()
        {
            return isConnected;
        }
        //function to split string into list of filenames
        public List<string> filesList(string input)
        {
            List<string> files = new List<string>();
            string runningChar = "";

            //Console.WriteLine("value of club is " + Convert.ToByte('♣'));
                        

        foreach (char c in input)
            {
                
                //get hex val of char
                int hexVal = ((byte)c);

                

                //if 03 in hex which is <3 04 is diamond 05 is club   
                if(hexVal == 01 || hexVal == 02 || hexVal == 03 || hexVal == 04 || hexVal == 05 || hexVal == 06|| hexVal == 07 || hexVal == 08)
                {
                    files.Add(runningChar);
                    runningChar = "";
                }
                //58 smiley
                else if (hexVal == 1)
                {
                    runningChar = runningChar.Substring(0, runningChar.Length);
                    files.Add(runningChar);
                    runningChar = "";
                }
                else
                {
                    runningChar += c;
                }

            }
        //add last
        //files.Add(runningChar);
        //remove first thing which is part of command
        if (files.Count > 1)
        {
            files.RemoveAt(0);
        }
        Console.WriteLine("how many files >> " + files.Count);
        return files;
        }
        public void closeConnection()
        {
            client.Close();
        }
        public void disconnect()
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = @"C:\WINDOWS\System32\ipconfig.exe";
            pInfo.Arguments = "/release";
            Process p = Process.Start(pInfo);
            p.WaitForExit();
         }
        private string getIP()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
               .ToString();
        }
        public void checkOnlineThread()
        {
        while (true)
        {           

            //if(getIP() == "192.168.4.2")
            if (getIP() == "192.168.4.2")
                {
                isConnected = true;

               
            }
            else
            {
                //Do stuffs when network not available
                isConnected = false;

            }
            Thread.Sleep(1000);
            }
        }



        public event EventHandler MyEvent;
        public void OnMyEvent(object sender, EventArgs e)
        {
            MessageBox.Show("MyEvent fired!");
        }



    //connect to the wifiNetwork of the deivce
    public void connectToAccessPoint()
        {
            string ssid = "3D_Circle_45cm_F4_04EED1";
            SimpleWifi.AccessPoint selectedAP = null;
            bool isApFound = false;
            Wifi wifi = new Wifi();
            // get list of access points
            IEnumerable<AccessPoint> accessPoints = wifi.GetAccessPoints();


            foreach (SimpleWifi.AccessPoint ap in accessPoints)
            {
                Console.WriteLine("WIfi point is" + ap.Name);
                if (ap.Name.Equals(ssid, StringComparison.InvariantCultureIgnoreCase))
                {
                    //Console.WriteLine("Finded");
                    selectedAP = ap;
                    isApFound = true;
                    break;
                }
            }

            if (!isApFound)
            {

                Console.WriteLine("No ssid");
                return;

            }

            // Auth
            SimpleWifi.AuthRequest authRequest = new AuthRequest(selectedAP);
            bool overwrite = true;

            if (authRequest.IsPasswordRequired)
            {
                if (selectedAP.HasProfile)
                // If there already is a stored profile for the network, we can either use it or overwrite it with a new password.
                {


                }

                if (overwrite)
                {

                }
            }

            //selectedAP.ConnectAsync(authRequest, overwrite, OnConnectedComplete);
            selectedAP.Connect(authRequest);
            if (selectedAP.IsConnected)
            {
                Console.WriteLine("We Connected san");
                isConnected = true;
                //Console.WriteLine(selectedAP.GetProfileXML());
            }
            else
            {
                Console.WriteLine("No We not connected");
            }
        }


    }
