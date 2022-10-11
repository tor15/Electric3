//
// cross platform app to show solar production and electricity consumption
//
// app reads production.json file on the local network - http://192.168.2.106/production.json
//
// file is not secure so xml file is edited for android - added - android:usesCleartextTraffic="true"
// otherwise android will not open the file
//
// app reads wNow - watts now from the production and consumption and displays on screen
//
// data is read every 5 seconds
//

using System.Net.Http;
using System.Text.Json;
using System.Text;
using JsonElement = System.Text.Json.JsonElement;


namespace Electric3;


public partial class MainPage : ContentPage
{
    int count = 0;                              // count = 1 when continuously looping to open the json file
                                                // count = 2 then the loop is broken and http call not made
    bool error = false;                         // if there is an error with opening/reading json file set to true
    HttpClient client = new HttpClient();       // single instance used for each http call to read json file
    

    public MainPage()
    {
        InitializeComponent();                  // start the app running - waiting for button clicks
        
    }


    // loop in this function 
    // reads the data from the production.json file every 5 seconds
    // and displays the data on the screen
    private async Task<int> loopDisplay()
    {
        // loop continuously here while reading and displaying data every 5 seconds
        // runs asynchronously so a button click to stop sets count != 0 breaks the loop
        while (count == 1)
        {
            displayNewData();       // get the updated numbers and display them

            if (!error)                             // if no error is given reading data
                ErrorLabel.Text = "started";        // display started on the screen
            else
                count = 0;                          // error so break the loop and return (could just return here)

            await Task.Delay(5000);                 // wait 5 seconds before repeating the loop
        }
        return 1;
    }

    // get the updated numbers for production and consumption and display them
    private void displayNewData()
    {
        error = false;          // there is no error 
        string result;          // to hold the json data

        // read the json file into result - if error, catch it
        try
        {
            result = client.GetStringAsync("http://192.168.2.106/production.json").Result;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "json file not opened";       // display the error on the bottom of the screen
            error = true;                                   // set that there is an error
            return;                                         
        }

        // json file has been opened successfully
        // now extract the readings from the file
        try
        {
            // parse the string of the json information into a jsondocument
            using JsonDocument jsonDoc = JsonDocument.Parse(result);

            // get production values - there are two values in the array
            // I think we want the second one in production[1]
            var production = jsonDoc.RootElement.GetProperty("production");
            var production_wNow = production[1].GetProperty("wNow");
            
            // display the production numbers on the screen
            ProductionLabelText.Text = production_wNow.ToString();

            // get consumption values
            var consumption = jsonDoc.RootElement.GetProperty("consumption");
            var consumption_wNow = consumption[0].GetProperty("wNow");
            
            // display the consumption numbers on the screen
            ConsumptionLabelText.Text = consumption_wNow.ToString();
        }
        // if error reading the data from the json document
        catch (Exception ex)
        {
            ErrorLabel.Text = "json not parsed";        // display error message on bottom of screen
            error = true;                               // set error to true
        }
    
    }

    
    // image button is the image in the centre of the screen
    // click on the image button to start and stop the reading of the data
    private void OnImageButtonClicked(object sender, EventArgs e)
    {
        // display started on bottom of screen if started reading data from json file
        // count is 0 if it is just starting to read the data
        if (count == 0) ErrorLabel.Text = "started";        
        
        // start or stop displaying the data
        fromImageButtonClicked();
        
    }

    // button has been clicked to start or stop reading and displaying the data

    private async void fromImageButtonClicked()
    {
        // count is 0 if starting, 1 if stopping
        count++;

        // count is now 1 if starting and 2 if stopping
        if (count == 2)
        {
            count = 0;          // reset count to 0 - ready to start
            ErrorLabel.Text = "click electric symbol to start";     // display how to start the app reading data
            return;
        }

        // enter the loop that reads and displays the data every 5 seconds
        
        Task<int> longRunningTask = loopDisplay();      // create a task
        int result = await longRunningTask;             // run the task asynchronously and await
    }
}
