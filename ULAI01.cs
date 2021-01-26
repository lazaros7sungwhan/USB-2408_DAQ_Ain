// ==============================================================================

//  File:                         ULAI01.CS

//  Library Call Demonstrated:    Mccdaq.MccBoard.AIn()

//  Purpose:                      Reads an A/D Input Channel.

//  Demonstration:                Displays the analog input on a user-specified
//                                channel.

//  Other Library Calls:          Mccdaq.MccBoard.ToEngUnits()
//                                MccDaq.MccService.ErrHandling()

//  Special Requirements:         Board 0 must have an A/D converter.
//                                Analog signal on an input channel.

// ==============================================================================

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;

using AnalogIO;
using MccDaq;
using ErrorDefs;

namespace ULAI01
{
	public class frmDataDisplay : System.Windows.Forms.Form
	{
        
        // Create a new MccBoard object for Board 0
        MccDaq.MccBoard DaqBoard = new MccDaq.MccBoard(0);

        MccDaq.Range Range;
        private int HighChan, NumAIChans;
        private int ADResolution;
        public Label lblShowVolts2;
        public Label lblShowVolts_2;
        public Label lblShowData_2;
        AnalogIO.clsAnalogIO AIOProps = new AnalogIO.clsAnalogIO();
        private void frmDataDisplay_Load(object eventSender, System.EventArgs eventArgs)
        {

            int LowChan;
            MccDaq.TriggerType DefaultTrig;

            InitUL();

            // determine the number of analog channels and their capabilities
            int ChannelType = clsAnalogIO.ANALOGINPUT;
            NumAIChans = AIOProps.FindAnalogChansOfType(DaqBoard, ChannelType,
                out ADResolution, out Range, out LowChan, out DefaultTrig);

            if (NumAIChans == 0)
            {
                lblInstruction.Text = "Board " + DaqBoard.BoardNum.ToString() +
                    " does not have analog input channels.";
                cmdStartConvert.Enabled = false;
                txtNumChan.Enabled = false;
            }
            else
            {
                string CurFunc = "MccBoard.AIn";
                if (ADResolution > 16)
                    CurFunc = "MccBoard.AIn32";
                lblDemoFunction.Text = "Demonstration of " + CurFunc;
                lblInstruction.Text = "Board " + DaqBoard.BoardNum.ToString() +
                    " collecting analog data using " + CurFunc + 
                    " and Range of " + Range.ToString() + ".";
                HighChan = LowChan + NumAIChans - 1;
                this.lblChanPrompt.Text = "Enter a channel (" 
                    + LowChan.ToString() + " - " + HighChan.ToString() + "):";
            }
        }

        private void cmdStartConvert_Click(object eventSender, System.EventArgs eventArgs)
        {

            if (tmrConvert.Enabled)
            {
                cmdStartConvert.Text = "Start";
                tmrConvert.Enabled = false;
            }
            else
            {
                cmdStartConvert.Text = "Stop";
                tmrConvert.Enabled = true;
            }

        }

        private void tmrConvert_Tick(object eventSender, System.EventArgs eventArgs)
        {

            float EngUnits;
            double HighResEngUnits, HighResEngUnits_2;
            MccDaq.ErrorInfo ULStat;
            System.UInt16 DataValue;
            System.UInt32 DataValue32, DataValue32_2;
            int Chan;
            int Options = 0;

            tmrConvert.Stop();

            //  Collect the data by calling AIn member function of MccBoard object
            //   Parameters:
            //     Chan       :the input channel number
            //     Range      :the Range for the board.
            //     DataValue  :the name for the value collected

            //  set input channel
            bool ValidChan = int.TryParse(txtNumChan.Text, out Chan);
            if (ValidChan)
            {
                if (Chan > HighChan) Chan = HighChan;
                txtNumChan.Text = Chan.ToString();
            }
            
            if (ADResolution > 16)
            {
                /*ULStat = DaqBoard.AIn32(Chan, Range, out DataValue32, Options);*/
                ULStat = DaqBoard.AIn32(0, Range, out DataValue32, Options);
                //  Convert raw data to Volts by calling ToEngUnits
                //  (member function of MccBoard class)
                ULStat = DaqBoard.ToEngUnits32(Range, DataValue32, out HighResEngUnits);

                lblShowData.Text = DataValue32.ToString();                //  print the counts
                lblShowVolts.Text = HighResEngUnits.ToString("F5") + " Volts"; //  print the voltage

                ULStat = DaqBoard.AIn32(2, Range, out DataValue32_2, Options);
                ULStat = DaqBoard.ToEngUnits32(Range, DataValue32_2, out HighResEngUnits_2);

                lblShowData_2.Text = DataValue32_2.ToString();                //  print the counts
                lblShowVolts_2.Text = HighResEngUnits_2.ToString("F5") + " Volts"; //  print the voltage
            }
            else
            {
                ULStat = DaqBoard.AIn(Chan, Range, out DataValue);

                //  Convert raw data to Volts by calling ToEngUnits
                //  (member function of MccBoard class)
                ULStat = DaqBoard.ToEngUnits(Range, DataValue, out EngUnits);

                lblShowData.Text = DataValue.ToString();                //  print the counts
                lblShowVolts.Text = EngUnits.ToString("F4") + " Volts"; //  print the voltage
                /*lblShowVolts2.Text=*/
            }

            tmrConvert.Start();
        }

        private void cmdStopConvert_Click(object eventSender, System.EventArgs eventArgs)
        {

            tmrConvert.Enabled = false;
            Application.Exit();

        }

        private void InitUL()
        {

            //  Initiate error handling
            //   activating error handling will trap errors like
            //   bad channel numbers and non-configured conditions.
            //   Parameters:
            //     MccDaq.ErrorReporting.PrintAll :all warnings and errors encountered will be printed
            //     MccDaq.ErrorHandling.StopAll   :if an error is encountered, the program will stop

            clsErrorDefs.ReportError = MccDaq.ErrorReporting.PrintAll;
            clsErrorDefs.HandleError = MccDaq.ErrorHandling.StopAll;
            MccDaq.ErrorInfo ULStat = MccDaq.MccService.ErrHandling
                (clsErrorDefs.ReportError, clsErrorDefs.HandleError);

        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
	    
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdStartConvert = new System.Windows.Forms.Button();
            this.cmdStopConvert = new System.Windows.Forms.Button();
            this.txtNumChan = new System.Windows.Forms.TextBox();
            this.tmrConvert = new System.Windows.Forms.Timer(this.components);
            this.lblShowVolts = new System.Windows.Forms.Label();
            this.lblVoltsRead = new System.Windows.Forms.Label();
            this.lblValueRead = new System.Windows.Forms.Label();
            this.lblChanPrompt = new System.Windows.Forms.Label();
            this.lblDemoFunction = new System.Windows.Forms.Label();
            this.lblShowData = new System.Windows.Forms.Label();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblShowVolts2 = new System.Windows.Forms.Label();
            this.lblShowVolts_2 = new System.Windows.Forms.Label();
            this.lblShowData_2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdStartConvert
            // 
            this.cmdStartConvert.BackColor = System.Drawing.SystemColors.Control;
            this.cmdStartConvert.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdStartConvert.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdStartConvert.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdStartConvert.Location = new System.Drawing.Point(244, 297);
            this.cmdStartConvert.Name = "cmdStartConvert";
            this.cmdStartConvert.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdStartConvert.Size = new System.Drawing.Size(78, 38);
            this.cmdStartConvert.TabIndex = 5;
            this.cmdStartConvert.Text = "Start";
            this.cmdStartConvert.UseVisualStyleBackColor = false;
            this.cmdStartConvert.Click += new System.EventHandler(this.cmdStartConvert_Click);
            // 
            // cmdStopConvert
            // 
            this.cmdStopConvert.BackColor = System.Drawing.SystemColors.Control;
            this.cmdStopConvert.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdStopConvert.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdStopConvert.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdStopConvert.Location = new System.Drawing.Point(348, 297);
            this.cmdStopConvert.Name = "cmdStopConvert";
            this.cmdStopConvert.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdStopConvert.Size = new System.Drawing.Size(78, 38);
            this.cmdStopConvert.TabIndex = 6;
            this.cmdStopConvert.Text = "Quit";
            this.cmdStopConvert.UseVisualStyleBackColor = false;
            this.cmdStopConvert.Click += new System.EventHandler(this.cmdStopConvert_Click);
            // 
            // txtNumChan
            // 
            this.txtNumChan.AcceptsReturn = true;
            this.txtNumChan.BackColor = System.Drawing.SystemColors.Window;
            this.txtNumChan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumChan.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNumChan.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumChan.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtNumChan.Location = new System.Drawing.Point(356, 134);
            this.txtNumChan.MaxLength = 0;
            this.txtNumChan.Name = "txtNumChan";
            this.txtNumChan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtNumChan.Size = new System.Drawing.Size(49, 26);
            this.txtNumChan.TabIndex = 0;
            this.txtNumChan.Text = "0";
            this.txtNumChan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tmrConvert
            // 
            this.tmrConvert.Tick += new System.EventHandler(this.tmrConvert_Tick);
            // 
            // lblShowVolts
            // 
            this.lblShowVolts.BackColor = System.Drawing.SystemColors.Window;
            this.lblShowVolts.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblShowVolts.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowVolts.ForeColor = System.Drawing.Color.Blue;
            this.lblShowVolts.Location = new System.Drawing.Point(312, 238);
            this.lblShowVolts.Name = "lblShowVolts";
            this.lblShowVolts.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblShowVolts.Size = new System.Drawing.Size(120, 24);
            this.lblShowVolts.TabIndex = 8;
            this.lblShowVolts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVoltsRead
            // 
            this.lblVoltsRead.BackColor = System.Drawing.SystemColors.Window;
            this.lblVoltsRead.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblVoltsRead.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoltsRead.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblVoltsRead.Location = new System.Drawing.Point(12, 238);
            this.lblVoltsRead.Name = "lblVoltsRead";
            this.lblVoltsRead.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblVoltsRead.Size = new System.Drawing.Size(276, 24);
            this.lblVoltsRead.TabIndex = 7;
            this.lblVoltsRead.Text = "Value converted to voltage:";
            this.lblVoltsRead.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValueRead
            // 
            this.lblValueRead.BackColor = System.Drawing.SystemColors.Window;
            this.lblValueRead.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblValueRead.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueRead.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblValueRead.Location = new System.Drawing.Point(24, 191);
            this.lblValueRead.Name = "lblValueRead";
            this.lblValueRead.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblValueRead.Size = new System.Drawing.Size(276, 24);
            this.lblValueRead.TabIndex = 3;
            this.lblValueRead.Text = "Value read from selected channel:";
            this.lblValueRead.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblChanPrompt
            // 
            this.lblChanPrompt.BackColor = System.Drawing.SystemColors.Window;
            this.lblChanPrompt.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChanPrompt.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChanPrompt.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblChanPrompt.Location = new System.Drawing.Point(16, 134);
            this.lblChanPrompt.Name = "lblChanPrompt";
            this.lblChanPrompt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChanPrompt.Size = new System.Drawing.Size(326, 24);
            this.lblChanPrompt.TabIndex = 1;
            this.lblChanPrompt.Text = "Enter the Channel to display: ";
            this.lblChanPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDemoFunction
            // 
            this.lblDemoFunction.BackColor = System.Drawing.SystemColors.Window;
            this.lblDemoFunction.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDemoFunction.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDemoFunction.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblDemoFunction.Location = new System.Drawing.Point(12, 13);
            this.lblDemoFunction.Name = "lblDemoFunction";
            this.lblDemoFunction.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDemoFunction.Size = new System.Drawing.Size(429, 37);
            this.lblDemoFunction.TabIndex = 2;
            this.lblDemoFunction.Text = "Demonstration of MccBoard.AIn";
            this.lblDemoFunction.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblShowData
            // 
            this.lblShowData.Font = new System.Drawing.Font("Arial", 8F);
            this.lblShowData.ForeColor = System.Drawing.Color.Blue;
            this.lblShowData.Location = new System.Drawing.Point(312, 191);
            this.lblShowData.Name = "lblShowData";
            this.lblShowData.Size = new System.Drawing.Size(120, 24);
            this.lblShowData.TabIndex = 9;
            this.lblShowData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblInstruction
            // 
            this.lblInstruction.BackColor = System.Drawing.SystemColors.Window;
            this.lblInstruction.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblInstruction.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruction.ForeColor = System.Drawing.Color.Red;
            this.lblInstruction.Location = new System.Drawing.Point(14, 51);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblInstruction.Size = new System.Drawing.Size(428, 59);
            this.lblInstruction.TabIndex = 10;
            this.lblInstruction.Text = "Demonstration of MccBoard.AIn";
            this.lblInstruction.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblShowVolts2
            // 
            this.lblShowVolts2.BackColor = System.Drawing.SystemColors.Window;
            this.lblShowVolts2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblShowVolts2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowVolts2.ForeColor = System.Drawing.Color.Blue;
            this.lblShowVolts2.Location = new System.Drawing.Point(493, 238);
            this.lblShowVolts2.Name = "lblShowVolts2";
            this.lblShowVolts2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblShowVolts2.Size = new System.Drawing.Size(120, 24);
            this.lblShowVolts2.TabIndex = 8;
            this.lblShowVolts2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblShowVolts_2
            // 
            this.lblShowVolts_2.BackColor = System.Drawing.SystemColors.Window;
            this.lblShowVolts_2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblShowVolts_2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowVolts_2.ForeColor = System.Drawing.Color.Blue;
            this.lblShowVolts_2.Location = new System.Drawing.Point(483, 238);
            this.lblShowVolts_2.Name = "lblShowVolts_2";
            this.lblShowVolts_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblShowVolts_2.Size = new System.Drawing.Size(120, 24);
            this.lblShowVolts_2.TabIndex = 8;
            this.lblShowVolts_2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblShowData_2
            // 
            this.lblShowData_2.Font = new System.Drawing.Font("Arial", 8F);
            this.lblShowData_2.ForeColor = System.Drawing.Color.Blue;
            this.lblShowData_2.Location = new System.Drawing.Point(483, 191);
            this.lblShowData_2.Name = "lblShowData_2";
            this.lblShowData_2.Size = new System.Drawing.Size(120, 24);
            this.lblShowData_2.TabIndex = 9;
            this.lblShowData_2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmDataDisplay
            // 
            this.AcceptButton = this.cmdStartConvert;
            this.AutoScaleBaseSize = new System.Drawing.Size(9, 19);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(933, 804);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblShowData_2);
            this.Controls.Add(this.lblShowData);
            this.Controls.Add(this.cmdStartConvert);
            this.Controls.Add(this.cmdStopConvert);
            this.Controls.Add(this.txtNumChan);
            this.Controls.Add(this.lblShowVolts_2);
            this.Controls.Add(this.lblShowVolts2);
            this.Controls.Add(this.lblShowVolts);
            this.Controls.Add(this.lblVoltsRead);
            this.Controls.Add(this.lblValueRead);
            this.Controls.Add(this.lblChanPrompt);
            this.Controls.Add(this.lblDemoFunction);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Location = new System.Drawing.Point(182, 100);
            this.Name = "frmDataDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Universal Library Analog Input";
            this.Load += new System.EventHandler(this.frmDataDisplay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

	#endregion

        #region Form initialization, variables, and entry point

        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmDataDisplay());
		}

        public frmDataDisplay()
        {

            // This call is required by the Windows Form Designer.
            InitializeComponent();

        }

        // Form overrides dispose to clean up the component list.
        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(Disposing);
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;
        public ToolTip ToolTip1;
        public Button cmdStartConvert;
        public Button cmdStopConvert;
        public TextBox txtNumChan;
        public Timer tmrConvert;
        public Label lblShowVolts;
        public Label lblVoltsRead;
        public Label lblValueRead;
        public Label lblChanPrompt;
        public Label lblDemoFunction;
        public Label lblInstruction;
        public Label lblShowData;

        #endregion

    }
}