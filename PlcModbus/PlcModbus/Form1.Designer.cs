namespace PlcModbus
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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(31, 33);
            button1.Name = "button1";
            button1.Size = new Size(109, 65);
            button1.TabIndex = 0;
            button1.Text = "PLC 연결";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 108);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(396, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(16, 17);
            toolStripStatusLabel1.Text = "...";
            // 
            // button2
            // 
            button2.Location = new Point(146, 33);
            button2.Name = "button2";
            button2.Size = new Size(109, 65);
            button2.TabIndex = 2;
            button2.Text = "PLC 연결 해제";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(261, 33);
            button3.Name = "button3";
            button3.Size = new Size(109, 32);
            button3.TabIndex = 3;
            button3.Text = "DataBase 생성";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(261, 66);
            button4.Name = "button4";
            button4.Size = new Size(109, 32);
            button4.TabIndex = 4;
            button4.Text = "DataBase 초기화";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 130);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(statusStrip1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button button2;
        private Button button3;
        private Button button4;
    }
}
