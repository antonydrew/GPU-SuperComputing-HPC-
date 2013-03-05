using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using HTMLReportEngine;
namespace HTMLReportEngineTestApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.CheckedListBox chkLstFields;
		private System.Windows.Forms.ListBox lstGroupBy;
		private System.Windows.Forms.Button btnAddGroup;
		private System.Windows.Forms.Button btnRemoveGroup;
		private System.Windows.Forms.GroupBox grpHeaderProps;
		private System.Windows.Forms.GroupBox grpFieldProps;
		private System.Windows.Forms.TextBox txtHeaderText;
		private System.Windows.Forms.Button btnHeaderBGPick;
		private System.Windows.Forms.Panel pnlHBG;
		private System.Windows.Forms.Button btnBGPick;
		private System.Windows.Forms.Panel pnlBG;
		private System.Windows.Forms.CheckBox chkTotalField;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.ComboBox cmbAlignment;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TextBox txtTitlePrefix;
		private System.Windows.Forms.Button btnGenerateReport;
		private System.Windows.Forms.Panel pnlSecBGColor;
		private System.Windows.Forms.Button btnSBGPick;
		private System.Windows.Forms.CheckBox chkGradient;
		private System.Windows.Forms.CheckBox chkTotalRow;
		private System.Windows.Forms.CheckBox chkChart;
		private System.Windows.Forms.CheckBox chkChartAtBottom;
		private System.Windows.Forms.TextBox txtChartTitle;
		private System.Windows.Forms.TextBox txtLabelText;
		private System.Windows.Forms.TextBox txtPercentageText;
		private System.Windows.Forms.TextBox txtValueText;
		private System.Windows.Forms.ComboBox cmbChangeOnField;
		private System.Windows.Forms.ComboBox cmbValueField;

		private OleDbConnection connection;
		private DataSet dataSet;
		private Hashtable fieldProps;
		private Hashtable sectionProps;
		private string selectedField="";
		private string selectedSection="";
		private System.Windows.Forms.Label lblColumnName;
		private System.Windows.Forms.GroupBox grpChartProps;
		private System.Windows.Forms.GroupBox grpReport;
		private System.Windows.Forms.CheckBox chkBorder;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label lblSectionName;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ComboBox cmbConnectionString;
		private System.Windows.Forms.Button btnExecute;
		private System.Windows.Forms.TextBox txtQuery;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlDB;
		private System.Windows.Forms.CheckBox chkOverAllTotal;
		private System.Windows.Forms.TextBox txtReportTitle;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox cmbReportFont;
		private System.Windows.Forms.Label label25;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpReport = new System.Windows.Forms.GroupBox();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.grpFieldProps = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.lblColumnName = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.cmbAlignment = new System.Windows.Forms.ComboBox();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.chkTotalField = new System.Windows.Forms.CheckBox();
			this.pnlBG = new System.Windows.Forms.Panel();
			this.btnBGPick = new System.Windows.Forms.Button();
			this.pnlHBG = new System.Windows.Forms.Panel();
			this.btnHeaderBGPick = new System.Windows.Forms.Button();
			this.txtHeaderText = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.grpHeaderProps = new System.Windows.Forms.GroupBox();
			this.lblSectionName = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.chkBorder = new System.Windows.Forms.CheckBox();
			this.grpChartProps = new System.Windows.Forms.GroupBox();
			this.cmbValueField = new System.Windows.Forms.ComboBox();
			this.cmbChangeOnField = new System.Windows.Forms.ComboBox();
			this.txtValueText = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.txtPercentageText = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.txtLabelText = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.txtChartTitle = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.chkChart = new System.Windows.Forms.CheckBox();
			this.chkTotalRow = new System.Windows.Forms.CheckBox();
			this.chkGradient = new System.Windows.Forms.CheckBox();
			this.pnlSecBGColor = new System.Windows.Forms.Panel();
			this.btnSBGPick = new System.Windows.Forms.Button();
			this.txtTitlePrefix = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.chkChartAtBottom = new System.Windows.Forms.CheckBox();
			this.btnRemoveGroup = new System.Windows.Forms.Button();
			this.btnAddGroup = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lstGroupBy = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.chkLstFields = new System.Windows.Forms.CheckedListBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.btnGenerateReport = new System.Windows.Forms.Button();
			this.pnlDB = new System.Windows.Forms.Panel();
			this.cmbConnectionString = new System.Windows.Forms.ComboBox();
			this.btnExecute = new System.Windows.Forms.Button();
			this.txtQuery = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.chkOverAllTotal = new System.Windows.Forms.CheckBox();
			this.txtReportTitle = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.cmbReportFont = new System.Windows.Forms.ComboBox();
			this.label25 = new System.Windows.Forms.Label();
			this.grpReport.SuspendLayout();
			this.grpFieldProps.SuspendLayout();
			this.grpHeaderProps.SuspendLayout();
			this.grpChartProps.SuspendLayout();
			this.pnlDB.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpReport
			// 
			this.grpReport.Controls.Add(this.chkOverAllTotal);
			this.grpReport.Controls.Add(this.txtReportTitle);
			this.grpReport.Controls.Add(this.cmbReportFont);
			this.grpReport.Controls.Add(this.label25);
			this.grpReport.Controls.Add(this.btnMoveDown);
			this.grpReport.Controls.Add(this.btnMoveUp);
			this.grpReport.Controls.Add(this.grpFieldProps);
			this.grpReport.Controls.Add(this.grpHeaderProps);
			this.grpReport.Controls.Add(this.btnRemoveGroup);
			this.grpReport.Controls.Add(this.btnAddGroup);
			this.grpReport.Controls.Add(this.label4);
			this.grpReport.Controls.Add(this.lstGroupBy);
			this.grpReport.Controls.Add(this.label3);
			this.grpReport.Controls.Add(this.chkLstFields);
			this.grpReport.Controls.Add(this.label12);
			this.grpReport.Enabled = false;
			this.grpReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpReport.Location = new System.Drawing.Point(6, 96);
			this.grpReport.Name = "grpReport";
			this.grpReport.Size = new System.Drawing.Size(715, 392);
			this.grpReport.TabIndex = 4;
			this.grpReport.TabStop = false;
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.BackColor = System.Drawing.Color.LightGray;
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveDown.Location = new System.Drawing.Point(81, 357);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(68, 24);
			this.btnMoveDown.TabIndex = 12;
			this.btnMoveDown.Text = "Down";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.BackColor = System.Drawing.Color.LightGray;
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveUp.Location = new System.Drawing.Point(9, 357);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(68, 24);
			this.btnMoveUp.TabIndex = 11;
			this.btnMoveUp.Text = "Up";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// grpFieldProps
			// 
			this.grpFieldProps.Controls.Add(this.label15);
			this.grpFieldProps.Controls.Add(this.lblColumnName);
			this.grpFieldProps.Controls.Add(this.label11);
			this.grpFieldProps.Controls.Add(this.cmbAlignment);
			this.grpFieldProps.Controls.Add(this.txtWidth);
			this.grpFieldProps.Controls.Add(this.chkTotalField);
			this.grpFieldProps.Controls.Add(this.pnlBG);
			this.grpFieldProps.Controls.Add(this.btnBGPick);
			this.grpFieldProps.Controls.Add(this.pnlHBG);
			this.grpFieldProps.Controls.Add(this.btnHeaderBGPick);
			this.grpFieldProps.Controls.Add(this.txtHeaderText);
			this.grpFieldProps.Controls.Add(this.label10);
			this.grpFieldProps.Controls.Add(this.label9);
			this.grpFieldProps.Controls.Add(this.label8);
			this.grpFieldProps.Controls.Add(this.label7);
			this.grpFieldProps.Controls.Add(this.label6);
			this.grpFieldProps.Controls.Add(this.label5);
			this.grpFieldProps.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpFieldProps.Location = new System.Drawing.Point(317, 58);
			this.grpFieldProps.Name = "grpFieldProps";
			this.grpFieldProps.Size = new System.Drawing.Size(389, 128);
			this.grpFieldProps.TabIndex = 8;
			this.grpFieldProps.TabStop = false;
			this.grpFieldProps.Text = "Field Properties";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(338, 41);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(24, 23);
			this.label15.TabIndex = 19;
			this.label15.Text = "px";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblColumnName
			// 
			this.lblColumnName.Location = new System.Drawing.Point(112, 16);
			this.lblColumnName.Name = "lblColumnName";
			this.lblColumnName.Size = new System.Drawing.Size(272, 23);
			this.lblColumnName.TabIndex = 18;
			this.lblColumnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label11.Location = new System.Drawing.Point(8, 16);
			this.label11.Name = "label11";
			this.label11.TabIndex = 17;
			this.label11.Text = "Column:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbAlignment
			// 
			this.cmbAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbAlignment.Items.AddRange(new object[] {
															  "LEFT",
															  "RIGHT",
															  "CENTER"});
			this.cmbAlignment.Location = new System.Drawing.Point(306, 69);
			this.cmbAlignment.Name = "cmbAlignment";
			this.cmbAlignment.Size = new System.Drawing.Size(72, 21);
			this.cmbAlignment.TabIndex = 8;
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(306, 43);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(32, 20);
			this.txtWidth.TabIndex = 7;
			this.txtWidth.Text = "";
			this.txtWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWidth_KeyPress);
			// 
			// chkTotalField
			// 
			this.chkTotalField.Location = new System.Drawing.Point(306, 95);
			this.chkTotalField.Name = "chkTotalField";
			this.chkTotalField.Size = new System.Drawing.Size(21, 24);
			this.chkTotalField.TabIndex = 9;
			// 
			// pnlBG
			// 
			this.pnlBG.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlBG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlBG.Location = new System.Drawing.Point(117, 99);
			this.pnlBG.Name = "pnlBG";
			this.pnlBG.Size = new System.Drawing.Size(32, 12);
			this.pnlBG.TabIndex = 13;
			this.pnlBG.Click += new System.EventHandler(this.btnBGPick_Click);
			// 
			// btnBGPick
			// 
			this.btnBGPick.BackColor = System.Drawing.Color.LightGray;
			this.btnBGPick.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBGPick.Location = new System.Drawing.Point(112, 94);
			this.btnBGPick.Name = "btnBGPick";
			this.btnBGPick.Size = new System.Drawing.Size(70, 23);
			this.btnBGPick.TabIndex = 6;
			this.btnBGPick.Text = "Pick";
			this.btnBGPick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnBGPick.Click += new System.EventHandler(this.btnBGPick_Click);
			// 
			// pnlHBG
			// 
			this.pnlHBG.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlHBG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlHBG.Location = new System.Drawing.Point(117, 73);
			this.pnlHBG.Name = "pnlHBG";
			this.pnlHBG.Size = new System.Drawing.Size(32, 12);
			this.pnlHBG.TabIndex = 11;
			this.pnlHBG.Click += new System.EventHandler(this.btnHeaderBGPick_Click);
			// 
			// btnHeaderBGPick
			// 
			this.btnHeaderBGPick.BackColor = System.Drawing.Color.LightGray;
			this.btnHeaderBGPick.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnHeaderBGPick.Location = new System.Drawing.Point(112, 68);
			this.btnHeaderBGPick.Name = "btnHeaderBGPick";
			this.btnHeaderBGPick.Size = new System.Drawing.Size(70, 23);
			this.btnHeaderBGPick.TabIndex = 5;
			this.btnHeaderBGPick.Text = "Pick";
			this.btnHeaderBGPick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnHeaderBGPick.Click += new System.EventHandler(this.btnHeaderBGPick_Click);
			// 
			// txtHeaderText
			// 
			this.txtHeaderText.Location = new System.Drawing.Point(112, 43);
			this.txtHeaderText.Name = "txtHeaderText";
			this.txtHeaderText.Size = new System.Drawing.Size(120, 20);
			this.txtHeaderText.TabIndex = 4;
			this.txtHeaderText.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(248, 42);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(56, 23);
			this.label10.TabIndex = 8;
			this.label10.Text = "Width:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 42);
			this.label9.Name = "label9";
			this.label9.TabIndex = 7;
			this.label9.Text = "Header Text:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(204, 94);
			this.label8.Name = "label8";
			this.label8.TabIndex = 6;
			this.label8.Text = "Is Total Field:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 94);
			this.label7.Name = "label7";
			this.label7.TabIndex = 5;
			this.label7.Text = "BackColor:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 68);
			this.label6.Name = "label6";
			this.label6.TabIndex = 4;
			this.label6.Text = "Header BackColor:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(240, 68);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 23);
			this.label5.TabIndex = 3;
			this.label5.Text = "Alignment:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// grpHeaderProps
			// 
			this.grpHeaderProps.Controls.Add(this.lblSectionName);
			this.grpHeaderProps.Controls.Add(this.label16);
			this.grpHeaderProps.Controls.Add(this.chkBorder);
			this.grpHeaderProps.Controls.Add(this.grpChartProps);
			this.grpHeaderProps.Controls.Add(this.chkChart);
			this.grpHeaderProps.Controls.Add(this.chkTotalRow);
			this.grpHeaderProps.Controls.Add(this.chkGradient);
			this.grpHeaderProps.Controls.Add(this.pnlSecBGColor);
			this.grpHeaderProps.Controls.Add(this.btnSBGPick);
			this.grpHeaderProps.Controls.Add(this.txtTitlePrefix);
			this.grpHeaderProps.Controls.Add(this.label13);
			this.grpHeaderProps.Controls.Add(this.label14);
			this.grpHeaderProps.Controls.Add(this.chkChartAtBottom);
			this.grpHeaderProps.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpHeaderProps.Location = new System.Drawing.Point(160, 191);
			this.grpHeaderProps.Name = "grpHeaderProps";
			this.grpHeaderProps.Size = new System.Drawing.Size(546, 192);
			this.grpHeaderProps.TabIndex = 10;
			this.grpHeaderProps.TabStop = false;
			this.grpHeaderProps.Text = "Section Properties";
			// 
			// lblSectionName
			// 
			this.lblSectionName.Location = new System.Drawing.Point(82, 16);
			this.lblSectionName.Name = "lblSectionName";
			this.lblSectionName.Size = new System.Drawing.Size(166, 23);
			this.lblSectionName.TabIndex = 20;
			this.lblSectionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label16
			// 
			this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label16.Location = new System.Drawing.Point(7, 16);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(73, 23);
			this.label16.TabIndex = 19;
			this.label16.Text = "Section:";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkBorder
			// 
			this.chkBorder.Location = new System.Drawing.Point(16, 155);
			this.chkBorder.Name = "chkBorder";
			this.chkBorder.Size = new System.Drawing.Size(128, 24);
			this.chkBorder.TabIndex = 17;
			this.chkBorder.Text = "Show Chart Border";
			// 
			// grpChartProps
			// 
			this.grpChartProps.Controls.Add(this.cmbValueField);
			this.grpChartProps.Controls.Add(this.cmbChangeOnField);
			this.grpChartProps.Controls.Add(this.txtValueText);
			this.grpChartProps.Controls.Add(this.label24);
			this.grpChartProps.Controls.Add(this.txtPercentageText);
			this.grpChartProps.Controls.Add(this.label23);
			this.grpChartProps.Controls.Add(this.txtLabelText);
			this.grpChartProps.Controls.Add(this.label22);
			this.grpChartProps.Controls.Add(this.label21);
			this.grpChartProps.Controls.Add(this.label20);
			this.grpChartProps.Controls.Add(this.txtChartTitle);
			this.grpChartProps.Controls.Add(this.label19);
			this.grpChartProps.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpChartProps.Location = new System.Drawing.Point(255, 11);
			this.grpChartProps.Name = "grpChartProps";
			this.grpChartProps.Size = new System.Drawing.Size(282, 172);
			this.grpChartProps.TabIndex = 6;
			this.grpChartProps.TabStop = false;
			this.grpChartProps.Text = "Chart Properties";
			// 
			// cmbValueField
			// 
			this.cmbValueField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbValueField.Location = new System.Drawing.Point(110, 68);
			this.cmbValueField.Name = "cmbValueField";
			this.cmbValueField.Size = new System.Drawing.Size(160, 21);
			this.cmbValueField.TabIndex = 2;
			// 
			// cmbChangeOnField
			// 
			this.cmbChangeOnField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbChangeOnField.Location = new System.Drawing.Point(110, 45);
			this.cmbChangeOnField.Name = "cmbChangeOnField";
			this.cmbChangeOnField.Size = new System.Drawing.Size(160, 21);
			this.cmbChangeOnField.TabIndex = 1;
			// 
			// txtValueText
			// 
			this.txtValueText.Location = new System.Drawing.Point(110, 140);
			this.txtValueText.Name = "txtValueText";
			this.txtValueText.Size = new System.Drawing.Size(160, 20);
			this.txtValueText.TabIndex = 5;
			this.txtValueText.Text = "";
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(6, 140);
			this.label24.Name = "label24";
			this.label24.TabIndex = 35;
			this.label24.Text = "Value Text:";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPercentageText
			// 
			this.txtPercentageText.Location = new System.Drawing.Point(110, 116);
			this.txtPercentageText.Name = "txtPercentageText";
			this.txtPercentageText.Size = new System.Drawing.Size(160, 20);
			this.txtPercentageText.TabIndex = 4;
			this.txtPercentageText.Text = "";
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(6, 116);
			this.label23.Name = "label23";
			this.label23.TabIndex = 33;
			this.label23.Text = "Percnetage Text:";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtLabelText
			// 
			this.txtLabelText.Location = new System.Drawing.Point(110, 92);
			this.txtLabelText.Name = "txtLabelText";
			this.txtLabelText.Size = new System.Drawing.Size(160, 20);
			this.txtLabelText.TabIndex = 3;
			this.txtLabelText.Text = "";
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(6, 92);
			this.label22.Name = "label22";
			this.label22.TabIndex = 31;
			this.label22.Text = "Label Text:";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(6, 68);
			this.label21.Name = "label21";
			this.label21.TabIndex = 29;
			this.label21.Text = "Value Field:";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(6, 44);
			this.label20.Name = "label20";
			this.label20.TabIndex = 27;
			this.label20.Text = "Change on Field:";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtChartTitle
			// 
			this.txtChartTitle.Location = new System.Drawing.Point(110, 20);
			this.txtChartTitle.Name = "txtChartTitle";
			this.txtChartTitle.Size = new System.Drawing.Size(160, 20);
			this.txtChartTitle.TabIndex = 0;
			this.txtChartTitle.Text = "";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(6, 20);
			this.label19.Name = "label19";
			this.label19.TabIndex = 25;
			this.label19.Text = "Chart Title:";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkChart
			// 
			this.chkChart.Location = new System.Drawing.Point(16, 130);
			this.chkChart.Name = "chkChart";
			this.chkChart.Size = new System.Drawing.Size(96, 24);
			this.chkChart.TabIndex = 4;
			this.chkChart.Text = "Include Chart";
			this.chkChart.CheckedChanged += new System.EventHandler(this.chkChart_CheckedChanged);
			// 
			// chkTotalRow
			// 
			this.chkTotalRow.Location = new System.Drawing.Point(128, 106);
			this.chkTotalRow.Name = "chkTotalRow";
			this.chkTotalRow.Size = new System.Drawing.Size(120, 24);
			this.chkTotalRow.TabIndex = 3;
			this.chkTotalRow.Text = "Include Total Row";
			// 
			// chkGradient
			// 
			this.chkGradient.Location = new System.Drawing.Point(16, 106);
			this.chkGradient.Name = "chkGradient";
			this.chkGradient.Size = new System.Drawing.Size(88, 24);
			this.chkGradient.TabIndex = 2;
			this.chkGradient.Text = "Gradient BG";
			// 
			// pnlSecBGColor
			// 
			this.pnlSecBGColor.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlSecBGColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSecBGColor.Location = new System.Drawing.Point(88, 73);
			this.pnlSecBGColor.Name = "pnlSecBGColor";
			this.pnlSecBGColor.Size = new System.Drawing.Size(32, 12);
			this.pnlSecBGColor.TabIndex = 16;
			this.pnlSecBGColor.Click += new System.EventHandler(this.btnSBGPick_Click);
			// 
			// btnSBGPick
			// 
			this.btnSBGPick.BackColor = System.Drawing.Color.LightGray;
			this.btnSBGPick.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSBGPick.Location = new System.Drawing.Point(82, 68);
			this.btnSBGPick.Name = "btnSBGPick";
			this.btnSBGPick.Size = new System.Drawing.Size(70, 23);
			this.btnSBGPick.TabIndex = 1;
			this.btnSBGPick.Text = "Pick";
			this.btnSBGPick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnSBGPick.Click += new System.EventHandler(this.btnSBGPick_Click);
			// 
			// txtTitlePrefix
			// 
			this.txtTitlePrefix.Location = new System.Drawing.Point(82, 42);
			this.txtTitlePrefix.Name = "txtTitlePrefix";
			this.txtTitlePrefix.Size = new System.Drawing.Size(136, 20);
			this.txtTitlePrefix.TabIndex = 0;
			this.txtTitlePrefix.Text = "";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(7, 41);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(73, 23);
			this.label13.TabIndex = 13;
			this.label13.Text = "Title Prefix:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(7, 68);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(73, 23);
			this.label14.TabIndex = 12;
			this.label14.Text = "BackColor:";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkChartAtBottom
			// 
			this.chkChartAtBottom.Location = new System.Drawing.Point(128, 130);
			this.chkChartAtBottom.Name = "chkChartAtBottom";
			this.chkChartAtBottom.Size = new System.Drawing.Size(112, 24);
			this.chkChartAtBottom.TabIndex = 5;
			this.chkChartAtBottom.Text = "Chart at Bottom";
			// 
			// btnRemoveGroup
			// 
			this.btnRemoveGroup.BackColor = System.Drawing.Color.LightGray;
			this.btnRemoveGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoveGroup.Location = new System.Drawing.Point(157, 124);
			this.btnRemoveGroup.Name = "btnRemoveGroup";
			this.btnRemoveGroup.Size = new System.Drawing.Size(24, 24);
			this.btnRemoveGroup.TabIndex = 2;
			this.btnRemoveGroup.Text = "<";
			this.btnRemoveGroup.Click += new System.EventHandler(this.btnRemoveGroup_Click);
			// 
			// btnAddGroup
			// 
			this.btnAddGroup.BackColor = System.Drawing.Color.LightGray;
			this.btnAddGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAddGroup.Location = new System.Drawing.Point(157, 96);
			this.btnAddGroup.Name = "btnAddGroup";
			this.btnAddGroup.Size = new System.Drawing.Size(24, 24);
			this.btnAddGroup.TabIndex = 1;
			this.btnAddGroup.Text = ">";
			this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(184, 39);
			this.label4.Name = "label4";
			this.label4.TabIndex = 4;
			this.label4.Text = "Section(Group By):";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lstGroupBy
			// 
			this.lstGroupBy.Location = new System.Drawing.Point(188, 63);
			this.lstGroupBy.Name = "lstGroupBy";
			this.lstGroupBy.Size = new System.Drawing.Size(122, 121);
			this.lstGroupBy.TabIndex = 3;
			this.lstGroupBy.SelectedIndexChanged += new System.EventHandler(this.lstGroupBy_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 39);
			this.label3.Name = "label3";
			this.label3.TabIndex = 2;
			this.label3.Text = "Report Fields:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkLstFields
			// 
			this.chkLstFields.Location = new System.Drawing.Point(8, 63);
			this.chkLstFields.Name = "chkLstFields";
			this.chkLstFields.Size = new System.Drawing.Size(141, 289);
			this.chkLstFields.TabIndex = 0;
			this.chkLstFields.SelectedIndexChanged += new System.EventHandler(this.chkLstFields_SelectedIndexChanged);
			// 
			// colorDialog1
			// 
			this.colorDialog1.Color = System.Drawing.Color.LightGray;
			this.colorDialog1.FullOpen = true;
			// 
			// btnGenerateReport
			// 
			this.btnGenerateReport.BackColor = System.Drawing.Color.LightGray;
			this.btnGenerateReport.Enabled = false;
			this.btnGenerateReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnGenerateReport.Location = new System.Drawing.Point(610, 495);
			this.btnGenerateReport.Name = "btnGenerateReport";
			this.btnGenerateReport.Size = new System.Drawing.Size(112, 25);
			this.btnGenerateReport.TabIndex = 5;
			this.btnGenerateReport.Text = "Generate Report";
			this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
			// 
			// pnlDB
			// 
			this.pnlDB.BackColor = System.Drawing.Color.Gainsboro;
			this.pnlDB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlDB.Controls.Add(this.cmbConnectionString);
			this.pnlDB.Controls.Add(this.btnExecute);
			this.pnlDB.Controls.Add(this.txtQuery);
			this.pnlDB.Controls.Add(this.label2);
			this.pnlDB.Controls.Add(this.label1);
			this.pnlDB.Location = new System.Drawing.Point(7, 8);
			this.pnlDB.Name = "pnlDB";
			this.pnlDB.Size = new System.Drawing.Size(714, 86);
			this.pnlDB.TabIndex = 14;
			// 
			// cmbConnectionString
			// 
			this.cmbConnectionString.Items.AddRange(new object[] {
																	 "Provider=MSDAORA; Data Source=ORACLE8i7; Persist Security Info=False; Integrated " +
																	 "Security=yes",
																	 "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\\Access.mdb",
																	 "Provider=SQLOLEDB; Data Source=SERVERNAME; Initial Catalog=DATABASE; User ID=USER" +
																	 "; password=PASS"});
			this.cmbConnectionString.Location = new System.Drawing.Point(104, 7);
			this.cmbConnectionString.Name = "cmbConnectionString";
			this.cmbConnectionString.Size = new System.Drawing.Size(600, 21);
			this.cmbConnectionString.TabIndex = 0;
			// 
			// btnExecute
			// 
			this.btnExecute.BackColor = System.Drawing.Color.LemonChiffon;
			this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnExecute.Location = new System.Drawing.Point(624, 54);
			this.btnExecute.Name = "btnExecute";
			this.btnExecute.Size = new System.Drawing.Size(80, 24);
			this.btnExecute.TabIndex = 2;
			this.btnExecute.Text = "Execute";
			this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
			// 
			// txtQuery
			// 
			this.txtQuery.Location = new System.Drawing.Point(104, 33);
			this.txtQuery.Multiline = true;
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Size = new System.Drawing.Size(513, 44);
			this.txtQuery.TabIndex = 1;
			this.txtQuery.Text = "";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Gainsboro;
			this.label2.Location = new System.Drawing.Point(0, 32);
			this.label2.Name = "label2";
			this.label2.TabIndex = 14;
			this.label2.Text = "SQL Query:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Gainsboro;
			this.label1.Location = new System.Drawing.Point(0, 6);
			this.label1.Name = "label1";
			this.label1.TabIndex = 13;
			this.label1.Text = "Connection String:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "html";
			this.saveFileDialog1.FileName = "Report";
			this.saveFileDialog1.Filter = "HTML Files|*.html";
			// 
			// chkOverAllTotal
			// 
			this.chkOverAllTotal.Location = new System.Drawing.Point(514, 15);
			this.chkOverAllTotal.Name = "chkOverAllTotal";
			this.chkOverAllTotal.Size = new System.Drawing.Size(168, 24);
			this.chkOverAllTotal.TabIndex = 23;
			this.chkOverAllTotal.Text = "Show Over All Total Row";
			// 
			// txtReportTitle
			// 
			this.txtReportTitle.Enabled = false;
			this.txtReportTitle.Location = new System.Drawing.Point(73, 16);
			this.txtReportTitle.Name = "txtReportTitle";
			this.txtReportTitle.Size = new System.Drawing.Size(191, 20);
			this.txtReportTitle.TabIndex = 21;
			this.txtReportTitle.Text = "HTML Report";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 14);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(73, 23);
			this.label12.TabIndex = 22;
			this.label12.Text = "Report Title:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbReportFont
			// 
			this.cmbReportFont.Enabled = false;
			this.cmbReportFont.Location = new System.Drawing.Point(340, 16);
			this.cmbReportFont.Name = "cmbReportFont";
			this.cmbReportFont.Size = new System.Drawing.Size(160, 21);
			this.cmbReportFont.TabIndex = 19;
			this.cmbReportFont.Text = "Arial";
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(264, 16);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(73, 23);
			this.label25.TabIndex = 20;
			this.label25.Text = "Report Font:";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(726, 525);
			this.Controls.Add(this.btnGenerateReport);
			this.Controls.Add(this.grpReport);
			this.Controls.Add(this.pnlDB);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HTML Report Engine  Application";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.grpReport.ResumeLayout(false);
			this.grpFieldProps.ResumeLayout(false);
			this.grpHeaderProps.ResumeLayout(false);
			this.grpChartProps.ResumeLayout(false);
			this.pnlDB.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnExecute_Click(object sender, System.EventArgs e)
		{
			try 
			{
				if(cmbConnectionString.Text.Trim() == "" || txtQuery.Text.Trim()=="") 
				{
					MessageBox.Show("Connection string and SQL Query must be entered.");
					return;
				}
				this.Cursor = Cursors.WaitCursor;
				pnlDB.Enabled = false;
				btnGenerateReport.Enabled = false;
				connection = new OleDbConnection(cmbConnectionString.Text);
				OleDbDataAdapter adapter = new OleDbDataAdapter(txtQuery.Text, connection);
				sectionProps = new Hashtable();
				fieldProps = new Hashtable();
				dataSet = new DataSet();
				adapter.Fill(dataSet);
				if(dataSet.Tables.Count ==0 || dataSet.Tables[0].Rows.Count == 0) 
					throw new Exception("No data available");
				LoadFields();
				CreateFieldProps();
				grpReport.Enabled = true;
				cmbReportFont.Enabled = true;
				txtReportTitle.Enabled = true;
				pnlDB.Enabled = true;
				btnGenerateReport.Enabled = true;
				this.Cursor = Cursors.Default;
			} 
			catch(Exception exp) 
			{
				this.Cursor = Cursors.Default;
				MessageBox.Show(exp.Message);
				pnlDB.Enabled = true;
				grpReport.Enabled = false;
				cmbReportFont.Enabled = false;
				txtReportTitle.Enabled = false;		
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			cmbReportFont.Items.Clear();
			cmbReportFont.Text = "Arial";
			foreach(FontFamily f in System.Drawing.FontFamily.Families) 
			{
				cmbReportFont.Items.Add(f.Name);
			}
		}

		private void LoadFields() 
		{
			chkLstFields.Items.Clear();
			cmbChangeOnField.Items.Clear();
			cmbValueField.Items.Clear();
			cmbValueField.Items.Add("COUNT");
			foreach(DataColumn dc in dataSet.Tables[0].Columns) 
			{
				chkLstFields.Items.Add(dc.ColumnName);
				cmbChangeOnField.Items.Add(dc.ColumnName);
				cmbValueField.Items.Add(dc.ColumnName);
			}
		}

		private void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			if(chkLstFields.SelectedItems.Count > 0) 
			{
				if( ! lstGroupBy.Items.Contains(chkLstFields.SelectedItems[0].ToString())) 
				{
					lstGroupBy.Items.Add(chkLstFields.SelectedItems[0].ToString());
					CreateSectionProps(chkLstFields.SelectedItems[0].ToString());
				}
			}
		}

		private void btnRemoveGroup_Click(object sender, System.EventArgs e)
		{
			if(lstGroupBy.SelectedItems.Count > 0) 
			{
				string secName = lstGroupBy.SelectedItems[0].ToString();
				lstGroupBy.Items.Remove(lstGroupBy.SelectedItems[0]);
				sectionProps.Remove(secName);
			}
		}

		private void CreateFieldProps() 
		{
			foreach(object field in chkLstFields.Items) 
			{
				fieldProps.Add(field.ToString(), new Field(field.ToString(), field.ToString()));
			}
		}

		private void CreateSectionProps(string groupByColumnName) 
		{
			sectionProps.Add(groupByColumnName, new Section(groupByColumnName,""));
		}

		private void chkLstFields_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SaveFieldAttributes(selectedField);
			if(chkLstFields.SelectedItems.Count > 0) 
			{
				LoadFieldAttributes(chkLstFields.SelectedItems[0].ToString());
			}
		}

		private void LoadFieldAttributes(string fieldName) 
		{
			if(fieldProps[fieldName] != null) 
			{
				Field field = (Field)fieldProps[fieldName];
				lblColumnName.Text = field.FieldName;
				txtHeaderText.Text = field.HeaderName;
				pnlHBG.BackColor = field.HeaderBackColor;
				pnlBG.BackColor = field.BackColor;
				txtWidth.Text = field.Width.ToString();
				cmbAlignment.Text = field.Alignment.ToString();
				chkTotalField.Checked = field.isTotalField;
				selectedField = fieldName;
			}
		}
		private void SaveFieldAttributes(string fieldName) 
		{
			if(fieldProps[fieldName] != null) 
			{
				Field field = (Field)fieldProps[fieldName];
				field.FieldName = lblColumnName.Text;
				field.HeaderName = txtHeaderText.Text;
				field.HeaderBackColor = pnlHBG.BackColor;
				field.BackColor = pnlBG.BackColor;
				field.Width = int.Parse(txtWidth.Text);
				field.Alignment = (cmbAlignment.Text == "RIGHT") ? ALIGN.RIGHT : (cmbAlignment.Text == "CENTER") ? ALIGN.CENTER : ALIGN.LEFT;
				field.isTotalField = chkTotalField.Checked;			
			}
		}

		private void lstGroupBy_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SaveSectionAttributes(selectedSection);
			if(lstGroupBy.SelectedItems.Count > 0) 
			{
				LoadSectionAttributes(lstGroupBy.SelectedItems[0].ToString());
			}
		}

		private void LoadSectionAttributes(string sectionName) 
		{
			if(sectionProps[sectionName] != null)  
			{
				Section section = (Section)sectionProps[sectionName];
				lblSectionName.Text = sectionName;
				txtTitlePrefix.Text = section.TitlePrefix;
				pnlSecBGColor.BackColor = section.BackColor;
				chkGradient.Checked = section.GradientBackground;
				chkTotalRow.Checked = section.IncludeTotal;
				chkChart.Checked = section.IncludeChart;
				chkChartAtBottom.Checked = section.ChartShowAtBottom;
				chkBorder.Checked = section.ChartShowBorder;
				if(section.IncludeChart) 
				{
					grpChartProps.Enabled = true;
					txtChartTitle.Text = section.ChartTitle;
					cmbChangeOnField.Text = section.ChartChangeOnField;
					cmbValueField.Text = section.ChartValueField=="" ? "COUNT" : section.ChartValueField;
					txtLabelText.Text = section.ChartLabelHeader;
					txtPercentageText.Text = section.ChartPercentageHeader;
					txtValueText.Text = section.ChartValueHeader;
				} 
				else 
				{
					grpChartProps.Enabled = false;
					txtChartTitle.Text = "";
					cmbChangeOnField.Text = "";
					cmbValueField.Text = "";
					txtLabelText.Text = "";
					txtPercentageText.Text = "";
					txtValueText.Text = "";
				}
			}
			selectedSection = sectionName;
		}
		private void SaveSectionAttributes(string sectionName) 
		{
			if(sectionProps[sectionName] != null) 
			{
				Section section = (Section)sectionProps[sectionName];
				section.TitlePrefix = txtTitlePrefix.Text;
				section.BackColor = pnlSecBGColor.BackColor;
				section.GradientBackground = chkGradient.Checked;
				section.IncludeTotal = chkTotalRow.Checked;
				section.IncludeChart = chkChart.Checked;
				section.ChartShowAtBottom = chkChartAtBottom.Checked;
				section.ChartShowBorder = chkBorder.Checked;
				if(chkChart.Checked) 
				{
					section.ChartTitle = txtChartTitle.Text;
					section.ChartChangeOnField = cmbChangeOnField.Text;
					section.ChartValueField = cmbValueField.Text=="" ? "COUNT" : cmbValueField.Text;
					section.ChartLabelHeader = txtLabelText.Text.Trim()=="" ? "Label" : txtLabelText.Text;
					section.ChartPercentageHeader = txtPercentageText.Text.Trim()=="" ? "Percentage" : txtPercentageText.Text;
					section.ChartValueHeader = txtValueText.Text.Trim()=="" ? "Value" : txtValueText.Text;
				} 
				else 
				{
					section.ChartTitle = "";
					section.ChartChangeOnField = "";
					section.ChartValueField = "";
					section.ChartLabelHeader = "";
					section.ChartPercentageHeader = "";
					section.ChartValueHeader = "";
				}
			}
		}

		private void chkChart_CheckedChanged(object sender, System.EventArgs e)
		{
			grpChartProps.Enabled = chkChart.Checked;
		}

		private void btnHeaderBGPick_Click(object sender, System.EventArgs e)
		{
			DialogResult result = colorDialog1.ShowDialog(this);
			if(result == DialogResult.OK)
				pnlHBG.BackColor = colorDialog1.Color;
		}

		private void btnBGPick_Click(object sender, System.EventArgs e)
		{
			DialogResult result = colorDialog1.ShowDialog(this);
			if(result == DialogResult.OK)
				pnlBG.BackColor = colorDialog1.Color;
		}

		private void btnSBGPick_Click(object sender, System.EventArgs e)
		{
			DialogResult result = colorDialog1.ShowDialog(this);
			if(result == DialogResult.OK)
				pnlSecBGColor.BackColor = colorDialog1.Color;
		}

		private void btnGenerateReport_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			grpReport.Enabled = false;
			pnlDB.Enabled = false;
			btnGenerateReport.Enabled = false;
			SaveFieldAttributes(selectedField);
			SaveSectionAttributes(selectedSection);
			Report report = new Report();
			report.ReportTitle = txtReportTitle.Text;
			report.ReportFont = cmbReportFont.Text;
			report.IncludeTotal = chkOverAllTotal.Checked;

			foreach(object obj in chkLstFields.CheckedItems) 
			{
				Field field = (Field)fieldProps[obj.ToString()];
				report.ReportFields.Add( field );
				if(field.isTotalField)
					report.TotalFields.Add(field.FieldName);
			}

			Section section = null;
			foreach(object obj in lstGroupBy.Items) 
			{
				if(report.Sections.Count == 0) 
				{
					section = (Section)sectionProps[obj.ToString()];
					report.Sections.Add(section);
				}
				else 
				{
					section.SubSection = (Section)sectionProps[obj.ToString()];
					section = section.SubSection;
				}
			}

			report.ReportSource = dataSet;
			this.Cursor = Cursors.Default;
			DialogResult res = saveFileDialog1.ShowDialog(this);
			if(res == DialogResult.OK) 
			{
				this.Cursor = Cursors.WaitCursor;
				report.SaveReport(saveFileDialog1.FileName);
				this.Cursor = Cursors.Default;
			}
			grpReport.Enabled = true;
			pnlDB.Enabled = true;
			btnGenerateReport.Enabled = true;
		}

		private void btnMoveUp_Click(object sender, System.EventArgs e)
		{
			int index = chkLstFields.SelectedIndices[0];
			if(index!=0) 
			{
				ArrayList list = new ArrayList();
				CheckedListBox cb = new CheckedListBox();
				cb.Items.AddRange(chkLstFields.Items);
				for(int i=0; i<chkLstFields.CheckedItems.Count; i++) 
				{
					cb.SetItemCheckState(cb.Items.IndexOf(chkLstFields.CheckedItems[i]),CheckState.Checked);					
				}
				list.AddRange(chkLstFields.Items);
				ArrayList newlist = new ArrayList(list);
				newlist[index] = list[index-1];
				newlist[index-1] = list[index];
				chkLstFields.Items.Clear();
				chkLstFields.Items.AddRange((string[])newlist.ToArray(typeof(string)));
				for(int i=0; i<cb.CheckedItems.Count; i++) 
				{
					chkLstFields.SetItemCheckState(chkLstFields.Items.IndexOf(cb.CheckedItems[i]),CheckState.Checked);					
				}
				chkLstFields.SelectedItem = chkLstFields.Items[index-1];
			}
		}

		private void btnMoveDown_Click(object sender, System.EventArgs e)
		{
			int index = chkLstFields.SelectedIndices[0];
			if(index!=chkLstFields.Items.Count-1) 
			{
				CheckedListBox cb = new CheckedListBox();
				cb.Items.AddRange(chkLstFields.Items);
				for(int i=0; i<chkLstFields.CheckedItems.Count; i++) 
				{
					cb.SetItemCheckState(cb.Items.IndexOf(chkLstFields.CheckedItems[i]),CheckState.Checked);					
				}
				ArrayList list = new ArrayList();
				list.AddRange(chkLstFields.Items);
				ArrayList newlist = new ArrayList(list);
				newlist[index] = list[index+1];
				newlist[index+1] = list[index];
				chkLstFields.Items.Clear();
				chkLstFields.Items.AddRange((string[])newlist.ToArray(typeof(string)));
				for(int i=0; i<cb.CheckedItems.Count; i++) 
				{
					chkLstFields.SetItemCheckState(chkLstFields.Items.IndexOf(cb.CheckedItems[i]),CheckState.Checked);					
				}				
				chkLstFields.SelectedItem = chkLstFields.Items[index+1];
			}
		}

		private void txtWidth_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			try
			{				
				//Condition to Allow Only Numberic in Text Box				
				if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 45 && e.KeyChar != 59)
					e.Handled = true; // Remove the character
			}
			catch(Exception err)
			{
				MessageBox.Show("Invalid Width");											
			}
		}
	}
}
