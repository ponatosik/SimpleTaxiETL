using Data.Entities;

namespace CvsReader;

public class CvsReader
{
    public IEnumerable<TaxiTrip> ReadFile()
    {
        using (var reader = new StreamReader("filePersons.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Person>();
        }
    }
}