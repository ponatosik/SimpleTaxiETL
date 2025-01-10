using Data.Entities;
using Data.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class TaxiTripService
{
    private readonly TimeZoneInfo? _localTimezone;
    private readonly ILogger? _logger;
    private readonly TaxiTripRepository _repository;

    public TaxiTripService(TaxiTripRepository repository, TimeZoneInfo? localTimezone = null, ILogger? logger = null)
    {
        _repository = repository;
        _localTimezone = localTimezone;
        _logger = logger;
    }

    // Persists unique trips and return duplicates
    public async IAsyncEnumerable<TaxiTrip> SaveTripsAsync(IEnumerable<TaxiTrip> trips)
    {
        var uniqueKeys = new HashSet<(DateTime, DateTime, int)>();

        List<TaxiTrip> uniqueTrips = [];

        foreach (var trip in trips)
        {
            if (!uniqueKeys.Add((trip.PickupTime, trip.DropOffTime, trip.PassengerCount)))
            {
                _logger?.LogInformation($"Duplicate record found: {trip}");
                yield return trip;
                continue;
            }

            uniqueTrips.Add(TransformRecord(trip));
        }

        await _repository.InsertRangeAsync(uniqueTrips);
    }

    private TaxiTrip TransformRecord(TaxiTrip trip)
    {
        trip.StoreAndForwardFlag = !trip.StoreAndForwardFlag;
        if (_localTimezone is not null)
        {
            trip.PickupTime = TimeZoneInfo.ConvertTimeToUtc(trip.PickupTime, _localTimezone);
            trip.DropOffTime = TimeZoneInfo.ConvertTimeToUtc(trip.DropOffTime, _localTimezone);
        }

        return trip;
    }
}