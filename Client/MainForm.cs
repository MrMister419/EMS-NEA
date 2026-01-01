using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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

        private void groupBox1_Enter(object sender, EventArgs e)
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
            RadioButton button = (RadioButton)buttonObject;
            if (button.Checked)
            {
                switchPanelFrom(button);
            }
        }

        private void switchPanelFrom(RadioButton button)
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
            
        }
    }
}
