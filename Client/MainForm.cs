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
    private readonly List<EventData> receivedEvents = new List<EventData>();

    public MainForm()
    {
        InitializeComponent();
        feedPanel.BringToFront();
        toggleAlertsCheckbox.Checked = AppContext.isReceiving;
        AppContext.appService.RegisterEventHandler(DisplayIncomingEvent);
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
    private void toggleAlertsCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        // TODO: disable for a few seconds
        bool newChoice = toggleAlertsCheckbox.Checked;
        AppContext.appService.ToggleAlertChoice(newChoice);
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

    private void label30_Click(object sender, EventArgs e)
    {

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
    public void DisplayIncomingEvent(Dictionary<string, string> eventData)
    {
        EventData evt = MapEvent(eventData);
        receivedEvents.Add(evt);

        Panel tile = new Panel();
        tile.Width = flowLayoutPanel1.ClientSize.Width - 10;
        tile.Height = 60;
        tile.BackColor = Color.WhiteSmoke;
        tile.BorderStyle = BorderStyle.FixedSingle;
        tile.Margin = new Padding(3);
        tile.Tag = evt;

        Label title = new Label();
        title.AutoSize = false;
        title.Dock = DockStyle.Fill;
        title.TextAlign = ContentAlignment.MiddleLeft;
        title.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        title.Text = evt.Type;
        tile.Controls.Add(title);

        tile.Cursor = Cursors.Hand;
        tile.Click += EventTileClick;
        title.Click += EventTileClick;

        flowLayoutPanel1.Controls.Add(tile);
    }

    // Shows event details when tile clicked
    private void EventTileClick(object sender, EventArgs e)
    {
        EventData evt = null;
        if (sender is Control control && control.Tag is EventData data)
        {
            evt = data;
        }
        else if (sender is Control control2 && control2.Parent != null && control2.Parent.Tag is EventData parentData)
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
    private void ShowEventDetails(EventData evt)
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

    // Maps dictionary event data to EventData object
    // Parameters:
    // - Dictionary<string, string> eventData: raw event data from server
    // Returns:
    // EventData: mapped event object
    private EventData MapEvent(Dictionary<string, string> eventData)
    {
        EventData evt = new EventData();
        if (eventData == null)
        {
            return evt;
        }

        if (eventData.TryGetValue("type", out string type)) evt.Type = type;
        if (eventData.TryGetValue("description", out string desc)) evt.Description = desc;
        if (eventData.TryGetValue("status", out string status)) evt.Status = status;
        if (eventData.TryGetValue("startTimestamp", out string start)) evt.StartTimestamp = start;
        if (eventData.TryGetValue("resolvedTimestamp", out string resolved)) evt.ResolvedTimestamp = resolved;
        if (eventData.TryGetValue("location.address", out string address)) evt.Address = address;

        double latParsed;
        double lonParsed;
        if (eventData.TryGetValue("location.latitude", out string latString))
        {
            if (double.TryParse(latString, NumberStyles.Any, CultureInfo.InvariantCulture, out latParsed))
            {
                evt.LocationLatitude = latParsed;
            }
        }
        if (eventData.TryGetValue("location.longitude", out string lonString))
        {
            if (double.TryParse(lonString, NumberStyles.Any, CultureInfo.InvariantCulture, out lonParsed))
            {
                evt.LocationLongitude = lonParsed;
            }
        }
        return evt;
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
/// Represents event data
/// </summary>
class EventData
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