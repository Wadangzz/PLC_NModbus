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
            components = new System.ComponentModel.Container();
            button1 = new Button();
            Timer = new System.Windows.Forms.Timer(components);
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            button2 = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(27, 33);
            button1.Name = "button1";
            button1.Size = new Size(109, 65);
            button1.TabIndex = 0;
            button1.Text = "PLC 연결";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Timer
            // 
            Timer.Enabled = true;
            Timer.Tick += Timer_Tick;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 379);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(409, 22);
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
            button2.Location = new Point(142, 33);
            button2.Name = "button2";
            button2.Size = new Size(109, 65);
            button2.TabIndex = 2;
            button2.Text = "PLC 연결 해제";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 401);
            Controls.Add(button2);
            Controls.Add(statusStrip1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private System.Windows.Forms.Timer Timer;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button button2;
    }
}
