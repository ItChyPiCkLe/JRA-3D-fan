using SimpleWifi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        private string makeFnamePacket(string input)
        {
            //take the full filename input and strip out the end (just the filname and not path)
            string Fname = filenameStrpper(input);
            //Concat these to make commmand note the ld is for 3 character filenames 20 is mc
            //make it mc and append on Fname.Length with padding to allow for 20 length file names

            //get the nae without extension
            string FnameWithPadding = Fname.Substring(0, Fname.Length - 4);
            Console.WriteLine("filename for padding witout extension is >>>> " + FnameWithPadding);
        
            if(FnameWithPadding.Length < 20)
            {
                int amountOfPaddingToAdd = 20 - FnameWithPadding.Length;
                //add * as padding to make up to 20
                for(int i = 0; i < amountOfPaddingToAdd; i++)
                {
                    FnameWithPadding += "*";
                }

            }
            Console.WriteLine("filename after padding is >>>> " + FnameWithPadding);
            //else error name > 20 chars

            string output = "B2DDDDED\0mc" + FnameWithPadding + "C0EEBDF9E5B7";

            return output;
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
            doConnection();

            Thread.Sleep(100);
            //send the request so the device knows incoming data
            sendRequest(prepareForFile);
            Thread.Sleep(100);
            Thread.Sleep(100);
            //send the filename so device can save it s this
            sendRequest(filenameToSend);
            stream.FlushAsync();
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
        
            //getFilenames();
            reciveFilenames();
            sendAck();
            sendRequest(nextCmd);
            reciveFilenames();
            //sendAck();

        }
        public void pauseImage()
        {
            string pauseCmd = "C0EEB7C9BAA3\0cceC0EEBDF9E5B7";
            reciveFilenames();
            //sendAck();
            sendRequest(pauseCmd);
            sendAck();
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
            //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
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
            //if (stream != null)
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
                }


            }

        }
        //delete the contents of the devices sd card
        public void deleteSD()
        {
            //command the device uses as instruction to delete sd card contents
            string deleteSDcontenrs = "C0EEB7C9BAA3\0ccjC0EEBDF9E5B7";
            getFilenames();
            //do twice as dev made it that way
            reciveFilenames();
            sendRequest(deleteSDcontenrs);
            reciveFilenames();
            //sendAck();

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
            Byte[] rdata = new byte[14000];
            stream.Flush();
            Int32 bytes = stream.Read(rdata, 0, rdata.Length);
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            Console.WriteLine("Received Ack: {0}", enc.GetString(rdata, 0, rdata.Length));
        }
        //connect to 3d holoFan device
        public void doConnection()
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
        public List<string> filesList(string input)
        {
            List<string> files = new List<string>();
            string runningChar = "";

            foreach (char c in input)
            {
                
                //get hex val of char
                int hexVal = ((byte)c);
                
                //if 03 in hex which is <3
                if(hexVal == 03)
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
        files.RemoveAt(0);
        //while char ! end one
        //if char == diveder

        //list.add

        return files;
        }
        public void closeConnection()
        {
            client.Close();
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
