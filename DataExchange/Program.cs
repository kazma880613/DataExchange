// See https://aka.ms/new-console-template for more information
using DataExchange;
using System.Security.Cryptography.X509Certificates;

Thread newThread = new Thread(DoWork);

newThread.Start();

newThread.Join();

SendToSalesForce _SendToSalesForce = new SendToSalesForce();

await _SendToSalesForce.SendData();


static async void DoWork()

{
    InfoLoad _infoLoad = new InfoLoad();

    dbService _dbService = new dbService();

    response _response = new response();

    Console.WriteLine("DataExchange Start...");

    Thread.Sleep(1000);

    Console.WriteLine("Checking Connection to SQL ...");

    _infoLoad.SQLInfoLoad();

    Thread.Sleep(1000);

    _dbService.chooseSQL_Open();

    Thread.Sleep(1000);

    Console.WriteLine("Loading SQL QueryString ...");

    _infoLoad.scheduleInfo();

    Thread.Sleep(1000);

    Console.WriteLine("Data Collecting ...");

    _dbService.Collecting_data(InfoLoad._request.QueryString);

    Thread.Sleep(2000);

    Console.WriteLine("Closing SQL Connection ...");

    _dbService.chooseSQL_Close();

    //_dbService.ODBC_SQL_Close();

    Thread.Sleep(1000);

    Console.WriteLine("Delivering Data to SalesForce ...");

    _infoLoad.getURL();

}