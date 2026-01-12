using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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

    // Initializes MainForm and sets up default UI
    public MainForm()
    {
        receivedEvents = new List<Event>();

        InitializeComponent();
        MainFormInitialize();
    }

    // Initializes starting main form UI and event handling
    private void MainFormInitialize()
    {
        // Setup default starting UI
        feedPanel.BringToFront();
        ChangeCheckBoxNoTrigger();

        // Register long-polling handler and request event
        AppContext.appService.RegisterEventHandler(DisplayEventInFeed);
        if (AppContext.isReceiving)
        {
            AppContext.appService.RequestEvent();
        }
    }

    // Switches main panel to live feed when selected
    private void liveFeedButton_CheckedChanged(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches main panel to settings when selected
    private void settingsButton_CheckedChanged(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches main panel to account when selected
    private void accountBbutton_CheckedChanged(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches main panel to information when selected
    private void informationButton_CheckedChanged(object sender, EventArgs e)
    {
        switchPanels(sender);
    }
    
    // Switches to change account details panel
    private void accountPanelButton_Click(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to change password panel
    private void ChangeToPasswordPanel(object sender, EventArgs e)
    {
        switchPanels(sender);
    }

    // Switches to delete account panel
    private void DeleteAccountPanel(object sender, EventArgs e)
    {
        switchPanels(sender);
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

    // Routes main panel switching based on button type
    // Parameters:
    // - object buttonObject: the button or radio button that was clicked
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
    // Parameters:
    // - ButtonBase button: the button whose tag matches the target panel
    private void switchPanelFrom(ButtonBase button)
    {
        string tag = button.Tag.ToString();

        // Get the list of panels in the main area
        List<Panel> panels = AppContext.formNavigator.GetControlsByType<Panel>(mainAreaPanel);

        foreach (Panel panel in panels)
        {
            if (panel.Tag.ToString() == tag)
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
        bool newChoice = toggleAlertsCheckbox.Checked;
        Dictionary<string, string>? outcome = await AppContext.appService.ToggleAlertChoice(newChoice);
        
        if (outcome == null || !outcome.ContainsKey("outcome") || !outcome.ContainsKey("success"))
        {
            changeStatusResultLabel.Text = "An error occurred. Try again later.";
            ChangeCheckBoxNoTrigger();
        }
        else if (outcome["success"] == "True")
        {
            changeStatusResultLabel.Text = outcome["outcome"];
            AppContext.isReceiving = newChoice;
        }
        else
        {
            changeStatusResultLabel.Text = outcome["outcome"];
            ChangeCheckBoxNoTrigger();
        }
        
        // Disable checkbox for a few seconds to prevent double-clicks / spamming
        toggleAlertsCheckbox.Enabled = false;
        await Task.Delay(2000);
        toggleAlertsCheckbox.Enabled = true;
    }

    // Retrieves and displays user account details
    private async void ViewAccountDetailsPanel(object sender, EventArgs e)
    {
        switchPanels(sender);
        Dictionary<string, string>? accountDetails = await AppContext.appService.GetAccountDetails();

        if (accountDetails == null || !accountDetails.ContainsKey("outcome") || !accountDetails.ContainsKey("success"))
        {
            viewAccountDataStatusLabel.Text = "An error occurred. Please try again later.";
        } else if (accountDetails["success"] == "False")
        {
            viewAccountDataStatusLabel.Text = accountDetails["outcome"];
        }
        else
        {
            displayAccountDetails(accountDetails);
        }
    }

    // Populates account details form fields with user data
    // Parameters:
    // - Dictionary<string, string> accountDetails: user account information
    private void displayAccountDetails(Dictionary<string, string> accountDetails)
    {
        if (!accountDetails.ContainsKey("Email"))
        {
            accountDetails.Add("Email", AppContext.email);
        }

        List<TextBox> textBoxes = AppContext.formNavigator.GetControlsByType<TextBox>(displayDataGroupBox);
        string fieldTag;

        foreach (TextBox textBox in textBoxes)
        {
            fieldTag = textBox.Tag.ToString();
            if (accountDetails.TryGetValue(fieldTag, out string fieldValue))
            {
                Console.Write(fieldTag);
                textBox.Text = fieldValue;
            }
            else
            {
                textBox.Text = "Failed to retrieve field.";
            }
        }
    }

    // Submits modified account details to server
    private async void ConfirmAccountChangesButtonClick(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(modifyAccountPanel);
        string newEmail = formValues["NewEmail"];
        Dictionary<string, string>? outcome = await AppContext.appService.ModifyAccountDetails(formValues);

        if (outcome == null || !outcome.ContainsKey("outcome") || !outcome.ContainsKey("success"))
        {
            modifyAccountResultLabel.Text = "An error occurred. Please try again later.";
            Console.WriteLine("Account modification failed: invalid server response.");
        }
        else if (outcome["success"] == "True")
        {
            AppContext.email = newEmail;
            modifyAccountResultLabel.Text = outcome["outcome"];
        }
        else
        {
            modifyAccountResultLabel.Text = outcome["outcome"];
        }
    }

    // Submits password change request to server
    private async void ChangePasswordButtonClick(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(changePasswordPanel);
        Dictionary<string, string>? outcome = await AppContext.appService.ChangePassword(formValues);
        
        if (outcome == null || !outcome.ContainsKey("outcome") || !outcome.ContainsKey("success"))
        {
            changePasswordOutcomeLabel.Text = "An error occurred. Please try again later.";
            Console.WriteLine("Password update failed: invalid server response.");
        }
        else if (outcome["success"] == "True")
        {
            changePasswordOutcomeLabel.Text = outcome["outcome"];
        }
        else
        {
            changePasswordOutcomeLabel.Text = outcome["outcome"];
        }
    }

    // Submits account deletion request to server
    private async void ConfirmDeleteButton_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> formValues = AppContext.formNavigator.GetEnteredValues(deleteAccountPanel);
        Dictionary<string, string>? outcome = await AppContext.appService.DeleteUser(formValues);
        
        if (outcome == null || !outcome.ContainsKey("outcome") || !outcome.ContainsKey("success"))
        {
            changePasswordOutcomeLabel.Text = "An error occurred. Please try again later.";
            Console.WriteLine("Account deletion failed: invalid server response.");
        }
        else if (outcome["success"] == "True")
        {
            AppContext.formManager.SwitchForm(this);
        }
        else
        {
            changePasswordOutcomeLabel.Text = outcome["outcome"];
        }
    }
    
    // Creates and displays event tile in feed panel when event received
    // Parameters:
    // - string eventData: event information from server in JSON format
    private void DisplayEventInFeed(string eventData)
    {
        // Create event object and add to list
        Event evt = Event.DeserializeEvent(eventData);
        receivedEvents.Add(evt);

        // Create button for event
        Button eventButton = new Button
        {
            Width = feedPanel.ClientSize.Width - 25,
            Height = 80,
            BackColor = Color.LightGray,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10),
            Tag = evt,
            // TODO: Display status as well
            Text = $"{evt.Type}"
        };
        eventButton.Click += EventButtonClick;

        // Add button to flow layout panel and display it at the bottom of the list
        int listIndex = receivedEvents.Count - 1;
        flowLayoutPanel.Invoke((MethodInvoker)(() =>
        {
            flowLayoutPanel.Controls.Add(eventButton);
            flowLayoutPanel.Controls.SetChildIndex(eventButton, listIndex);
        }));
    }
    
    // Shows event details when tile clicked
    private async void EventButtonClick(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Event evt = (Event)button.Tag;
        switchPanels(sender);
        await ShowEventDetails(evt);
    }

    // Displays event details panel with all event information
    // Parameters:
    // - Event evt: event data to display
    // Returns:
    // Task: asyncronous operation with no return value
    private async Task ShowEventDetails(Event evt)
    {
        FillEventDetailFields(evt);

        // Get user location and display route to event location
        Dictionary<string, string>? coordinates = await AppContext.appService.GetUserLocation();
        if (coordinates == null)
        {
            Console.WriteLine("Bad response to user location request.");
        }
        else
        {
            string userLat = coordinates["Latitude"];
            string userLong = coordinates["Longitude"];
            string eventLat = evt.location.latitude.ToString(CultureInfo.InvariantCulture);
            string eventLong = evt.location.longitude.ToString(CultureInfo.InvariantCulture);
            LoadRoute(userLat, userLong, eventLat, eventLong);
        }
    }

    // Populates event details panel fields with event data
    // Parameters:
    // - Event evt: event data to extract field values from
    private void FillEventDetailFields(Event evt)
    {
        List<Label> fieldLabels = AppContext.formNavigator.GetControlsByType<Label>(eventDetailsPanel);
        string fieldName = "";
        string fieldValue = "";

        foreach (Label label in fieldLabels)
        {
            fieldName = label.Tag.ToString();
            fieldValue = GetFieldValue(evt, fieldName);
            label.Text = $"{fieldName}: {fieldValue}";
        }
    }

    // Retrieves specific field value from event object
    // Parameters:
    // - Event evt: event data to extract value from
    // - string fieldName: name of the field to retrieve
    // Returns:
    // string: field value or empty string if not found
    private string GetFieldValue(Event evt, string fieldName)
    {
        switch (fieldName)
        {
            case "Type": return evt.Type;
            case "Description": return evt.Description;
            case "Latitude":
                {
                    double latitude = evt.location.latitude;
                    string latitudeString = latitude.ToString(CultureInfo.InvariantCulture);
                    return latitudeString;
                }
            case "Longitude":
                {
                    double longitude = evt.location.longitude;
                    string longitudeString = longitude.ToString(CultureInfo.InvariantCulture);
                    return longitudeString;
                }
            case "Address": return evt.location.address;
            case "Status": return evt.Status;
            case "Time started": return evt.StartTimestamp;
            case "Time resolved": return evt.ResolvedTimestamp;
            default: return "";
        }
    }

    // Displays route on map using Google Maps embed
    // Parameters:
    // - string lat1: origin latitude
    // - string long1: origin longitude
    // - string lat2: destination latitude
    // - string long2: destination longitude
    private async void LoadRoute(string lat1, string long1, string lat2, string long2)
    {
        try
        {
            string apiKey = "AIzaSyBR0vC11aRcwkXbXPfGr2utiILnF9EpLRY";
            string url = $"https://www.google.com/maps/embed/v1/directions?key={apiKey}&origin={lat1},{long1}&destination={lat2},{long2}&mode=driving";
            string html = FetchHTML(url);
            
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            webView.NavigateToString(html);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error displaying map embed: " + ex);
        }
    }

    // Loads HTML template and inserts map URL
    // Parameters:
    // - string url: Google Maps embed URL
    // Returns:
    // string: HTML with URL inserted
    private string FetchHTML(string url)
    {
        string html = File.ReadAllText("mapEmbed.html");
        html = html.Replace("{url}", url);
        return html;
    }
    
    // Updates alert choice checkbox without triggering event handler
    private void ChangeCheckBoxNoTrigger()
    {
        toggleAlertsCheckbox.CheckedChanged -= toggleAlertsCheckbox_CheckedChanged;
        toggleAlertsCheckbox.Checked = AppContext.isReceiving;
        toggleAlertsCheckbox.CheckedChanged += toggleAlertsCheckbox_CheckedChanged;
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
    public Location location { get; set; }
    
    protected static JsonSerializerOptions jsonConfig = 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    // Factory method to create an Event instance by deserializing JSON
    // Parameters:
    // - string json: JSON string containing Event data
    // Returns:
    // Event: deserialized Event object
    public static Event DeserializeEvent(string json)
    {
        return JsonSerializer.Deserialize<Event>(json, jsonConfig);
    }
}

/// <summary>
/// Represents a geographic location with address and coordinates
/// </summary>
class Location
{
    public string address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}