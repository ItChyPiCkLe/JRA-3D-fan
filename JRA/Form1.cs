using com.sun.org.apache.bcel.@internal.generic;
using sun.tools.tree;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace JRA
{
    public partial class Form1 : Form
    {
        binCreator bb = new binCreator();
        Fan fan = new Fan();
        string file = "";

        public Form1()
        {
            Console.WriteLine("test");
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.Text = "3d fan control software";          
            




        }
        private void disableEnableButtons(bool yayOrNay)
        {
            button1.Enabled = yayOrNay;
            button2.Enabled = yayOrNay;
            button4.Enabled = yayOrNay;
            button5.Enabled = yayOrNay;
            button6.Enabled = yayOrNay;
        }
        private void Form1_Load(object sender, EventArgs e)
        {            
            //start thread to keep cheing if we online
            Thread thread1 = new Thread(fan.checkOnlineThread);
            thread1.Start();

            //start another thread to display result to user
            //wont access on another thread need event listener
            Thread thread2 = new Thread(showUserWeConnected);
            thread2.Start();

            //animate logo
            Thread thread3 = new Thread(animateLogo);
            thread3.Start();


        }





        private void Form1_Close(object sender, EventArgs e)
        {        
            Application.Exit();
            Environment.Exit(0);            
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int size = -1;
            string file = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();            
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(file);
            //send file
            fan.sendFile(file);
            //clear anything in list box to start
            clearFileNameBox();
            //update GUI wuth new list inludeing this uploaded one LATEST
            fan.initiliseStream();
            fan.getFilenames();
            updateListBoxWithFiles(fan.getListOfFilenamesOnDevie());

        }

        //on click will create a bin version of the file provided called the same with .bin extension rather than
        //provdied i.e test.png becomes test.bin

        //consider disconnect from network to avoid clashing of TCP object between mine and their soft
        private void button2_Click(object sender, EventArgs e)
        {
            int size = -1;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.

            int marker = file.LastIndexOf('\\');
            // using the method
            string filename = Path.GetFileName(file);
            string location = "";               

            try
            {
                filename = filename.Substring(0, filename.LastIndexOf('.'));
           
     
            location = file.Substring(0, marker);
            location += "\\" + filename + ".bin";


            //bb.makeFile(file, location);

            string message = "Bin file created";
            string title = "Complete";
            //MessageBox.Show(message, title);
            }
            catch
            {

            }
            //disconnect from fan network so it dont confuse their software then make bin and reconnect before sending
            fan.disconnect();


            //make the bin
            redneckBinCreator rb = new redneckBinCreator();
            rb.startShow(file);

            
            //pass this same filename to the send function but change the extension to bin as locartion will be the same
            string editedfilename = Path.GetFileNameWithoutExtension(file) + ".bin";
            Console.WriteLine(editedfilename);
            Console.WriteLine(location);
            //send the file from where it was put
            Thread.Sleep(100);

            fan.connectToAccessPoint();            
            Thread.Sleep(2000);
            fan.initiliseStream();
            fan.getFilenames();
            Thread.Sleep(500);
            fan.sendFile(location);

            //delete old contents
            clearFileNameBox();
            //
            //TODO
            //get filenames as below and update (theres a prob with disconnect after sending maybe just init stream etc again)
            //get and display new inc this one
            //fan.getFilenames();
            //updateListBoxWithFiles(fan.getListOfFilenamesOnDevie());


        }       

        
        private void showUserWeConnected()
        {
            while (true)
            {
                if (fan.getIsConnected())
                {
                    //wait 2 secs to allow us to b fully connected
                    Thread.Sleep(2500);
                    this.pictureBox8.Invoke((MethodInvoker)delegate {
                        // Running on the UI thread
                        
                        this.pictureBox8.Image = Properties.Resources.pawwOK;
                        disableEnableButtons(true);
                    });
                }
                else
                {
                    this.pictureBox8.Invoke((MethodInvoker)delegate {
                        // Running on the UI thread
                        
                        this.pictureBox8.Image = Properties.Resources.pawwNO;
                        disableEnableButtons(false);
                    });
                }
                Thread.Sleep(1000);
            }
        }

        private void animateLogo()
        {
            while (true)
            {
                pictureBox1.Image = Properties.Resources.UoS;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos2;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos3;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos4;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos5;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos6;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos7;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos8;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos9;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos10;
                Thread.Sleep(200);
                pictureBox1.Image = Properties.Resources.uos11;
                Thread.Sleep(200);

            }
        }

        //connect to device button
        private void button3_Click(object sender, EventArgs e)
        {
            //make sure we not connected to anything to start
            fan.disconnect();
            Thread.Sleep(3000);

            List<string> list = new List<string>();
            fan.connectToAccessPoint();
            Thread.Sleep(1000);
            fan.initiliseStream();
            fan.getFilenames();
            updateListBoxWithFiles(fan.getListOfFilenamesOnDevie());

            



        }
        private void updateListBoxWithFiles(List<string> list)
        {
            if (list.Count == 1)
            {
                listBox1.Items.Add("No Files On Device");
            }
            else
            {
                foreach (string fName in list)
                {
                    listBox1.Items.Add(fName);
                }
            }
                

            
        }

        //next button
        private void button5_Click(object sender, EventArgs e)
        {
            fan.initiliseStream();
            fan.getFilenames();
            fan.nextImage();
        }

        //pause button
        private void button4_Click(object sender, EventArgs e)
        {
            fan.initiliseStream();
            fan.getFilenames();
            fan.pauseImage();
        }
        private void clearFileNameBox()
        {
            listBox1.Items.Clear();
        }

        //format sd card
        private void button6_Click(object sender, EventArgs e)
        {
            fan.initiliseStream();
            fan.getFilenames();
            fan.deleteSD();
            //update GUI
            clearFileNameBox();
            listBox1.Items.Add("No Files On Device");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}