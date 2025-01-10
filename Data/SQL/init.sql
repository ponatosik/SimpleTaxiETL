CREATE
DATABASE TaxiTripDB;
GO

USE TaxiTripDB;
GO

CREATE TABLE TaxiTrips
(
    tpep_pickup_datetime  DATETIME NOT NULL,
    tpep_dropoff_datetime DATETIME NOT NULL,
    passenger_count       INT CHECK (passenger_count >= 0),
    PRIMARY KEY (tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count),

    trip_distance         DECIMAL(10, 2) CHECK (trip_distance >= 0),
    store_and_fwd_flag    BIT,
    PULocationID          INT      NOT NULL,
    DOLocationID          INT      NOT NULL,
    fare_amount           DECIMAL(10, 2) CHECK (fare_amount >= 0),
    tip_amount            DECIMAL(10, 2) CHECK (tip_amount >= 0)
);
GO

CREATE INDEX idx_PULocationID ON TaxiTrips (PULocationID);
GO

CREATE INDEX idx_trip_distance ON TaxiTrips (trip_distance);
GO