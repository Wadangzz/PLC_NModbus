namespace ModbusMaster
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            WriteAddress = new TextBox();
            btnWrite = new Button();
            whiteDevice = new TextBox();
            ipAddress = new TextBox();
            data = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            lbx_Log = new ListBox();
            label5 = new Label();
            btnWriteDevice = new Button();
            button2 = new Button();
            label6 = new Label();
            readAddress = new TextBox();
            readDevice = new TextBox();
            btnRead = new Button();
            btnReadDevice = new Button();
            label7 = new Label();
            label8 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("맑은 고딕", 15F);
            button1.Location = new Point(30, 100);
            button1.Name = "button1";
            button1.Size = new Size(205, 44);
            button1.TabIndex = 0;
            button1.Text = "ModbusServer 접속";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // WriteAddress
            // 
            WriteAddress.Location = new Point(344, 74);
            WriteAddress.Name = "WriteAddress";
            WriteAddress.Size = new Size(100, 23);
            WriteAddress.TabIndex = 1;
            // 
            // btnWrite
            // 
            btnWrite.Location = new Point(344, 126);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(100, 32);
            btnWrite.TabIndex = 2;
            btnWrite.Text = "입력";
            btnWrite.UseVisualStyleBackColor = true;
            btnWrite.Click += btnWrite_Click;
            // 
            // whiteDevice
            // 
            whiteDevice.Location = new Point(460, 74);
            whiteDevice.Name = "whiteDevice";
            whiteDevice.Size = new Size(100, 23);
            whiteDevice.TabIndex = 1;
            // 
            // ipAddress
            // 
            ipAddress.Location = new Point(31, 67);
            ipAddress.Name = "ipAddress";
            ipAddress.Size = new Size(204, 23);
            ipAddress.TabIndex = 3;
            // 
            // data
            // 
            data.Location = new Point(460, 100);
            data.Name = "data";
            data.Size = new Size(100, 23);
            data.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(274, 77);
            label1.Name = "label1";
            label1.Size = new Size(57, 17);
            label1.TabIndex = 4;
            label1.Text = "Address";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.Location = new Point(460, 43);
            label2.Name = "label2";
            label2.Size = new Size(83, 17);
            label2.TabIndex = 4;
            label2.Text = "D데이터쓰기";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.Location = new Point(31, 43);
            label3.Name = "label3";
            label3.Size = new Size(63, 17);
            label3.TabIndex = 4;
            label3.Text = "Server IP";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.Location = new Point(274, 103);
            label4.Name = "label4";
            label4.Size = new Size(37, 17);
            label4.TabIndex = 4;
            label4.Text = "Data";
            // 
            // lbx_Log
            // 
            lbx_Log.FormattingEnabled = true;
            lbx_Log.ItemHeight = 15;
            lbx_Log.Location = new Point(30, 281);
            lbx_Log.Name = "lbx_Log";
            lbx_Log.Size = new Size(530, 199);
            lbx_Log.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.Location = new Point(344, 43);
            label5.Name = "label5";
            label5.Size = new Size(86, 17);
            label5.TabIndex = 4;
            label5.Text = "M메모리쓰기";
            // 
            // btnWriteDevice
            // 
            btnWriteDevice.Location = new Point(460, 126);
            btnWriteDevice.Name = "btnWriteDevice";
            btnWriteDevice.Size = new Size(100, 32);
            btnWriteDevice.TabIndex = 6;
            btnWriteDevice.Text = "입력";
            btnWriteDevice.UseVisualStyleBackColor = true;
            btnWriteDevice.Click += btnWriteDevice_Click;
            // 
            // button2
            // 
            button2.Font = new Font("맑은 고딕", 15F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button2.Location = new Point(31, 150);
            button2.Name = "button2";
            button2.Size = new Size(205, 44);
            button2.TabIndex = 7;
            button2.Text = "ModbusServer 해제";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.Location = new Point(274, 208);
            label6.Name = "label6";
            label6.Size = new Size(57, 17);
            label6.TabIndex = 4;
            label6.Text = "Address";
            // 
            // readAddress
            // 
            readAddress.Location = new Point(344, 205);
            readAddress.Name = "readAddress";
            readAddress.Size = new Size(100, 23);
            readAddress.TabIndex = 10;
            // 
            // readDevice
            // 
            readDevice.Location = new Point(460, 205);
            readDevice.Name = "readDevice";
            readDevice.Size = new Size(100, 23);
            readDevice.TabIndex = 10;
            // 
            // btnRead
            // 
            btnRead.Location = new Point(345, 233);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(100, 32);
            btnRead.TabIndex = 11;
            btnRead.Text = "입력";
            btnRead.UseVisualStyleBackColor = true;
            btnRead.Click += btnRead_Click;
            // 
            // btnReadDevice
            // 
            btnReadDevice.Location = new Point(460, 233);
            btnReadDevice.Name = "btnReadDevice";
            btnReadDevice.Size = new Size(100, 32);
            btnReadDevice.TabIndex = 12;
            btnReadDevice.Text = "입력";
            btnReadDevice.UseVisualStyleBackColor = true;
            btnReadDevice.Click += btnReadDevice_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold);
            label7.Location = new Point(344, 177);
            label7.Name = "label7";
            label7.Size = new Size(86, 17);
            label7.TabIndex = 13;
            label7.Text = "M메모리읽기";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold);
            label8.Location = new Point(460, 177);
            label8.Name = "label8";
            label8.Size = new Size(83, 17);
            label8.TabIndex = 13;
            label8.Text = "D데이터읽기";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 529);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(btnReadDevice);
            Controls.Add(btnRead);
            Controls.Add(readDevice);
            Controls.Add(readAddress);
            Controls.Add(button2);
            Controls.Add(btnWriteDevice);
            Controls.Add(lbx_Log);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label6);
            Controls.Add(label1);
            Controls.Add(ipAddress);
            Controls.Add(data);
            Controls.Add(whiteDevice);
            Controls.Add(btnWrite);
            Controls.Add(WriteAddress);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox WriteAddress;
        private Button btnWrite;
        private TextBox whiteDevice;
        private TextBox ipAddress;
        private TextBox data;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ListBox lbx_Log;
        private Label label5;
        private Button btnWriteDevice;
        private Button button2;
        private Label label6;
        private TextBox readAddress;
        private TextBox readDevice;
        private Button btnRead;
        private Button btnReadDevice;
        private Label label7;
        private Label label8;
    }
}
