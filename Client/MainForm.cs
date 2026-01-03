using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void DisplayDataGroupBoxEnter(object sender, EventArgs e)
        {

        }

        private void liveFeedButton_CheckedChanged(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void settingsButton_CheckedChanged(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void accountBbutton_CheckedChanged(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void informationButton_CheckedChanged(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void switchPanels(object buttonObject)
        {
            if (buttonObject is RadioButton radioButton)
            {
                if (radioButton.Checked)
                {
                    switchPanelFrom(radioButton);
                }
            }
            else if (buttonObject is Button button)
            {
                switchPanelFrom(button);
            }
        }

        private void switchPanelFrom(ButtonBase button)
        {
            string tag = button.Tag.ToString();

            // Get the list of panels in the main area
            List<Panel> panels = AppContext.formNavigator.GetControlsByType<Panel>(mainAreaPanel);

            foreach (Panel panel in panels)
            {
                if (panel.Tag?.ToString() == tag)
                {
                    panel.Visible = true;
                    panel.Enabled = true;
                }
                else
                {
                    panel.Visible = false;
                    panel.Enabled = false;
                }
            }
        }

        private void signOutButton_Click(object sender, EventArgs e)
        {
            // TODO: Sign out cleanup

            AppContext.formManager.SwitchForm(this);
        }

        private void toggleAlertsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: disable for a few seconds
            bool newChoice = toggleAlertsCheckbox.Checked;
            AppContext.appService.ToggleAlertChoice(newChoice);
        }

        private async void viewAccountButton_Click(object sender, EventArgs e)
        {
            switchPanels(sender);
            Dictionary<string, string> accountDetails = await AppContext.appService.GetAccountDetails();
            foreach (KeyValuePair<string, string> entry in accountDetails)
            {
                Console.WriteLine( $"{entry.Key}: {entry.Value}");
            }
            
            displayAccountDetails(accountDetails);
        }

        private void displayAccountDetails(Dictionary<string, string> accountDetails)
        {
            accountDetails.Add("Email", AppContext.email);

            List<Panel> fieldPanels = AppContext.formNavigator.GetControlsByType<Panel>(displayDataGroupBox);
            Label fieldLabel = null;
            TextBox fieldTextBox = null;
            string fieldValue;
            
            foreach (Panel fieldPanel in fieldPanels)
            {
                foreach (Control childControl in fieldPanel.Controls)
                {
                    if (childControl is Label label)
                    {
                        fieldLabel = label;
                    }
                    else if (childControl is TextBox textBox)
                    {
                        fieldTextBox = textBox;
                    }
                }
                
                // TODO: Use tag instead of string comparison
                fieldValue = accountDetails[fieldLabel.Text];
                fieldTextBox.Text = fieldValue;
            }
        }

        private void changeAccountButton_Click(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void changePasswordButton_Click(object sender, EventArgs e)
        {
            switchPanels(sender);
        }

        private void deleteAccountButton_Click(object sender, EventArgs e)
        {
            switchPanels(sender);
        }
    }
}
