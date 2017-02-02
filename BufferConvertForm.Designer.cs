namespace DudeMugen
{
    partial class BufferConvertForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.commandBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBufferTimeBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.directionBufferTimeBox = new System.Windows.Forms.NumericUpDown();
            this.outputBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.convertButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.commandVarBox = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.elemBufferTimeBox = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.commandNameBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.buttonBufferTimeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionBufferTimeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandVarBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.elemBufferTimeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // commandBox
            // 
            this.commandBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandBox.Location = new System.Drawing.Point(12, 29);
            this.commandBox.Name = "commandBox";
            this.commandBox.Size = new System.Drawing.Size(550, 20);
            this.commandBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Command";
            // 
            // buttonBufferTimeBox
            // 
            this.buttonBufferTimeBox.Location = new System.Drawing.Point(12, 128);
            this.buttonBufferTimeBox.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.buttonBufferTimeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.buttonBufferTimeBox.Name = "buttonBufferTimeBox";
            this.buttonBufferTimeBox.Size = new System.Drawing.Size(120, 20);
            this.buttonBufferTimeBox.TabIndex = 2;
            this.buttonBufferTimeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Button Buffer Time";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(154, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Direction Buffer Time";
            // 
            // directionBufferTimeBox
            // 
            this.directionBufferTimeBox.Location = new System.Drawing.Point(157, 128);
            this.directionBufferTimeBox.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.directionBufferTimeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.directionBufferTimeBox.Name = "directionBufferTimeBox";
            this.directionBufferTimeBox.Size = new System.Drawing.Size(120, 20);
            this.directionBufferTimeBox.TabIndex = 4;
            this.directionBufferTimeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // outputBox
            // 
            this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputBox.Location = new System.Drawing.Point(12, 215);
            this.outputBox.MaxLength = 65536;
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputBox.Size = new System.Drawing.Size(550, 472);
            this.outputBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Output";
            // 
            // convertButton
            // 
            this.convertButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.convertButton.Location = new System.Drawing.Point(234, 180);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(115, 23);
            this.convertButton.TabIndex = 8;
            this.convertButton.Text = "Convert!";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(439, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Variable Number";
            // 
            // commandVarBox
            // 
            this.commandVarBox.Location = new System.Drawing.Point(442, 128);
            this.commandVarBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.commandVarBox.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.commandVarBox.Name = "commandVarBox";
            this.commandVarBox.Size = new System.Drawing.Size(120, 20);
            this.commandVarBox.TabIndex = 9;
            this.commandVarBox.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(296, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Element Buffer Time";
            // 
            // elemBufferTimeBox
            // 
            this.elemBufferTimeBox.Location = new System.Drawing.Point(299, 128);
            this.elemBufferTimeBox.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.elemBufferTimeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.elemBufferTimeBox.Name = "elemBufferTimeBox";
            this.elemBufferTimeBox.Size = new System.Drawing.Size(120, 20);
            this.elemBufferTimeBox.TabIndex = 11;
            this.elemBufferTimeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Command Name";
            // 
            // commandNameBox
            // 
            this.commandNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandNameBox.Location = new System.Drawing.Point(12, 78);
            this.commandNameBox.Name = "commandNameBox";
            this.commandNameBox.Size = new System.Drawing.Size(550, 20);
            this.commandNameBox.TabIndex = 13;
            // 
            // BufferConvertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 699);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.commandNameBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.elemBufferTimeBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.commandVarBox);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.directionBufferTimeBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBufferTimeBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.commandBox);
            this.MinimumSize = new System.Drawing.Size(550, 738);
            this.Name = "BufferConvertForm";
            this.Text = "Buffering System Converter by Jesuszilla";
            ((System.ComponentModel.ISupportInitialize)(this.buttonBufferTimeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionBufferTimeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandVarBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.elemBufferTimeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox commandBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown buttonBufferTimeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown directionBufferTimeBox;
        private System.Windows.Forms.TextBox outputBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown commandVarBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown elemBufferTimeBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox commandNameBox;
    }
}

