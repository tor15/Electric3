using System.Net.Http;
using System.Text.Json;
using System.Text;
using JsonElement = System.Text.Json.JsonElement;
//https://www.vector4free.com/free-vectors/electric

namespace Electric3;




public partial class MainPage : ContentPage
{
    int count = 0;
    bool error = false;
    HttpClient client = new HttpClient();
    //private void displayNewData();

    public MainPage()
    {
        InitializeComponent();
        //ErrorLabel.Text = "press power symbol to start";
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        if (count == 2)
        {
            count = 0;
            //CounterBtn.Text = "Click to start";
            return;
        }

        //CounterBtn.Text = "started";
        ErrorLabel.Text= "started";
        Task<int> longRunningTask = loopDisplay();
        int result = await longRunningTask;


    }

    private async Task<int> loopDisplay()
    {
        int i = 0;
        while (count == 1)
        {
            i++;
            displayNewData();

            /*
             * comment out the first line and uncomment second
             * when deploying....
             * 
             */
            //CounterBtn.Text = $"Click to stop {i}";
            if (!error)
                //CounterBtn.Text = "Click to stop";
                ErrorLabel.Text = "started";
            else
                count = 0;

            //keepDisplaying();
            await Task.Delay(5000);
        }
        return 1;
    }

    private void displayNewData()
    {

        error = false;
        string result;
        try
        {
            result = client.GetStringAsync("http://192.168.2.106/production.json").Result;
        }
        catch (Exception ex)
        {
            //CounterBtn.Text = "json file not opened";
            ErrorLabel.Text = "json file not opened";
            error = true;
            return;
        }

        try
        {
            using JsonDocument jsonDoc = JsonDocument.Parse(result);

            // get production values - there are two values in the array
            // I think we want the second one in production[1]
            var production = jsonDoc.RootElement.GetProperty("production");
            var production_wNow = production[1].GetProperty("wNow");
            //CounterBtn.Text = production_wNow.ToString();
            ProductionLabelText.Text = production_wNow.ToString();

            // get consumption values
            var consumption = jsonDoc.RootElement.GetProperty("consumption");
            var consumption_wNow = consumption[0].GetProperty("wNow");
            //CounterBtn.Text = consumption_wNow.ToString();
            ConsumptionLabelText.Text = consumption_wNow.ToString();
        }
        catch (Exception ex)
        {
            //CounterBtn.Text = "json not parsed";
            ErrorLabel.Text = "json not parsed";
            error = true;
        }
    

    // is this needed?  is it needed for the label text updates?
    //SemanticScreenReader.Announce(CounterBtn.Text);
    }

    // does it need to be Task instead of void?
    private async void keepDisplaying()
    {
        //CounterBtn.IsEnabled = false;

        await Task.Delay(5000);
        //CounterBtn.IsEnabled = true;
    }

    private void OnImageButtonClicked(object sender, EventArgs e)
    {
        if (count == 0) ErrorLabel.Text = "started";
        SemanticScreenReader.Announce(ErrorLabel.Text);
        fromImageButtonClicked();
        
    }

    private async void fromImageButtonClicked()
    {
        // same code as OnButtonClicked
        count++;
        
        if (count == 2)
        {
            count = 0;
            //CounterBtn.Text = "Click to start";
            ErrorLabel.Text = "click electric symbol to start";
            return;
        }

        //CounterBtn.Text = "started";
        Task<int> longRunningTask = loopDisplay();
        int result = await longRunningTask;
    }
}
