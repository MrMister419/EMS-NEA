using System.ComponentModel;

namespace Client;

partial class Signup
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
        helpProvider1 = new System.Windows.Forms.HelpProvider();
        textBox1 = new System.Windows.Forms.TextBox();
        groupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Font = new System.Drawing.Font("Segoe UI", 22F);
        label1.Location = new System.Drawing.Point(326, 32);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(191, 74);
        label1.TabIndex = 0;
        label1.Text = "Sign Up";
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(textBox1);
        groupBox1.Location = new System.Drawing.Point(111, 154);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new System.Drawing.Size(545, 55);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        // 
        // textBox1
        // 
        textBox1.Location = new System.Drawing.Point(296, 22);
        textBox1.Name = "textBox1";
        textBox1.Size = new System.Drawing.Size(243, 27);
        textBox1.TabIndex = 0;
        // 
        // Signup
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(groupBox1);
        Controls.Add(label1);
        Text = "Signup";
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.HelpProvider helpProvider1;
    private System.Windows.Forms.TextBox textBox1;

    private System.Windows.Forms.GroupBox groupBox1;

    private System.Windows.Forms.Label label1;

    #endregion
}