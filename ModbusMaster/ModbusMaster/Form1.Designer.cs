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
            components = new System.ComponentModel.Container();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            WriteAddress = new TextBox();
            btnWrite = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("맑은 고딕", 15F);
            button1.Location = new Point(22, 22);
            button1.Name = "button1";
            button1.Size = new Size(205, 84);
            button1.TabIndex = 0;
            button1.Text = "ModbusServer 접속";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // WriteAddress
            // 
            WriteAddress.Location = new Point(254, 36);
            WriteAddress.Name = "WriteAddress";
            WriteAddress.Size = new Size(100, 23);
            WriteAddress.TabIndex = 1;
            // 
            // btnWrite
            // 
            btnWrite.Location = new Point(254, 74);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(100, 32);
            btnWrite.TabIndex = 2;
            btnWrite.Text = "입력";
            btnWrite.UseVisualStyleBackColor = true;
            btnWrite.Click += btnWrite_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(502, 529);
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
        private System.Windows.Forms.Timer timer1;
        private TextBox WriteAddress;
        private Button btnWrite;
    }
}
