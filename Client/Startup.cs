using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client;

public partial class Startup : Form
{
    public Startup()
    {
        InitializeComponent();
        loginTabButton.Checked = true;
        loginTabButton.Enabled = false;
    }

    private void textBox4_TextChanged(object sender, System.EventArgs e)
    {

    }

    private void SignupTabButtonCheckedChanged(object sender, System.EventArgs e)
    {

    }

    private void LoginTabButtonClick(object sender, System.EventArgs e)
    {
        loginTabButton.Enabled = false;
        signupTabButton.Enabled = true;

        signupPanel.Enabled = false;
        signupPanel.Visible = false;

        loginPanel.Visible = true;
        loginPanel.Enabled = true;
    }

    private void SignupTabButtonClick(object sender, System.EventArgs e)
    {
        signupTabButton.Enabled = false;
        loginTabButton.Enabled = true;

        loginPanel.Enabled = false;
        loginPanel.Visible = false;

        signupPanel.Visible = true;
        signupPanel.Enabled = true;
    }

    private void LoginTabButtonCheckedChanged(object sender, System.EventArgs e)
    {

    }

    private void SignupSubmitButton_Click(object sender, System.EventArgs e)
    {
        Client.SignUp(GetFormValues());
    }
    
    private Dictionary<string, string> GetFormValues()
    {
        Dictionary<string, string> formValues = new Dictionary<string, string>();
        formValues.Add("First Name", textBox1.Text);
        formValues.Add("Last Name", textBox2.Text);
        formValues.Add("Phone Number", textBox3.Text);
        formValues.Add("Email", textBox4.Text);
        formValues.Add("Password", maskedTextBox1.Text);
        
        // TODO: Use SecureString instead of string
        maskedTextBox1.Text = null;

        return formValues;
    }
}