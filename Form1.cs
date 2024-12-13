using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace @lock
{
    public partial class Form1 : Form
    {
        
        public async Task<bool> CheckApiResponse()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync("http://localhost/api.php?id=user123");

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    var content = await response.Content.ReadAsStringAsync();

                    // Attempt to deserialize the content
                    try
                    {
                        var lockStatus = JsonConvert.DeserializeObject<LockStatus>(content);

                        // Show the lock status in a formatted message
                        string statusMessage = lockStatus.is_locked == "1" ? "The system is currently LOCKED." : "The system is currently UNLOCKED.";
                        MessageBox.Show($"Lock Status: {statusMessage}", "Lock Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Return true if locked, false if unlocked
                        return lockStatus.is_locked == "1"; // Assuming "1" means locked
                    }
                    catch (JsonException)
                    {
                        // Handle the case where the content is not valid JSON
                        MessageBox.Show("Received non-JSON response.");
                        return false; // Default to unlocked on error
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    // Handle HTTP request errors
                    MessageBox.Show($"HTTP error: {httpEx.Message}");
                    return false; // Default to unlocked on error
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    MessageBox.Show($"Error checking lock status: {ex.Message}");
                    return false; // Default to unlocked on error
                }
            }
        }
        public class LockStatus
        {
            public string is_locked { get; set; } // This property matches the API response
        }

        // Event handler for the form load event
        private async void MainForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("MainForm_Load is called."); // Debugging line
            bool isLocked = await CheckApiResponse();
            if (isLocked)
            {
                this.Show(); // Show the lock screen if the computer is locked
            }
            else
            {
                // If payment is done, inform the user and close the application
                MessageBox.Show("Payment has been cleared. The application will now close.");
                Application.Exit(); // This will close the application
            }
        }

        public Form1()
        {
            InitializeComponent();
            //MessageBox.Show("CheckLockStatus method is called.");
            this.Load += new System.EventHandler(this.MainForm_Load);
        }

        private void btnContactPS_Click(object sender, EventArgs e)
        {

        }


    }
}
