using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client;

public partial class Startup : Form
{
    
    public Startup()
    {
        InitializeComponent();
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
        signupPanel.Enabled = false;
        signupPanel.Visible = false;
        loginPanel.Visible = true;
        loginPanel.Enabled = true;
    }

    private void SignupTabButtonClick(object sender, System.EventArgs e)
    {
        loginPanel.Enabled = false;
        loginPanel.Visible = false;
        signupPanel.Visible = true;
        signupPanel.Enabled = true;
    }
    
    private void SignupSubmitButton_Click(object sender, System.EventArgs e)
    {
        Dictionary<string, string> formValues = 
            AppContext.formNavigator.GetEnteredValues(signupPanel);
        AppContext.appService.SignUp(formValues);
    }
    
    private async void loginSubmitButton_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(loginPanel);
        Dictionary<string, string> outcome = await AppContext.appService.Authenticate(formValues);
        
        // TODO: Use typed strings, move this to AppService
        if (outcome["successful"] == "true")
        {
            AppContext.email = formValues["Email"];
            AppContext.formManager.SwitchForm(this);
        }
        else
        {
            loginMessageLabel.Text = outcome["outcome"];
        }
    }
}