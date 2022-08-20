using System.Diagnostics;

namespace JRA
{
    public partial class splash : Form
    {


        public splash()
        {
            InitializeComponent();         
            
            timer1.Tick += TimerEventProcessor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            timer1.Start();
        }

        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(object myObject, EventArgs myEventArgs)
        {
            timer1.Stop();
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
            form1.Closed += (s, args) => this.Close();

        }

        private void splash_Load(object sender, EventArgs e)
        {

        }

        private ImageList imageList1;
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::JRA.Properties.Resources.UoS;
            this.pictureBox1.Location = new System.Drawing.Point(259, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 225);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.pictureBox2.Image = global::JRA.Properties.Resources.PICKnEW2;
            this.pictureBox2.Location = new System.Drawing.Point(-5, 274);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(192, 149);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 314);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Alex\'s JRA - 3D Holofan Controller©";
            // 
            // splash
            // 
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(757, 418);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Name = "splash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.splash_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void splash_Load_1(object sender, EventArgs e)
        {

        }

        private ImageList imageList2;
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private PictureBox pictureBox2;
        private Label label1;
    }
}