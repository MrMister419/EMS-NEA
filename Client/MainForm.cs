using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client;

/// <summary>
/// Main application form displaying event feed, account settings, and information
/// </summary>
public partial class MainForm : Form
{
    private readonly List<Event> receivedEvents;

    public MainForm()
    {
        receivedEvents = new List<Event>();
        
        InitializeComponent();
        MainFormInitialize();
    }

    private void MainFormInitialize()
    {
        // Setup default starting UI
        feedPanel.BringToFront();
        toggleAlertsCheckbox.Checked = AppContext.isReceiving;
        
        // Register long-polling handler and request event
        AppContext.appService.RegisterEventHandler(DisplayIncomingEvent);
        if (AppContext.isReceiving)
        {
            AppContext.appService.RequestEvent();
        }
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

    // Routes main panel switching based on button type
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

    // Switches to panel matching button's tag
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

    // Signs out user and returns to startup form
    private void signOutButton_Click(object sender, EventArgs e)
    {
        AppContext.formManager.SwitchForm(this);
    }

    // Updates alert preference when checkbox toggled
    private async void toggleAlertsCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        // TODO: disable for a few seconds
        bool newChoice = toggleAlertsCheckbox.Checked;
        await AppContext.appService.ToggleAlertChoice(newChoice);
    }

    // Retrieves and displays user account details
    private async void viewAccountButton_Click(object sender, EventArgs e)
    {
        switchPanels(sender);
        Dictionary<string, string> accountDetails = await AppContext.appService.GetAccountDetails();
        foreach (KeyValuePair<string, string> entry in accountDetails)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }

        displayAccountDetails(accountDetails);
    }

    // Populates account details form fields with user data
    // Parameters:
    // - Dictionary<string, string> accountDetails: user account information
    private void displayAccountDetails(Dictionary<string, string> accountDetails)
    {
        accountDetails.Add("Email", AppContext.email);

        List<Panel> fieldPanels = AppContext.formNavigator.GetControlsByType<Panel>(displayDataGroupBox);
        TextBox fieldTextBox;
        string fieldTag;
        string fieldValue;

        foreach (Panel fieldPanel in fieldPanels)
        {
            fieldTextBox = null;
            foreach (Control childControl in fieldPanel.Controls)
            {
                if (childControl is TextBox textBox)
                {
                    fieldTextBox = textBox;
                }
            }

            if (fieldTextBox != null)
            {
                fieldTag = fieldTextBox.Tag.ToString();
                if (accountDetails.ContainsKey(fieldTag))
                {
                    fieldValue = accountDetails[fieldTag];
                    fieldTextBox.Text = fieldValue;
                }
            }
        }
    }

    // Switches to change account details panel
    private void changeAccountButton_Click(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to change password panel
    private void changePasswordButton_Click(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to delete account panel
    private void deleteAccountButton_Click(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Submits modified account details to server
    private async void ConfirmAccountChangesButtonClick(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(modifyAccountPanel);
        string newEmail = formValues["NewEmail"];
        Dictionary<string, string> outcome = await AppContext.appService.ModifyAccountDetails(formValues);
        Console.WriteLine(outcome["outcome"]);
        Console.WriteLine(JsonSerializer.Serialize(formValues));
        if (outcome["successful"] == "true")
        {
            AppContext.email = newEmail;
            modifyAccountResultLabel.Text = outcome["outcome"];
        }
        else
        {
            if (outcome["outcome"] == "Incorrect password.")
            {
                modifyAccountResultLabel.Text = outcome["outcome"];
            }
            else
            {
                modifyAccountResultLabel.Text = "An error occurred. Please try again later.";
            }
        }
    }

    // Switches to CPR help information panel
    private void CprHelpButtonClick(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to AED help information panel
    private void AedHelpButtonClick(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to legal information panel
    private void LegalPageButtonClick(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Loads user alert preference and requests events if enabled
    // Returns:
    // Task for the asyncronous operation with no return value
    private async Task LoadAlertChoice()
    {
        bool isReceiving = AppContext.isReceiving;
        toggleAlertsCheckbox.CheckedChanged -= toggleAlertsCheckbox_CheckedChanged;
        toggleAlertsCheckbox.Checked = isReceiving;
        toggleAlertsCheckbox.CheckedChanged += toggleAlertsCheckbox_CheckedChanged;

        if (isReceiving)
        {
            AppContext.appService.RequestEvent();
        }
    }

    // Creates and displays event tile in feed panel when event received
    // Parameters:
    // - Dictionary<string, string> eventData: event information from server
    private void DisplayIncomingEvent(Dictionary<string, string> eventData)
    {
        Event evt = JsonSerializer.Deserialize<Event>(JsonSerializer.Serialize(eventData));
        receivedEvents.Add(evt);
        
        eventPanel.BringToFront();
        eventPanel.Enabled = true;
        
        List<Label> fieldLabels = AppContext.formNavigator.GetControlsByType<Label>(eventDetailsPanel);
        string tag = "";
        foreach (Label label in fieldLabels)
        {
            tag = label.Tag.ToString();
            label.Text = $"{tag}: {eventData[tag]}";
        }
    }

    // Shows event details when tile clicked
    private void EventTileClick(object sender, EventArgs e)
    {
        Event evt = null;
        if (sender is Control control && control.Tag is Event data)
        {
            evt = data;
        }
        else if (sender is Control control2 && control2.Parent != null && control2.Parent.Tag is Event parentData)
        {
            evt = parentData;
        }
        if (evt == null)
        {
            return;
        }
        ShowEventDetails(evt);
    }

    // Displays event details panel with all event information
    // Parameters:
    // - EventData evt: event data to display
    private void ShowEventDetails(Event evt)
    {
        Label typeLabel = GetLabelByTag(eventPanel, "IncidentType");
        Label descriptionLabel = GetLabelByTag(eventPanel, "Description");
        Label addressLabel = GetLabelByTag(eventPanel, "Address");
        Label statusLabel = GetLabelByTag(eventPanel, "Status");
        Label startLabel = GetLabelByTag(eventPanel, "StartTime");
        Label resolvedLabel = GetLabelByTag(eventPanel, "ResolvedTime");

        if (typeLabel != null) typeLabel.Text = evt.Type;
        if (descriptionLabel != null) descriptionLabel.Text = evt.Description;
        if (addressLabel != null) addressLabel.Text = evt.Address;
        if (statusLabel != null) statusLabel.Text = evt.Status;
        if (startLabel != null) startLabel.Text = evt.StartTimestamp;
        if (resolvedLabel != null) resolvedLabel.Text = evt.ResolvedTimestamp;

        LoadRoute(evt.LocationLatitude, evt.LocationLongitude);

        eventPanel.Visible = true;
        eventPanel.Enabled = true;
        eventPanel.BringToFront();
    }

    // Recursively finds label control by Tag property
    // Parameters:
    // - Control parent: parent control to search within
    // - string tag: tag value to match
    // Returns:
    // Label: found label or null if not found
    private Label GetLabelByTag(Control parent, string tag)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is Label label && label.Tag != null && label.Tag.ToString() == tag)
            {
                return label;
            }
            Label child = GetLabelByTag(control, tag);
            if (child != null)
            {
                return child;
            }
        }
        return null;
    }

    // Loads Google Maps view of event location in WebView2
    // Parameters:
    // - double latitude: location latitude
    // - double longitude: location longitude
    private async void LoadRoute(double latitude, double longitude)
    {
        if (webView21 == null)
        {
            return;
        }
        try
        {
            string apiKey = "AIzaSyBR0vC11aRcwkXbXPfGr2utiILnF9EpLRY";
            string lat = latitude.ToString(CultureInfo.InvariantCulture);
            string lon = longitude.ToString(CultureInfo.InvariantCulture);
            string url = $"https://www.google.com/maps/embed/v1/directions?key={apiKey}&origin={lat},{lon}&destination={lat},{lon}&mode=walking";
            await webView21.EnsureCoreWebView2Async();
            webView21.Source = new Uri(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

/// <summary>
/// Represents a single received event
/// </summary>
class Event
{
    public string Type { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public string StartTimestamp { get; set; }
    public string ResolvedTimestamp { get; set; }
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
}