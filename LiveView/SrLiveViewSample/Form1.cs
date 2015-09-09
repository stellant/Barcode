using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Keyence.AutoID;

namespace SrLiveViewSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //
            // Register handler for reading data received event
            //
            this.barcodeReaderControl1.OnDataReceived += barcodeReaderControl1_OnDataReceived;
            //
            // Set the type of reader to connect
            //
            this.barcodeReaderControl1.ReaderType = ReaderType.SR_1000;
            //
            // Set the interface to connect the reader
            //
            this.barcodeReaderControl1.Comm.Interface = Interface.Ethernet;
            //
            // Set the IP address of the reader to connect
            //
            this.barcodeReaderControl1.IpAddress = "192.168.100.100";
            //
            // Set the TCP port number of the reader to connect
            //
            // Comannd port number must be different from Data port number
            //
            this.barcodeReaderControl1.Ether.CommandPort = 9003;
            this.barcodeReaderControl1.Ether.DataPort = 9004;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //
                // Connect to the Reader
                //
                this.barcodeReaderControl1.Connect();
                this.textBox1.Text = "Connected successfully";
                if (this.barcodeReaderControl1.Comm.Interface == Interface.USB)
                {
                    //
                    // Send "SKCLOSE" in order to occupy the data port connection
                    //
                    this.barcodeReaderControl1.SKCLOSE();
                }
                else
                {
                    //
                    // Make sure that command response character string is specified.
                    //
                    string val = this.barcodeReaderControl1.RP("610");
                    switch (this.barcodeReaderControl1.ReaderType)
                    {
                        case ReaderType.SR_1000:
                        case ReaderType.SR_700:
                            if (!val.Equals("1"))
                            {
                                this.textBox1.Text = "Set Baseic command response string to Detailed response.";
                            }
                            break;
                        case ReaderType.SR_D100:
                        case ReaderType.SR_750:
                            if (!val.Equals("0"))
                            {
                                this.textBox1.Text = "Disable the setting of Specify response character.";
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.textBox1.Text = ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            try
            {
                //
                // Send LON command
                //
                this.barcodeReaderControl1.LON();
            }
            catch (CommandException cex)
            {
                //
                // ExtErrCode shows the number of command error
                //
                this.textBox1.Text = "Command err," + cex.ExtErrCode;
            }
            catch (Exception ex)
            {
                this.textBox1.Text = ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            try
            {
                //
                // Send LOFF command
                //
                this.barcodeReaderControl1.LOFF();
            }
            catch (Exception ex)
            {
                this.textBox1.Text = ex.Message;
            }
        }

        private void barcodeReaderControl1_OnDataReceived(object sender, OnDataReceivedEventArgs e)
        {
            //
            // Delegate display processing of the received data to the textBox
            //
            this.textBox2.Invoke(new updateTextBoxDelegate(updateTextBox), e.data);
        }

        //
        // Delegated function of the textBox
        //
        private delegate void updateTextBoxDelegate(byte[] data);
        private void updateTextBox(byte[] data)
        {
            //
            // Display the received data to the textBox 
            //
            this.textBox2.Text = Encoding.GetEncoding("Shift_JIS").GetString(data);

            //
            // Image file saving process
            //
            bool saveImage;
            if (this.textBox2.Text.StartsWith("ERROR"))
            {
                saveImage = radioButtonImageErrSave.Checked;

            }
            else
            {
                saveImage = radioButtonImageOkSave.Checked;
            }
            if (saveImage)
            {
                try
                {
                    //
                    // Get file path of saved file
                    //
                    string srcFile = this.barcodeReaderControl1.LSIMG();
                    string dstFile = srcFile.Split('\\')[2];
                    //
                    // Get image file
                    //
                    this.barcodeReaderControl1.GetFile(srcFile, dstFile);
                    MessageBox.Show(dstFile, "Image file");
                }
                catch (Exception ex)
                {
                    this.textBox1.Text = ex.Message;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            try
            {
                //
                // Start processing of LiveView
                //
                this.barcodeReaderControl1.StartLiveView();
            }
            catch (Exception ex)
            {
                this.textBox1.Text = ex.Message;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.ReaderType = ReaderType.SR_D100;
                groupBox3.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.ReaderType = ReaderType.SR_750;
                groupBox3.Enabled = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.ReaderType = ReaderType.SR_1000;
                groupBox3.Enabled = true;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton5.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.Comm.Interface = Interface.USB;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton4.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.Comm.Interface = Interface.Ethernet;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton6.Checked)
            {
                this.barcodeReaderControl1.StopLiveView();
                this.barcodeReaderControl1.ReaderType = ReaderType.SR_700;
                groupBox3.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
