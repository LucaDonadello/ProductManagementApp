using SQLite;

namespace SQLMaui.Models
{
    public class Product
    {
        //Declared that this is a sql table with id as primary key and auto increment property
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }

        public Product Clone() => MemberwiseClone() as Product ?? new Product();    // Create a clone of the product object

        // Add validation method
        public (bool isValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, $"{nameof(Name)} is required.");
            }
            else if(Price <= 0)
            {
                return (false, $"{nameof(Price)} should be greather than 0");
            }
            else
            {
                return (true, null);
            }

        }

    }
}
