using System.Data;
using Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class TaxiTripRepository
{
    private readonly string _connectionString;
    private readonly ILogger? _logger;

    public TaxiTripRepository(string connectionString, ILogger? logger = null)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task InsertRangeAsync(IEnumerable<TaxiTrip> trips, CancellationToken cancellationToken = default)
    {
        var table = PrepareCopyTable(trips);
        await ExecuteBulkCopyAsync(table, cancellationToken);
    }

    // Performs SQL server bulk copy for better performance
    private async Task ExecuteBulkCopyAsync(DataTable table, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);

        try
        {
            var bulkCopy = new SqlBulkCopy(connection);

            bulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
            bulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
            bulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
            bulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
            bulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
            bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
            bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
            bulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
            bulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");

            bulkCopy.DestinationTableName = "TaxiTrips";

            await connection.OpenAsync(cancellationToken);

            await bulkCopy.WriteToServerAsync(table, cancellationToken);
            await connection.CloseAsync();
        }
        catch (SqlException e)
        {
            _logger?.LogError($"Sql exception while writing taxi trip records: \n {e.Message}");
            throw;
        }
    }

    // Prepares local table to perform bulk copy
    private static DataTable PrepareCopyTable(IEnumerable<TaxiTrip> trips)
    {
        var table = new DataTable();

        table.Columns.Add(new DataColumn("tpep_pickup_datetime", typeof(DateTime)));
        table.Columns.Add(new DataColumn("tpep_dropoff_datetime", typeof(DateTime)));
        table.Columns.Add(new DataColumn("passenger_count", typeof(int)));
        table.Columns.Add(new DataColumn("trip_distance", typeof(decimal)));
        table.Columns.Add(new DataColumn("store_and_fwd_flag", typeof(bool)));
        table.Columns.Add(new DataColumn("PULocationID", typeof(int)));
        table.Columns.Add(new DataColumn("DOLocationID", typeof(int)));
        table.Columns.Add(new DataColumn("fare_amount", typeof(decimal)));
        table.Columns.Add(new DataColumn("tip_amount", typeof(decimal)));


        foreach (var trip in trips)
        {
            var row = table.NewRow();
            row["tpep_pickup_datetime"] = trip.PickupTime;
            row["tpep_dropoff_datetime"] = trip.DropOffTime;
            row["passenger_count"] = trip.PassengerCount;
            row["trip_distance"] = trip.Distance;
            row["store_and_fwd_flag"] = trip.StoreAndForwardFlag;
            row["PULocationID"] = trip.PickupLocationId;
            row["DOLocationID"] = trip.DropOffLocationId;
            row["fare_amount"] = trip.FareAmount;
            row["tip_amount"] = trip.TipAmount;
            table.Rows.Add(row);
        }

        return table;
    }
}