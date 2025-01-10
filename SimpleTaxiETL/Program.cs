using BusinessLogic;
using Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SimpleTaxiETL;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var inputFilePath = args.ParseParameter("--file") ?? CliHelper.Prompt<string>("csv file path");
var duplicatesFilePath = args.ParseParameter("--duplicates") ?? ".\\duplicates.csv";
var showLogs = args.ParseFlag("--logs");
var connectionString =
    args.ParseParameter("--connection-string") ?? CliHelper.Prompt<string>("database connection string");

using var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(showLogs ? LogLevel.Information : LogLevel.None);
});
var logger = loggerFactory.CreateLogger<Program>();


var repo = new TaxiTripRepository(connectionString, logger);
var service = new TaxiTripService(repo, logger);
var cvsConverter = new TaxiTripCsvConverter(logger);


try
{
    var records = cvsConverter.Read(inputFilePath);
    var duplicates = service.SaveTripsAsync(records);
    await cvsConverter.WriteAsync(duplicates, duplicatesFilePath);
    Console.WriteLine($"Transactions from file {inputFilePath} were successfully persisted");
}
catch (IOException e)
{
    Console.WriteLine("An IO exception occurred while reading/writing from file:");
    Console.WriteLine(e.Message);
}
catch (SqlException)
{
    Console.WriteLine("An sql exception occurred while interacting with a database");
    Console.WriteLine("Please run the program with logs enabled (--logs) to see details");
}
catch (Exception)
{
    Console.WriteLine("An unexpected error occurred, please run the program with logs enabled (--logs) to see details");
}