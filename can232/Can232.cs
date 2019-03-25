﻿//
//
//  CAN232 Monitor Lite, a simple demo on how to use LAWICEL CAN232 together with C#
//  All commands and featuers are not implemented and it is a quick and dirty approach.
//  There is no check if commands can be sent or not, but result will be shown.
//
//  This demo will also work with CANUSB (not all commands though) when using the
//  FTDI VCP driver. CANUSB do not have the following commands:
//  Poll One        - P[CR]
//  Poll All        - A[CR]
//  Auto Poll Flag  - X0[CR] and X1[CR]
//
//  For more information we refer to the ASCII manual for each product.
//
//  (C) 2013 LAWICEL AB, Sweden, Lars Wictorsson
//
//  Version: 1.0.0, Released 27th of February 2013
//
//  http://www.can232.com
//
//  Disclaimer:
//  This software is provided "as is" and may be used and changed to suit your own
//  needs without royalties as long as it is used with products from LAWICEL only! 
//
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CAN232_Monitor
{
    public partial class Can232 : Form
    {
        private static string receiveBuffer = "";

        public Can232()
        {
            InitializeComponent();
        }

        private void CheckHexValue(object sender, KeyPressEventArgs e, int maxLen)
        {
            if ((sender as TextBox).TextLength < maxLen)
            {
                if (char.IsDigit(e.KeyChar) || (e.KeyChar >= 'a' && e.KeyChar <= 'f') || (e.KeyChar >= 'A' && e.KeyChar <= 'F') || (e.KeyChar == '\b'))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (e.KeyChar == '\b')  // Backspace?
                {
                    e.Handled = false;
                }
                else if ((sender as TextBox).SelectionLength > 0)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void UpdateDataBoxes()
        {
            if (numDlc.Value < 8)
            {
                tbxHex8.Visible = false;
            }
            else
            {
                tbxHex8.Visible = true;
            }

            if (numDlc.Value < 7)
            {
                tbxHex7.Visible = false;
            }
            else
            {
                tbxHex7.Visible = true;
            }

            if (numDlc.Value < 6)
            {
                tbxHex6.Visible = false;
            }
            else
            {
                tbxHex6.Visible = true;
            }

            if (numDlc.Value < 5)
            {
                tbxHex5.Visible = false;
            }
            else
            {
                tbxHex5.Visible = true;
            }

            if (numDlc.Value < 4)
            {
                tbxHex4.Visible = false;
            }
            else
            {
                tbxHex4.Visible = true;
            }

            if (numDlc.Value < 3)
            {
                tbxHex3.Visible = false;
            }
            else
            {
                tbxHex3.Visible = true;
            }

            if (numDlc.Value < 2)
            {
                tbxHex2.Visible = false;
            }
            else
            {
                tbxHex2.Visible = true;
            }

            if (numDlc.Value < 1)
            {
                tbxHex1.Visible = false;
            }
            else
            {
                tbxHex1.Visible = true;
            }
        }

        private void Can232_Load(object sender, EventArgs e)
        {
            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                cmbComPort.Items.Add(portName);
            }
            cmbComPort.SelectedIndex = 0;
            cmbComSpeed.SelectedIndex = 4;
            cmbCanBitrate.SelectedIndex = 4;

        }

        private void btnComOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    // Open Port
                    serialPort.PortName = cmbComPort.SelectedItem.ToString();
                    serialPort.BaudRate = int.Parse(cmbComSpeed.SelectedItem.ToString());
                    serialPort.Open();

                    cmbComPort.Enabled = false;
                    cmbComSpeed.Enabled = false;
                    grpboxCanCommands.Enabled = true;
                    grpboxCanFrameTransmit.Enabled = true;
                    btnComOpen.Enabled = false;
                    btnComClose.Enabled = true;

                    // Set status
                    toolStripStatusLabelComPort.Text = serialPort.PortName;
                    toolStripStatusLabelSpeed.Text = serialPort.BaudRate.ToString();
                }
                else
                {
                    rtboxReceive.Text = "Failed to open COM port";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnComClose_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                toolStripStatusLabelComPort.Text = "Closed";
                toolStripStatusLabelSpeed.Text = "-";
                grpboxCanCommands.Enabled = false;
                grpboxCanFrameTransmit.Enabled = false;
                btnComClose.Enabled = false;
                btnComOpen.Enabled = true;
                cmbComPort.Enabled = true;
                cmbComSpeed.Enabled = true;
            }
        }

        private void Can232_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void btnCanOpen_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("O\r");
            }
        }

        private void btnCanClose_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("C\r");
            }
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("S");
                serialPort.Write(cmbCanBitrate.SelectedIndex.ToString());
                serialPort.Write("\r");
            }
        }

        private void btnCanVersion_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("V\r");
            }
        }

        private void btnCanFlags_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("F\r");
            }
        }

        private void btnSerNo_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("N\r");
            }
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                receiveBuffer = serialPort.ReadExisting();
                this.Invoke(new EventHandler(DisplayText));
            }
            catch (System.TimeoutException)
            {
            }
        }

        private void DisplayText(object s, EventArgs e)
        {
            int buffLen = receiveBuffer.Length;
            int buffPos = -1;
            int c = ' ';
            if (buffLen > 1)
            {
                // More than one character
                if ((buffPos = receiveBuffer.IndexOf("\r")) >= 0)
                {
                    rtboxReceive.AppendText(receiveBuffer.Substring(0, buffPos));
                    rtboxReceive.AppendText("[CR]");
                    rtboxReceive.AppendText(receiveBuffer.Substring(buffPos, (buffLen - buffPos)));
                }
                else
                {
                    rtboxReceive.AppendText(receiveBuffer);
                }
            }
            else
            {
                // Only one character
                c = receiveBuffer[0];
                if (c == 13)
                {
                    rtboxReceive.AppendText("[CR]");
                    rtboxReceive.AppendText(receiveBuffer);
                }
                else if (c == 7)
                {
                    rtboxReceive.AppendText("[BELL]\n");
                }
            }

            if (rtboxReceive.Text.Contains("t0F1"))
            {
                //full message
                string message = rtboxReceive.Text.Substring(receiveBuffer.LastIndexOf("t"), receiveBuffer.LastIndexOf("\r"));
                rtboxReceive.Text = message;
            }
        }

        private void tbxID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cboxExt.Checked == true)
            {
                CheckHexValue(sender, e, 8);
            }
            else
            {
                CheckHexValue(sender, e, 3);
            }
        }

        private void tbxHex1_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex2_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex3_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex4_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex5_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex6_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex7_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxHex8_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckHexValue(sender, e, 2);
        }

        private void tbxID_Leave(object sender, EventArgs e)
        {
            if (cboxExt.Checked == true)
            {
                if (tbxID.TextLength == 0)
                {
                    tbxID.Text = "00000000";
                }
                else if (tbxID.TextLength == 1)
                {
                    tbxID.Text = "0000000" + tbxID.Text;
                }
                else if (tbxID.TextLength == 2)
                {
                    tbxID.Text = "000000" + tbxID.Text;
                }
                else if (tbxID.TextLength == 3)
                {
                    tbxID.Text = "00000" + tbxID.Text;
                }
                else if (tbxID.TextLength == 4)
                {
                    tbxID.Text = "0000" + tbxID.Text;
                }
                else if (tbxID.TextLength == 5)
                {
                    tbxID.Text = "000" + tbxID.Text;
                }
                else if (tbxID.TextLength == 6)
                {
                    tbxID.Text = "00" + tbxID.Text;
                }
                else if (tbxID.TextLength == 7)
                {
                    tbxID.Text = "0" + tbxID.Text;
                }
            }
            else
            {
                if (tbxID.TextLength == 0)
                {
                    tbxID.Text = "000";
                }
                else if (tbxID.TextLength == 1)
                {
                    tbxID.Text = "00" + tbxID.Text;
                }
                else if (tbxID.TextLength == 2)
                {
                    tbxID.Text = "0" + tbxID.Text;
                }
            }
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex1_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex2_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex3_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex4_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex5_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex6_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex7_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void tbxHex8_Leave(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
        }

        private void numDlc_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataBoxes();
        }

        private void cboxExt_CheckedChanged(object sender, EventArgs e)
        {
            if (cboxExt.Checked == true)
            {
                tbxID.Text = "00000" + tbxID.Text;
            }
            else
            {
                tbxID.Text = tbxID.Text.Substring(tbxID.Text.Length - 3);
            }
        }

        private void cboxRtr_CheckedChanged(object sender, EventArgs e)
        {
            if (cboxRtr.Checked == true)
            {
                tbxHex1.Visible = false;
                tbxHex2.Visible = false;
                tbxHex3.Visible = false;
                tbxHex4.Visible = false;
                tbxHex5.Visible = false;
                tbxHex6.Visible = false;
                tbxHex7.Visible = false;
                tbxHex8.Visible = false;
            }
            else
            {
                UpdateDataBoxes();
            }
        }

        private void btnSendFrame_Click(object sender, EventArgs e)
        {
            string canFrameData = "";

            if (cboxRtr.Checked == true)        // RTR Frame
            {
                if (cboxExt.Checked == true)    // 29bit Frame
                {
                    canFrameData += "R";
                }
                else                            // 11bit frame
                {
                    canFrameData += "r";
                }
                canFrameData += tbxID.Text + numDlc.Value.ToString();
            }
            else                                // Normal Frame
            {
                if (cboxExt.Checked == true)    // 29bit Frame
                {
                    canFrameData += "T";
                }
                else                            // 11bit frame
                {
                    canFrameData += "t";
                }
                canFrameData += tbxID.Text + numDlc.Value.ToString();
                if (numDlc.Value >= 1)
                {
                    canFrameData += tbxHex1.Text;
                }
                if (numDlc.Value >= 2)
                {
                    canFrameData += tbxHex2.Text;
                }
                if (numDlc.Value >= 3)
                {
                    canFrameData += tbxHex3.Text;
                }
                if (numDlc.Value >= 4)
                {
                    canFrameData += tbxHex4.Text;
                }
                if (numDlc.Value >= 5)
                {
                    canFrameData += tbxHex5.Text;
                }
                if (numDlc.Value >= 6)
                {
                    canFrameData += tbxHex6.Text;
                }
                if (numDlc.Value >= 7)
                {
                    canFrameData += tbxHex7.Text;
                }
                if (numDlc.Value >= 8)
                {
                    canFrameData += tbxHex8.Text;
                }
            }
            if (serialPort.IsOpen)
            {
                serialPort.Write(canFrameData);
                serialPort.Write("\r");
            }
            lblResult.Text = "Resulting command: " + canFrameData + "[CR]";
        }

        private void btnTimeStampOn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("Z1\r");
            }
        }

        private void btnTimeStampOff_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("Z0\r");
            }
        }

        private void btnAutoOn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("X1\r");
            }
        }

        private void btnAutoOff_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("X0\r");
            }
        }

        private void btnPollOne_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("P\r");
            }
        }

        private void btnPollAll_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("A\r");
            }
        }
    }
}
