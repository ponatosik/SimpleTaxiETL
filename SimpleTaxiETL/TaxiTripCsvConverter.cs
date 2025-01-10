using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Data.Entities;
using Microsoft.Extensions.Logging;

namespace SimpleTaxiETL;

public class TaxiTripCsvConverter
{
    private readonly CsvConfiguration _configuration;
    private readonly ILogger? _logger;

    public TaxiTripCsvConverter(ILogger? logger = null, CsvConfiguration? configuration = null)
    {
        _logger = logger;
        _configuration = configuration ?? new CsvConfiguration(CultureInfo.InvariantCulture);
    }

    public IEnumerable<TaxiTrip> Read(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _configuration);
        csv.Context.RegisterClassMap<TaxiTripCsvMap>();


        while (csv.Read())
        {
            TaxiTrip record;
            try
            {
                record = csv.GetRecord<TaxiTrip>();
            }
            catch (TypeConverterException e)
            {
                _logger?.LogWarning($"Cannot parse record from csv file \"{filePath}\": \n {e.Message}");
                continue;
            }

            yield return record;
        }
    }


    public async Task WriteAsync(IEnumerable<TaxiTrip> trips, string filePath)
    {
        try
        {
            await using var writer = new StreamWriter(filePath);
            await using var csv = new CsvWriter(writer, _configuration);
            csv.Context.RegisterClassMap<TaxiTripCsvMap>();

            await csv.WriteRecordsAsync(trips);
        }
        catch (IOException e)
        {
            _logger?.LogError($"An IO exception occured while writing to file {filePath}: \n {e.Message}");
            throw;
        }
    }

    public async Task WriteAsync(IAsyncEnumerable<TaxiTrip> trips, string filePath)
    {
        try
        {
            await using var writer = new StreamWriter(filePath);
            await using var csv = new CsvWriter(writer, _configuration);
            csv.Context.RegisterClassMap<TaxiTripCsvMap>();

            await csv.WriteRecordsAsync(trips);
        }
        catch (IOException e)
        {
            _logger?.LogError($"An IO exception occured while writing to file {filePath}: \n {e.Message}");
            throw;
        }
    }
}

internal sealed class TaxiTripCsvMap : ClassMap<TaxiTrip>
{
    public TaxiTripCsvMap()
    {
        Map(m => m.PickupTime).Name("tpep_pickup_datetime");
        Map(m => m.DropOffTime).Name("tpep_dropoff_datetime");
        Map(m => m.PassengerCount).Name("passenger_count");
        Map(m => m.Distance).Name("trip_distance");
        Map(m => m.PickupLocationId).Name("PULocationID");
        Map(m => m.DropOffLocationId).Name("DOLocationID");
        Map(m => m.FareAmount).Name("fare_amount");
        Map(m => m.TipAmount).Name("tip_amount");

        Map(m => m.StoreAndForwardFlag)
            .Name("store_and_fwd_flag")
            .TypeConverterOption.BooleanValues(true, true, "Y")
            .TypeConverterOption.BooleanValues(false, true, "N");
    }
}