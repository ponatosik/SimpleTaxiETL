namespace Data.Entities;

public record TaxiTrip
{
    public required decimal Distance;
    public required int DropOffLocationId;
    public required DateTime DropOffTime;
    public required decimal FareAmount;
    public required int PassengerCount;
    public required int PickupLocationId;
    public required DateTime PickupTime;
    public required bool StoreAndForwardFlag;
    public required decimal TipAmount;
}