namespace Client;

public partial class Startup : Form
{
    public Startup()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Signup signup = new Signup();
        signup.Show();
    }

    private void button2_Click(object sender, EventArgs e)
    {
        throw new System.NotImplementedException();
    }
}