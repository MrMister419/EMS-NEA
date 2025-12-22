using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client;

public partial class Startup : Form
{
    
    private FormNavigation formNavigator;
    
    public Startup()
    {
        InitializeComponent();
        loginTabButton.Checked = true;
        loginTabButton.Enabled = false;
        formNavigator = new FormNavigation(this);
    }

    private void textBox4_TextChanged(object sender, System.EventArgs e)
    {

    }

    private void SignupTabButtonCheckedChanged(object sender, System.EventArgs e)
    {

    }

    private void LoginTabButtonCheckedChanged(object sender, System.EventArgs e)
    {

    }

    private void LoginTabButtonClick(object sender, System.EventArgs e)
    {
        // TODO: optimise tab changing code
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

    private void SignupSubmitButton_Click(object sender, System.EventArgs e)
    {
        Client.SignUp(formNavigator.GetEnteredValues(signupPanel));
    }
    
    private void loginSubmitButton_Click(object sender, EventArgs e)
    {
        
    }
}