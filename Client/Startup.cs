using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client;

/// <summary>
/// Startup form handling user login and signup
/// </summary>
public partial class Startup : Form
{
    
    public Startup()
    {
        InitializeComponent();
    }

    // Switch to Login tab
    private void LoginTabButtonClick(object sender, System.EventArgs e)
    {
        signupPanel.Enabled = false;
        signupPanel.Visible = false;
        loginPanel.Visible = true;
        loginPanel.Enabled = true;
    }

    // Switch to Signup tab
    private void SignupTabButtonClick(object sender, System.EventArgs e)
    {
        loginPanel.Enabled = false;
        loginPanel.Visible = false;
        signupPanel.Visible = true;
        signupPanel.Enabled = true;
    }
    
    // Submits signup form data to server
    private async void SignupSubmitButton_Click(object sender, System.EventArgs e)
    {
        Dictionary<string, string> formValues = 
            AppContext.formNavigator.GetEnteredValues(signupPanel);
        await AppContext.appService.SignUp(formValues);
    }
    
    // Authenticates user and switches to main form on success
    private async void LoginSubmitButton_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(loginPanel);
        Dictionary<string, string> outcome = await AppContext.appService.Authenticate(formValues);
        
        if (outcome["successful"] == "true")
        {
            AppContext.email = formValues["Email"];
            await AppContext.appService.GetReceivingStatus();
            AppContext.formManager.SwitchForm(this);
        }
        else
        {
            loginMessageLabel.Text = outcome["outcome"];
        }
    }
}