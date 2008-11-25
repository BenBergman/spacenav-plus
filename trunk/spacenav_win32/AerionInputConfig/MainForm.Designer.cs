/*
   AerionInputConfig
 
   Copyright (C)    Christian Bayer 2008
					Friedrich-Schiller University of Jena
					Bauhaus University of Weimar

   This library is free software; you can redistribute it and/or
   modify it under the terms of the GNU Lesser General Public
   License as published by the Free Software Foundation; either
   version 2.1 of the License, or (at your option) any later version.

   This library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public
   License along with this library; if not, see http://www.gnu.org/licenses .

*/

namespace AerionInputConfig
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.YScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.XScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.ZScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.YRotScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.ZRotScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.XRotScaleFactorBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TranslationThreshold = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TranslationFunctionCombo = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.RotationThreshold = new System.Windows.Forms.NumericUpDown();
            this.RotationFunctionCombo = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.OKButton = new System.Windows.Forms.Button();
            this.CButton = new System.Windows.Forms.Button();
            this.ApplicationCombo = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.DeviceBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.YScaleFactorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XScaleFactorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZScaleFactorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YRotScaleFactorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZRotScaleFactorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XRotScaleFactorBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TranslationThreshold)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // YScaleFactorBox
            // 
            this.YScaleFactorBox.DecimalPlaces = 1;
            this.YScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.YScaleFactorBox.Location = new System.Drawing.Point(166, 40);
            this.YScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.YScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.YScaleFactorBox.Name = "YScaleFactorBox";
            this.YScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.YScaleFactorBox.TabIndex = 1;
            // 
            // XScaleFactorBox
            // 
            this.XScaleFactorBox.DecimalPlaces = 1;
            this.XScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.XScaleFactorBox.Location = new System.Drawing.Point(166, 14);
            this.XScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.XScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.XScaleFactorBox.Name = "XScaleFactorBox";
            this.XScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.XScaleFactorBox.TabIndex = 1;
            // 
            // ZScaleFactorBox
            // 
            this.ZScaleFactorBox.DecimalPlaces = 1;
            this.ZScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ZScaleFactorBox.Location = new System.Drawing.Point(166, 66);
            this.ZScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ZScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.ZScaleFactorBox.Name = "ZScaleFactorBox";
            this.ZScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.ZScaleFactorBox.TabIndex = 1;
            // 
            // YRotScaleFactorBox
            // 
            this.YRotScaleFactorBox.DecimalPlaces = 1;
            this.YRotScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.YRotScaleFactorBox.Location = new System.Drawing.Point(166, 40);
            this.YRotScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.YRotScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.YRotScaleFactorBox.Name = "YRotScaleFactorBox";
            this.YRotScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.YRotScaleFactorBox.TabIndex = 1;
            // 
            // ZRotScaleFactorBox
            // 
            this.ZRotScaleFactorBox.DecimalPlaces = 1;
            this.ZRotScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ZRotScaleFactorBox.Location = new System.Drawing.Point(166, 66);
            this.ZRotScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ZRotScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.ZRotScaleFactorBox.Name = "ZRotScaleFactorBox";
            this.ZRotScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.ZRotScaleFactorBox.TabIndex = 1;
            // 
            // XRotScaleFactorBox
            // 
            this.XRotScaleFactorBox.DecimalPlaces = 1;
            this.XRotScaleFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.XRotScaleFactorBox.Location = new System.Drawing.Point(166, 14);
            this.XRotScaleFactorBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.XRotScaleFactorBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.XRotScaleFactorBox.Name = "XRotScaleFactorBox";
            this.XRotScaleFactorBox.Size = new System.Drawing.Size(120, 20);
            this.XRotScaleFactorBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Y Skalierungsfaktor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "X Skalierungsfaktor";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Z Skalierungsfaktor";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TranslationThreshold);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.TranslationFunctionCombo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.YScaleFactorBox);
            this.groupBox1.Controls.Add(this.ZScaleFactorBox);
            this.groupBox1.Controls.Add(this.XScaleFactorBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 119);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Translation";
            // 
            // TranslationThreshold
            // 
            this.TranslationThreshold.DecimalPlaces = 1;
            this.TranslationThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TranslationThreshold.Location = new System.Drawing.Point(166, 92);
            this.TranslationThreshold.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.TranslationThreshold.Name = "TranslationThreshold";
            this.TranslationThreshold.Size = new System.Drawing.Size(120, 20);
            this.TranslationThreshold.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Schwellwert";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(292, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Beschleunigungsfunktion";
            // 
            // TranslationFunctionCombo
            // 
            this.TranslationFunctionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TranslationFunctionCombo.FormattingEnabled = true;
            this.TranslationFunctionCombo.Items.AddRange(new object[] {
            "Keine Beschleunigung",
            "Sinus-Beschleunigung",
            "Quadratische Beschleunigung",
            "Kubische Beschleunigung"});
            this.TranslationFunctionCombo.Location = new System.Drawing.Point(292, 42);
            this.TranslationFunctionCombo.Name = "TranslationFunctionCombo";
            this.TranslationFunctionCombo.Size = new System.Drawing.Size(121, 21);
            this.TranslationFunctionCombo.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.RotationThreshold);
            this.groupBox2.Controls.Add(this.RotationFunctionCombo);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.YRotScaleFactorBox);
            this.groupBox2.Controls.Add(this.ZRotScaleFactorBox);
            this.groupBox2.Controls.Add(this.XRotScaleFactorBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(424, 118);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rotation";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 92);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Schwellwert";
            // 
            // RotationThreshold
            // 
            this.RotationThreshold.DecimalPlaces = 1;
            this.RotationThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.RotationThreshold.Location = new System.Drawing.Point(166, 92);
            this.RotationThreshold.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.RotationThreshold.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.RotationThreshold.Name = "RotationThreshold";
            this.RotationThreshold.Size = new System.Drawing.Size(120, 20);
            this.RotationThreshold.TabIndex = 8;
            // 
            // RotationFunctionCombo
            // 
            this.RotationFunctionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RotationFunctionCombo.FormattingEnabled = true;
            this.RotationFunctionCombo.Items.AddRange(new object[] {
            "Keine Beschleunigung",
            "Sinus-Beschleunigung",
            "Quadratische Beschleunigung",
            "Kubische Beschleunigung"});
            this.RotationFunctionCombo.Location = new System.Drawing.Point(292, 42);
            this.RotationFunctionCombo.Name = "RotationFunctionCombo";
            this.RotationFunctionCombo.Size = new System.Drawing.Size(121, 21);
            this.RotationFunctionCombo.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(292, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Beschleunigungsfunktion";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "X Skalierungsfaktor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Z Skalierungsfaktor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Y Skalierungsfaktor";
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(12, 309);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "Ok";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CButton
            // 
            this.CButton.Location = new System.Drawing.Point(341, 309);
            this.CButton.Name = "CButton";
            this.CButton.Size = new System.Drawing.Size(75, 23);
            this.CButton.TabIndex = 10;
            this.CButton.Text = "Abbrechen";
            this.CButton.UseVisualStyleBackColor = true;
            this.CButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ApplicationCombo
            // 
            this.ApplicationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ApplicationCombo.FormattingEnabled = true;
            this.ApplicationCombo.Location = new System.Drawing.Point(178, 6);
            this.ApplicationCombo.Name = "ApplicationCombo";
            this.ApplicationCombo.Size = new System.Drawing.Size(120, 21);
            this.ApplicationCombo.TabIndex = 11;
            this.ApplicationCombo.SelectedIndexChanged += new System.EventHandler(this.ApplicationCombo_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Anwendungsprofil";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(169, 309);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(96, 23);
            this.ResetButton.TabIndex = 13;
            this.ResetButton.Text = "Zurücksetzen";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(18, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "angeschlossenes Gerät";
            // 
            // DeviceBox
            // 
            this.DeviceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DeviceBox.Enabled = false;
            this.DeviceBox.FormattingEnabled = true;
            this.DeviceBox.Location = new System.Drawing.Point(178, 33);
            this.DeviceBox.Name = "DeviceBox";
            this.DeviceBox.Size = new System.Drawing.Size(120, 21);
            this.DeviceBox.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 345);
            this.Controls.Add(this.DeviceBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ApplicationCombo);
            this.Controls.Add(this.CButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "AerionInputConfig";
            ((System.ComponentModel.ISupportInitialize)(this.YScaleFactorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XScaleFactorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZScaleFactorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YRotScaleFactorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZRotScaleFactorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XRotScaleFactorBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TranslationThreshold)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown YScaleFactorBox;
        private System.Windows.Forms.NumericUpDown XScaleFactorBox;
        private System.Windows.Forms.NumericUpDown ZScaleFactorBox;
        private System.Windows.Forms.NumericUpDown YRotScaleFactorBox;
        private System.Windows.Forms.NumericUpDown ZRotScaleFactorBox;
        private System.Windows.Forms.NumericUpDown XRotScaleFactorBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CButton;
        private System.Windows.Forms.ComboBox ApplicationCombo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox TranslationFunctionCombo;
        private System.Windows.Forms.ComboBox RotationFunctionCombo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.NumericUpDown TranslationThreshold;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown RotationThreshold;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox DeviceBox;
    }
}

