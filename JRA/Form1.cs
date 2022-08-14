using System.Diagnostics;

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

        private void Form1_Load(object sender, EventArgs e)
        {

            /*
             * disable buttons sfor flow control to enure user has to "connect" first
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            */

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


            //update GUI wuth new list inludeing this uploaded one LATEST
            fan.getFilenames();
            updateListBoxWithFiles(fan.getListOfFilenamesOnDevie());

        }

        //on click will create a bin version of the file provided called the same with .bin extension rather than
        //provdied i.e test.png becomes test.bin
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


            bb.makeFile(file, location);

            string message = "Bin file created";
            string title = "Complete";
            //MessageBox.Show(message, title);
            }
            catch
            {

            }
            //make the bin
            redneckBinCreator rb = new redneckBinCreator();
            rb.startShow(file);

            
            //pass this same filename to the send function but change the extension to bin as locartion will be the same
            string editedfilename = Path.GetFileNameWithoutExtension(file) + ".bin";
            Console.WriteLine(editedfilename);
            Console.WriteLine(location);
            //send the file from where it was put
            fan.sendFile(location);

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        //connect to device button
        private void button3_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            fan.connectToAccessPoint();
            fan.doConnection();
            fan.getFilenames();
            list = fan.getListOfFilenamesOnDevie();
            Thread.Sleep(500);
            updateListBoxWithFiles(list);
            //if the fan is connected after above enable buttons
            if (fan.getIsConnected())
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
            }
        }
        private void updateListBoxWithFiles(List<string> list)
        {
            foreach(string fName in list)
            {
                //if fname has * remove
                listBox1.Items.Add(fName);
            }

            
        }

        //next button
        private void button5_Click(object sender, EventArgs e)
        {
            fan.doConnection();
            fan.getFilenames();
            fan.nextImage();
        }

        //pause button
        private void button4_Click(object sender, EventArgs e)
        {
            fan.doConnection();
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
            fan.doConnection();
            fan.getFilenames();
            fan.deleteSD();
            //update GUI
            clearFileNameBox();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
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