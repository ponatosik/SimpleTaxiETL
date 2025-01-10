using Data.Entities;
using Data.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class TaxiTripService
{
    private readonly ILogger? _logger;
    private readonly TaxiTripRepository _repository;

    public TaxiTripService(TaxiTripRepository repository, ILogger? logger = null)
    {
        _repository = repository;
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

            trip.StoreAndForwardFlag = !trip.StoreAndForwardFlag;
            uniqueTrips.Add(trip);
        }

        await _repository.InsertRangeAsync(uniqueTrips);
    }
}