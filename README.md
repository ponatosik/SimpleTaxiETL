A simple CLI project that saves taxi ride data into database. Processes a CSV file, cleans the data, and efficiently insert it into a Microsoft SQL Server database.

### Prerequisites
 - SQL Server: Set up a database using the provided [schema script](https://github.com/ponatosik/SimpleTaxiETL/blob/master/Data/SQL/init.sql).
 - Input CSV File.

### Run options
 - `---file [file destination]` - Specify the path to the input CSV file. If omitted, the file path will be prompted before execution.
 - `--connection-string [SQL Server connection string]` - Provide the SQL Server connection string **with the default database specified**. If omitted, it will will be prompted before execution.
 - `--duplicates [file destination]` - Specify the output path for a CSV file containing duplicated records. Defaults to the a file in current directory (duplicates.csv).
 - `---logs` - Enable applicatin logs to be displayed during execution.

### Possible improvmenets for larger files
 - **Batch processing**. Divide the insert operation into several database queries to minimize memory usage and improve efficiency.
 - **Parallel Bulk Inserts**. Split the input file into multiple regions and process them simultaneously to read batches faster.
 - **SQL Server bulk copy directly from csv file**. The best option, but the preprocesses (fwd flag transform and timezone conversion) must be done to entire csv file (or different batches) before sending to a database.
