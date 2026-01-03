using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client;

static class AppContext
{
    public static FormNavigation formNavigator;
    public static FormManager formManager;
    public static AppService appService;
    public static string email { get; set; }
    
    public static void Initialize()
    {
        formNavigator = new FormNavigation();
        formManager = new FormManager();
        appService = new AppService();
    }
}

class FormManager
{
    public void SwitchForm(Form currentForm)
    {
        if (currentForm is Startup)
        {
            Form mainForm = new MainForm();
            mainForm.Show();
        }
        else
        {
            Form startupForm = new Startup();
            startupForm.Show();
        }

        currentForm.Hide();
    }
}

class FormNavigation
{ 
    
    public Dictionary<string, string> GetEnteredValues(Panel formPanel)
    {
        Dictionary<string, string> formValues = new Dictionary<string, string>();
        List<Panel> fieldPanels = GetControlsByType<Panel>(formPanel);

        string fieldName = "";
        string fieldEntry = "";
        
        foreach (Panel panel in fieldPanels)
        {
            foreach (Control childControl in panel.Controls)
            {
                switch (childControl)
                {
                    case Label:
                        fieldName = childControl.Text;
                        fieldName = fieldName.Replace("*", "").Replace(" ", "");
                        break;
                    case TextBox:
                    case MaskedTextBox:
                    {
                        fieldEntry = childControl.Text.Trim();

                        if (childControl is MaskedTextBox)
                        {
                            childControl.Text = "";
                            // TODO: Use SecureString instead of string
                        }

                        break;
                    }
                }
                
            }
            formValues.Add(fieldName, fieldEntry);
        }
        
        return formValues;
    }

    public List<type> GetControlsByType<type>(Control parentControl, bool getBaseControlsOnly = true) where type : Control
    {
        List<type> controls = new List<type>();
        
        foreach (Control control in parentControl.Controls)
        {
            List<type> subControls = GetControlsByType<type>(control, getBaseControlsOnly);
            if (control is type)
            {
                
                if (subControls.Count > 0 && !getBaseControlsOnly)
                {
                    controls.AddRange(subControls);
                }
                else
                {
                    controls.Add((type) control);
                }
            }
            else
            {
                controls.AddRange(subControls);
            }
        }
        
        return controls;
    }
}
