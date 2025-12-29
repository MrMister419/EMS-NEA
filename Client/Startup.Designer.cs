using System.ComponentModel;

namespace Client;

partial class Startup
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        label1 = new System.Windows.Forms.Label();
        groupBox1 = new System.Windows.Forms.GroupBox();
        panel7 = new System.Windows.Forms.Panel();
        maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
        label6 = new System.Windows.Forms.Label();
        panel6 = new System.Windows.Forms.Panel();
        label4 = new System.Windows.Forms.Label();
        textBox3 = new System.Windows.Forms.TextBox();
        panel5 = new System.Windows.Forms.Panel();
        label5 = new System.Windows.Forms.Label();
        textBox4 = new System.Windows.Forms.TextBox();
        panel4 = new System.Windows.Forms.Panel();
        label3 = new System.Windows.Forms.Label();
        textBox2 = new System.Windows.Forms.TextBox();
        panel3 = new System.Windows.Forms.Panel();
        label2 = new System.Windows.Forms.Label();
        textBox1 = new System.Windows.Forms.TextBox();
        helpProvider1 = new System.Windows.Forms.HelpProvider();
        signupPanel = new System.Windows.Forms.Panel();
        signupSubmitButton = new System.Windows.Forms.Button();
        loginPanel = new System.Windows.Forms.Panel();
        loginMessageLabel = new System.Windows.Forms.Label();
        loginSubmitButton = new System.Windows.Forms.Button();
        groupBox2 = new System.Windows.Forms.GroupBox();
        panel2 = new System.Windows.Forms.Panel();
        maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
        label9 = new System.Windows.Forms.Label();
        panel1 = new System.Windows.Forms.Panel();
        label8 = new System.Windows.Forms.Label();
        textBox7 = new System.Windows.Forms.TextBox();
        label12 = new System.Windows.Forms.Label();
        signupTabButton = new System.Windows.Forms.RadioButton();
        loginTabButton = new System.Windows.Forms.RadioButton();
        groupBox1.SuspendLayout();
        panel7.SuspendLayout();
        panel6.SuspendLayout();
        panel5.SuspendLayout();
        panel4.SuspendLayout();
        panel3.SuspendLayout();
        signupPanel.SuspendLayout();
        loginPanel.SuspendLayout();
        groupBox2.SuspendLayout();
        panel2.SuspendLayout();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("Segoe UI", 22F);
        label1.Location = new System.Drawing.Point(236, 20);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(191, 74);
        label1.TabIndex = 0;
        label1.Text = "Sign Up";
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(panel7);
        groupBox1.Controls.Add(panel6);
        groupBox1.Controls.Add(panel5);
        groupBox1.Controls.Add(panel4);
        groupBox1.Controls.Add(panel3);
        groupBox1.Location = new System.Drawing.Point(60, 78);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new System.Drawing.Size(545, 207);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        // 
        // panel7
        // 
        panel7.Controls.Add(maskedTextBox1);
        panel7.Controls.Add(label6);
        panel7.Location = new System.Drawing.Point(3, 130);
        panel7.Name = "panel7";
        panel7.Size = new System.Drawing.Size(255, 63);
        panel7.TabIndex = 19;
        // 
        // maskedTextBox1
        // 
        maskedTextBox1.Location = new System.Drawing.Point(3, 31);
        maskedTextBox1.Name = "maskedTextBox1";
        maskedTextBox1.PasswordChar = '*';
        maskedTextBox1.Size = new System.Drawing.Size(228, 27);
        maskedTextBox1.TabIndex = 14;
        maskedTextBox1.UseSystemPasswordChar = true;
        // 
        // label6
        // 
        label6.Location = new System.Drawing.Point(3, 9);
        label6.Name = "label6";
        label6.Size = new System.Drawing.Size(228, 17);
        label6.TabIndex = 9;
        label6.Text = "Password *";
        // 
        // panel6
        // 
        panel6.Controls.Add(label4);
        panel6.Controls.Add(textBox3);
        panel6.Location = new System.Drawing.Point(264, 71);
        panel6.Name = "panel6";
        panel6.Size = new System.Drawing.Size(263, 76);
        panel6.TabIndex = 18;
        // 
        // label4
        // 
        label4.Location = new System.Drawing.Point(3, 8);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(133, 17);
        label4.TabIndex = 13;
        label4.Text = "Phone Number *";
        // 
        // textBox3
        // 
        textBox3.Location = new System.Drawing.Point(3, 31);
        textBox3.Name = "textBox3";
        textBox3.Size = new System.Drawing.Size(228, 27);
        textBox3.TabIndex = 12;
        // 
        // panel5
        // 
        panel5.Controls.Add(label5);
        panel5.Controls.Add(textBox4);
        panel5.Location = new System.Drawing.Point(3, 68);
        panel5.Name = "panel5";
        panel5.Size = new System.Drawing.Size(255, 71);
        panel5.TabIndex = 17;
        // 
        // label5
        // 
        label5.Location = new System.Drawing.Point(3, 11);
        label5.Name = "label5";
        label5.Size = new System.Drawing.Size(133, 17);
        label5.TabIndex = 11;
        label5.Text = "Email *";
        // 
        // textBox4
        // 
        textBox4.Location = new System.Drawing.Point(3, 34);
        textBox4.Name = "textBox4";
        textBox4.Size = new System.Drawing.Size(228, 27);
        textBox4.TabIndex = 10;
        // 
        // panel4
        // 
        panel4.Controls.Add(label3);
        panel4.Controls.Add(textBox2);
        panel4.Location = new System.Drawing.Point(264, 23);
        panel4.Name = "panel4";
        panel4.Size = new System.Drawing.Size(263, 67);
        panel4.TabIndex = 16;
        // 
        // label3
        // 
        label3.Location = new System.Drawing.Point(5, -2);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(133, 17);
        label3.TabIndex = 3;
        label3.Text = "Last Name *";
        // 
        // textBox2
        // 
        textBox2.Location = new System.Drawing.Point(5, 21);
        textBox2.Name = "textBox2";
        textBox2.Size = new System.Drawing.Size(228, 27);
        textBox2.TabIndex = 2;
        // 
        // panel3
        // 
        panel3.Controls.Add(label2);
        panel3.Controls.Add(textBox1);
        panel3.Location = new System.Drawing.Point(3, 23);
        panel3.Name = "panel3";
        panel3.Size = new System.Drawing.Size(255, 65);
        panel3.TabIndex = 15;
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(3, -2);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(133, 17);
        label2.TabIndex = 1;
        label2.Text = "First Name *";
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(3, 21);
        textBox1.Name = "textBox1";
        textBox1.Size = new System.Drawing.Size(228, 27);
        textBox1.TabIndex = 0;
        // 
        // signupPanel
        // 
        signupPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        signupPanel.Controls.Add(signupSubmitButton);
        signupPanel.Controls.Add(groupBox1);
        signupPanel.Controls.Add(label1);
        signupPanel.Enabled = false;
        signupPanel.Location = new System.Drawing.Point(67, 116);
        signupPanel.Name = "signupPanel";
        signupPanel.Size = new System.Drawing.Size(659, 389);
        signupPanel.TabIndex = 4;
        signupPanel.Visible = false;
        // 
        // signupSubmitButton
        // 
        signupSubmitButton.Font = new System.Drawing.Font("Segoe UI", 12F);
        signupSubmitButton.Location = new System.Drawing.Point(246, 302);
        signupSubmitButton.Name = "signupSubmitButton";
        signupSubmitButton.Size = new System.Drawing.Size(168, 71);
        signupSubmitButton.TabIndex = 14;
        signupSubmitButton.Text = "Sign Up";
        signupSubmitButton.UseVisualStyleBackColor = true;
        signupSubmitButton.Click += SignupSubmitButton_Click;
        // 
        // loginPanel
        // 
        loginPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        loginPanel.Controls.Add(loginMessageLabel);
        loginPanel.Controls.Add(loginSubmitButton);
        loginPanel.Controls.Add(groupBox2);
        loginPanel.Controls.Add(label12);
        loginPanel.Location = new System.Drawing.Point(67, 113);
        loginPanel.Name = "loginPanel";
        loginPanel.Size = new System.Drawing.Size(659, 388);
        loginPanel.TabIndex = 5;
        // 
        // loginMessageLabel
        // 
        loginMessageLabel.AutoSize = true;
        loginMessageLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
        loginMessageLabel.Location = new System.Drawing.Point(66, 242);
        loginMessageLabel.Name = "loginMessageLabel";
        loginMessageLabel.Size = new System.Drawing.Size(0, 28);
        loginMessageLabel.TabIndex = 15;
        // 
        // loginSubmitButton
        // 
        loginSubmitButton.Font = new System.Drawing.Font("Segoe UI", 12F);
        loginSubmitButton.Location = new System.Drawing.Point(246, 288);
        loginSubmitButton.Name = "loginSubmitButton";
        loginSubmitButton.Size = new System.Drawing.Size(168, 71);
        loginSubmitButton.TabIndex = 14;
        loginSubmitButton.Text = "Log In";
        loginSubmitButton.UseVisualStyleBackColor = true;
        loginSubmitButton.Click += loginSubmitButton_Click;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(panel2);
        groupBox2.Controls.Add(panel1);
        groupBox2.Location = new System.Drawing.Point(60, 78);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new System.Drawing.Size(545, 145);
        groupBox2.TabIndex = 1;
        groupBox2.TabStop = false;
        // 
        // panel2
        // 
        panel2.Controls.Add(maskedTextBox2);
        panel2.Controls.Add(label9);
        panel2.Location = new System.Drawing.Point(6, 76);
        panel2.Name = "panel2";
        panel2.Size = new System.Drawing.Size(346, 63);
        panel2.TabIndex = 10;
        // 
        // maskedTextBox2
        // 
        maskedTextBox2.Location = new System.Drawing.Point(5, 28);
        maskedTextBox2.Name = "maskedTextBox2";
        maskedTextBox2.Size = new System.Drawing.Size(286, 27);
        maskedTextBox2.TabIndex = 10;
        maskedTextBox2.UseSystemPasswordChar = true;
        // 
        // label9
        // 
        label9.Location = new System.Drawing.Point(8, 5);
        label9.Name = "label9";
        label9.Size = new System.Drawing.Size(321, 17);
        label9.TabIndex = 9;
        label9.Text = "Password *";
        // 
        // panel1
        // 
        panel1.Controls.Add(label8);
        panel1.Controls.Add(textBox7);
        panel1.Location = new System.Drawing.Point(6, 23);
        panel1.Name = "panel1";
        panel1.Size = new System.Drawing.Size(346, 62);
        panel1.TabIndex = 0;
        // 
        // label8
        // 
        label8.Location = new System.Drawing.Point(5, -2);
        label8.Name = "label8";
        label8.Size = new System.Drawing.Size(226, 17);
        label8.TabIndex = 11;
        label8.Text = "Email *";
        // 
        // textBox7
        // 
        textBox7.Location = new System.Drawing.Point(5, 21);
        textBox7.Name = "textBox7";
        textBox7.Size = new System.Drawing.Size(286, 27);
        textBox7.TabIndex = 10;
        // 
        // label12
        // 
        label12.Font = new System.Drawing.Font("Segoe UI", 22F);
        label12.Location = new System.Drawing.Point(255, 20);
        label12.Name = "label12";
        label12.Size = new System.Drawing.Size(128, 58);
        label12.TabIndex = 0;
        label12.Text = "Log In";
        // 
        // signupTabButton
        // 
        signupTabButton.Appearance = System.Windows.Forms.Appearance.Button;
        signupTabButton.BackColor = System.Drawing.SystemColors.Control;
        signupTabButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        signupTabButton.Font = new System.Drawing.Font("Segoe UI", 12F);
        signupTabButton.Location = new System.Drawing.Point(405, 29);
        signupTabButton.Name = "signupTabButton";
        signupTabButton.Size = new System.Drawing.Size(133, 63);
        signupTabButton.TabIndex = 7;
        signupTabButton.TabStop = true;
        signupTabButton.Text = "Sign Up";
        signupTabButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        signupTabButton.UseVisualStyleBackColor = false;
        signupTabButton.CheckedChanged += SignupTabButtonCheckedChanged;
        signupTabButton.Click += SignupTabButtonClick;
        // 
        // loginTabButton
        // 
        loginTabButton.Appearance = System.Windows.Forms.Appearance.Button;
        loginTabButton.BackColor = System.Drawing.SystemColors.Control;
        loginTabButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        loginTabButton.Font = new System.Drawing.Font("Segoe UI", 12F);
        loginTabButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        loginTabButton.Location = new System.Drawing.Point(233, 29);
        loginTabButton.Name = "loginTabButton";
        loginTabButton.Size = new System.Drawing.Size(129, 63);
        loginTabButton.TabIndex = 8;
        loginTabButton.TabStop = true;
        loginTabButton.Text = "Log In";
        loginTabButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        loginTabButton.UseVisualStyleBackColor = false;
        loginTabButton.CheckedChanged += LoginTabButtonCheckedChanged;
        loginTabButton.Click += LoginTabButtonClick;
        // 
        // Startup
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(790, 535);
        Controls.Add(loginTabButton);
        Controls.Add(signupTabButton);
        Controls.Add(loginPanel);
        Controls.Add(signupPanel);
        Name = "Startup";
        Text = "Signup";
        groupBox1.ResumeLayout(false);
        panel7.ResumeLayout(false);
        panel7.PerformLayout();
        panel6.ResumeLayout(false);
        panel6.PerformLayout();
        panel5.ResumeLayout(false);
        panel5.PerformLayout();
        panel4.ResumeLayout(false);
        panel4.PerformLayout();
        panel3.ResumeLayout(false);
        panel3.PerformLayout();
        signupPanel.ResumeLayout(false);
        loginPanel.ResumeLayout(false);
        loginPanel.PerformLayout();
        groupBox2.ResumeLayout(false);
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.HelpProvider helpProvider1;
    private System.Windows.Forms.TextBox textBox1;

    private System.Windows.Forms.GroupBox groupBox1;

    private System.Windows.Forms.Label label1;

    #endregion
    private System.Windows.Forms.Panel signupPanel;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button signupSubmitButton;
    private System.Windows.Forms.Panel loginPanel;
    private System.Windows.Forms.Button loginSubmitButton;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textBox7;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.RadioButton signupTabButton;
    private System.Windows.Forms.RadioButton loginTabButton;
    private System.Windows.Forms.MaskedTextBox maskedTextBox1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel7;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label loginMessageLabel;
    private System.Windows.Forms.MaskedTextBox maskedTextBox2;
}