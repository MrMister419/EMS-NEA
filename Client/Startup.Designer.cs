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
        maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
        label4 = new System.Windows.Forms.Label();
        textBox3 = new System.Windows.Forms.TextBox();
        label5 = new System.Windows.Forms.Label();
        textBox4 = new System.Windows.Forms.TextBox();
        label6 = new System.Windows.Forms.Label();
        label3 = new System.Windows.Forms.Label();
        textBox2 = new System.Windows.Forms.TextBox();
        label2 = new System.Windows.Forms.Label();
        textBox1 = new System.Windows.Forms.TextBox();
        helpProvider1 = new System.Windows.Forms.HelpProvider();
        signupPanel = new System.Windows.Forms.Panel();
        signupSubmitButton = new System.Windows.Forms.Button();
        loginPanel = new System.Windows.Forms.Panel();
        button4 = new System.Windows.Forms.Button();
        groupBox2 = new System.Windows.Forms.GroupBox();
        label8 = new System.Windows.Forms.Label();
        textBox7 = new System.Windows.Forms.TextBox();
        label9 = new System.Windows.Forms.Label();
        textBox8 = new System.Windows.Forms.TextBox();
        label12 = new System.Windows.Forms.Label();
        signupTabButton = new System.Windows.Forms.RadioButton();
        loginTabButton = new System.Windows.Forms.RadioButton();
        groupBox1.SuspendLayout();
        signupPanel.SuspendLayout();
        loginPanel.SuspendLayout();
        groupBox2.SuspendLayout();
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
        groupBox1.Controls.Add(maskedTextBox1);
        groupBox1.Controls.Add(label4);
        groupBox1.Controls.Add(textBox3);
        groupBox1.Controls.Add(label5);
        groupBox1.Controls.Add(textBox4);
        groupBox1.Controls.Add(label6);
        groupBox1.Controls.Add(label3);
        groupBox1.Controls.Add(textBox2);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(textBox1);
        groupBox1.Location = new System.Drawing.Point(60, 78);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new System.Drawing.Size(545, 207);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        // 
        // maskedTextBox1
        // 
        maskedTextBox1.Location = new System.Drawing.Point(6, 161);
        maskedTextBox1.Name = "maskedTextBox1";
        maskedTextBox1.PasswordChar = '*';
        maskedTextBox1.Size = new System.Drawing.Size(228, 27);
        maskedTextBox1.TabIndex = 14;
        maskedTextBox1.UseSystemPasswordChar = true;
        // 
        // label4
        // 
        label4.Location = new System.Drawing.Point(277, 79);
        label4.Name = "label4";
        label4.Size = new System.Drawing.Size(133, 17);
        label4.TabIndex = 13;
        label4.Text = "Phone Number *";
        // 
        // textBox3
        // 
        textBox3.Location = new System.Drawing.Point(277, 102);
        textBox3.Name = "textBox3";
        textBox3.Size = new System.Drawing.Size(228, 27);
        textBox3.TabIndex = 12;
        // 
        // label5
        // 
        label5.Location = new System.Drawing.Point(6, 79);
        label5.Name = "label5";
        label5.Size = new System.Drawing.Size(133, 17);
        label5.TabIndex = 11;
        label5.Text = "Email *";
        // 
        // textBox4
        // 
        textBox4.Location = new System.Drawing.Point(6, 102);
        textBox4.Name = "textBox4";
        textBox4.Size = new System.Drawing.Size(228, 27);
        textBox4.TabIndex = 10;
        // 
        // label6
        // 
        label6.Location = new System.Drawing.Point(6, 139);
        label6.Name = "label6";
        label6.Size = new System.Drawing.Size(228, 17);
        label6.TabIndex = 9;
        label6.Text = "Password *";
        // 
        // label3
        // 
        label3.Location = new System.Drawing.Point(277, 21);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(133, 17);
        label3.TabIndex = 3;
        label3.Text = "Last Name *";
        // 
        // textBox2
        // 
        textBox2.Location = new System.Drawing.Point(277, 44);
        textBox2.Name = "textBox2";
        textBox2.Size = new System.Drawing.Size(228, 27);
        textBox2.TabIndex = 2;
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(6, 21);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(133, 17);
        label2.TabIndex = 1;
        label2.Text = "First Name *";
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(6, 44);
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
        loginPanel.Controls.Add(button4);
        loginPanel.Controls.Add(groupBox2);
        loginPanel.Controls.Add(label12);
        loginPanel.Location = new System.Drawing.Point(67, 113);
        loginPanel.Name = "loginPanel";
        loginPanel.Size = new System.Drawing.Size(659, 336);
        loginPanel.TabIndex = 5;
        // 
        // button4
        // 
        button4.Font = new System.Drawing.Font("Segoe UI", 12F);
        button4.Location = new System.Drawing.Point(246, 240);
        button4.Name = "button4";
        button4.Size = new System.Drawing.Size(168, 71);
        button4.TabIndex = 14;
        button4.Text = "Log In";
        button4.UseVisualStyleBackColor = true;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(label8);
        groupBox2.Controls.Add(textBox7);
        groupBox2.Controls.Add(label9);
        groupBox2.Controls.Add(textBox8);
        groupBox2.Location = new System.Drawing.Point(60, 78);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new System.Drawing.Size(545, 145);
        groupBox2.TabIndex = 1;
        groupBox2.TabStop = false;
        // 
        // label8
        // 
        label8.Location = new System.Drawing.Point(11, 21);
        label8.Name = "label8";
        label8.Size = new System.Drawing.Size(226, 17);
        label8.TabIndex = 11;
        label8.Text = "Email *";
        // 
        // textBox7
        // 
        textBox7.Location = new System.Drawing.Point(11, 44);
        textBox7.Name = "textBox7";
        textBox7.Size = new System.Drawing.Size(286, 27);
        textBox7.TabIndex = 10;
        // 
        // label9
        // 
        label9.Location = new System.Drawing.Point(11, 81);
        label9.Name = "label9";
        label9.Size = new System.Drawing.Size(321, 17);
        label9.TabIndex = 9;
        label9.Text = "Password *";
        // 
        // textBox8
        // 
        textBox8.Location = new System.Drawing.Point(11, 104);
        textBox8.Name = "textBox8";
        textBox8.Size = new System.Drawing.Size(286, 27);
        textBox8.TabIndex = 8;
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
        ClientSize = new System.Drawing.Size(777, 535);
        Controls.Add(signupPanel);
        Controls.Add(loginTabButton);
        Controls.Add(signupTabButton);
        Controls.Add(loginPanel);
        Name = "Startup";
        Text = "Signup";
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        signupPanel.ResumeLayout(false);
        loginPanel.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
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
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textBox7;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox textBox8;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.RadioButton signupTabButton;
    private System.Windows.Forms.RadioButton loginTabButton;
    private System.Windows.Forms.MaskedTextBox maskedTextBox1;
}