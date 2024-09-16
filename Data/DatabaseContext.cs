using SQLite;
using System.Linq.Expressions;

namespace SQLMaui.Data
{
    public class DatabaseContext : IAsyncDisposable
    {
        private const string DbName = "todo.db3";
        // Save the path to the database locally -- save the database file in this specific path location
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbName);  // AppDataDirectory saves the location of where the data is stored
        // Declare the database connection
        private SQLiteAsyncConnection _connection;

        // This is a function that checks if there is a database connection, if not, it creates a new connection with the following properties
        private SQLiteAsyncConnection Database => 
            (_connection ??= new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));

        private async Task CreateTableIfNotExistsAsync<TTable>() where TTable : class, new()
        {
            // Create a table (If not exists) in the database with a generic type of Product
            await Database.CreateTableAsync<TTable>();
        }

        // This method is used to execute a query on the database asynchronously
        private async Task<TResult> Execute<TTable, TResult>(Func<Task<TResult>> action) where TTable : class, new()
        {
            await CreateTableIfNotExistsAsync<TTable>();
            return await action();
        }

        // This method is used to get the table from the database asynchronously
        private async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class, new()
        {
            // Confirms the table exists in the database otherwise creates a new table
            await CreateTableIfNotExistsAsync<TTable>();
            return Database.Table<TTable>();
        }

        // Represents an asynchronous operation -- inefficient way to query the database we need to create multiple database connections and it is not feasible
        /*
        public async Task Try()
        {
            // Create a table (If not exists) in the database with a generic type of Product
            await Database.CreateTableAsync<Product>();
            // Perfom operations on the database
            await Database.Table<Product>().ToListAsync();  // This method queris the database and returns a list of all the products. You can apply any query here.

        }
        */
        // This is a generic method that takes a type of TTable and returns a list of the type IEnumerable is used to iterate over the list of Objects
        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class, new() // GetTablesAsync is a generic method that takes a type of TTable and returns a list of the type IEnumerable is used to iterate over the list of Objects
        {
            // No need all of this anymore because we are using the helper methods to do the same avoiding coupling
            /*
            // Create a table (If not exists) in the database with a generic type of Product
            await Database.CreateTableAsync<TTable>();
            // Perfom operations on the database
            return await Database.Table<TTable>().ToListAsync();  // This method queries the database and returns a list of all the products. You can apply any query here.
            */
            var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }
        public async Task<IEnumerable<TTable>> GetFilteredAsync<TTable>(Expression<Func<TTable,bool>> predicate) where TTable : class, new()    // As you can see we are filtering by where clause and we need to pass the predicarte as shown as argument of the method
        {
            // Same as above, we are using the helper methods to avoid coupling
            /*
            // Create a table (If not exists) in the database with a generic type of Product
            await Database.CreateTableAsync<TTable>();
            // Perfom operations on the database
            return await Database.Table<TTable>().Where(predicate).ToListAsync();
            */
            var table = await GetTableAsync<TTable>();
            return await table.Where(predicate).ToListAsync();
        }

        // Implementing delete by ID Product to the database
        public async Task<TTable> GetItemByKeyAsync<TTable>(Object PrimaryKey) where TTable : class, new()
        {
            // Confirm if the table exists in the table first
            //await CreateTableIfNotExistsAsync<TTable>();
            // Use database method to delete the item into the table in the database
            //return await Database.GetAsync<TTable>(PrimaryKey);  // Apply this method to get the item of the TTable
            return await Execute<TTable, TTable>(async () => await Database.GetAsync<TTable>(PrimaryKey));

        }

        // Implementing adding a new product to the database
        public async Task<bool> AddItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            // Confirm if the table exists in the table first
            //await CreateTableIfNotExistsAsync<TTable>();
            // Use database method to insert the item into the database
            //return await Database.InsertAsync(item) > 0;    // if succesfull > 0 otherwise 0

            return await Execute<TTable, bool>(async () => await Database.InsertAsync(item) > 0);

        }

        // Implementing update a new product to the database
        public async Task<bool> UpdateItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            // Confirm if the table exists in the table first
            await CreateTableIfNotExistsAsync<TTable>();    //  YOU CAN CHANGE THIS LATER AND SEE IF IT WORKS
            // Use database method to update the item into the database
            return await Database.UpdateAsync(item) > 0;    // if succesfull > 0 otherwise 0

        }

        // Implementing delete Product to the database
        public async Task<bool> DeleteItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            // Confirm if the table exists in the table first
            await CreateTableIfNotExistsAsync<TTable>();    //  YOU CAN CHANGE THIS LATER AND SEE IF IT WORKS
            // Use database method to delete the item into the database
            return await Database.DeleteAsync(item) > 0;    // if succesfull > 0 otherwise 0

        }

        // Implementing delete by ID Product to the database
        public async Task<bool> DeleteItemByKeyAsync<TTable>(Object PrimaryKey) where TTable : class, new()
        {
            // Confirm if the table exists in the table first
            await CreateTableIfNotExistsAsync<TTable>();    //  YOU CAN CHANGE THIS LATER AND SEE IF IT WORKS
            // Use database method to delete the item into the table in the database
            return await Database.DeleteAsync<TTable>(PrimaryKey) > 0;  // Apply this method to delete the item of the TTable

        }

        // Implementing delete all Products to the database -- In case it is out of memory need to be deleted and created again
        public async ValueTask DisposeAsync()
        {
            await _connection?.CloseAsync();
        }
    }
}
