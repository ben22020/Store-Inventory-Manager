namespace Project1.DB{

    using Microsoft.Data.SqlClient;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Project1.Logic;

    public class DBInteraction : IDBCommands
    {


        private readonly string connectionString;
        private readonly ILogger<DBInteraction> logger;

        /// <summary>
        ///     connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DBInteraction(string connectionString, ILogger<DBInteraction> logger)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.logger = logger;
        }

        /// <summary>
        ///     This method adds a new customer to the database
        /// </summary>
        /// <param name="firstName"> first name of customer to be added</param>
        /// <param name="lastName">last name of customer to be added</param>
       
        /// <returns>void</returns>
        public async Task AddNewCustomerAsync(string firstName, string lastName)
        {

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string cmdText = @"INSERT INTO Customer (firstName, lastName) VALUES (@firstName, @lastName);";
            using SqlCommand command = new(cmdText, connection);

            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);

            command.ExecuteNonQuery();
            await connection.CloseAsync();

        }
        /// <summary>
        ///     This method adds a new location to the database
        /// </summary>
        /// <param name="storeName"> first name of Location to be added</param>

        /// <returns>void</returns>
        public async Task AddNewLocationAsync(string storeName)
        {

            using SqlConnection connection = new(connectionString);

            await connection.OpenAsync();

            string cmdText = @"INSERT INTO Location (LocationName) VALUES (@storeName);";
            using SqlCommand command = new(cmdText, connection);

            command.Parameters.AddWithValue("@storeName", storeName);

            command.ExecuteNonQuery();
            await connection.CloseAsync();

        }
        /// <summary>
        ///     This method searches the database for a given customer
        ///     and returns a list of IDs attached to the passed name
        /// </summary>
        /// <param name="firstName"> first name of customer to be searched for</param>
        /// <param name="lastName"> last name of customer to be searched</param>

        /// <returns>IEnumerable<Customer></returns>

        public async Task<IEnumerable<Customer>> findCustomerAsync(string firstName, string lastName)
        {

            List<Customer> result = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new(@"SELECT * FROM Customer WHERE firstName = @firstName AND lastName = @lastName", connection);

            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);

            using SqlDataReader reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                
            }

            await connection.CloseAsync();

            return result;

        }
        /// <summary>
        ///     This method lists order details for a given customer
        /// 
        /// </summary>
        /// <param name="customerID"> customerID orders are attached to</param>

        /// <returns>IEnumerable<Order></returns>


        public async Task<IEnumerable<Order>> listOrderDetailsOfCustomerAsync(string customerID)
        {
            List<Order> result = new();
            
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new(@"SELECT * FROM Invoice JOIN InvoiceLine ON Invoice.OrderID = InvoiceLine.OrderID AND CustomerId = @customerID;", connection);

            command.Parameters.AddWithValue("@customerID", customerID);

            using SqlDataReader reader = command.ExecuteReader();

            while(await reader.ReadAsync())
            {
                result.Add(new(reader.GetInt32(2).ToString(), customerID, reader.GetDateTime(3), reader.GetInt32(6).ToString(), reader.GetInt32(7).ToString()));
                
            }

            await connection.CloseAsync();
            
            return result;

            
     
        }
        /// <summary>
        ///     This method lists order details for a given location
        /// 
        /// </summary>
        /// <param name="locationID"> locationID orders are attached to</param>

        /// <returns>IEnumerable<Order></returns>

        public async Task<IEnumerable<Order>> listOrderDetailsOfLocationAsync(string locationID)
        {
            List<Order> result = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new(@"SELECT * FROM Invoice JOIN InvoiceLine ON Invoice.OrderID = InvoiceLine.OrderID AND LocationID = @locationID;", connection);

            command.Parameters.AddWithValue("@locationID", locationID);

            using SqlDataReader reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                
                result.Add(new(reader.GetInt32(2).ToString(), reader.GetInt32(1).ToString(), reader.GetDateTime(3), reader.GetInt32(6).ToString(), reader.GetInt32(7).ToString()));

            }

            await connection.CloseAsync();

            return result;



        }
        /// <summary>
        ///     decreases the stock of an item in a locations inventory
        /// 
        /// </summary>
        /// <param name="locationID"> location inventory is attached to to</param>
        /// <param name="productID"> ID of product in inventory</param>
        /// <param name="amount"> amount to decrease store inventory by</param>

        /// <returns>void</returns>
        public async Task decreaseStockAsync(string locationID, string productID, int amount)
        {

            int locationStock = 0; 

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new(@"SELECT Stock FROM LocationInventory WHERE LocationID = @locationID AND ProductID = @productID; ", connection);

            command.Parameters.AddWithValue("@locationID", locationID);
            command.Parameters.AddWithValue("@productID", productID);

            using SqlDataReader reader = command.ExecuteReader();

            await reader.ReadAsync();

            locationStock = reader.GetInt32(0);

            await connection.CloseAsync();

            locationStock = locationStock - amount;

            await connection.OpenAsync();
            
            using SqlCommand command2 = new(@"UPDATE LocationInventory SET Stock = @locationStock WHERE LocationID = @locationID AND ProductID = @productID; ", connection);

            command2.Parameters.AddWithValue("@locationID", locationID);
            command2.Parameters.AddWithValue("@productID", productID);
            command2.Parameters.AddWithValue("@locationStock", locationStock);
            command2.ExecuteNonQuery();

            await connection.CloseAsync();

        }
        /// <summary>
        ///    supplementary method to placeOrder.
        /// 
        /// </summary>
        /// <param name="locationID"> ID of location</param>
        /// <param name="productID"> ID of product to be ordered</param>
        /// <param name="quantity"> amount to be ordered of product</param>
        /// <param name="orderID"> ID of order to be added to</param>
        

        /// <returns>void</returns>

        public async Task addItemsToOrder(string orderID, string locationID, string productID, int quantity)
        {
            using SqlConnection connection = new(connectionString);


            await connection.OpenAsync();

            if (await checkStoreHasEnoughAsync(locationID, productID, quantity))
            {
                using SqlCommand command = new(@"INSERT INTO InvoiceLine (OrderID, ProductID, Quantity) 
                                                VALUES (@orderID, @productID, @quantity); ", connection);
                command.Parameters.AddWithValue("@orderID", orderID);
                command.Parameters.AddWithValue("@productID", productID);
                command.Parameters.AddWithValue("@quantity", quantity);

                command.ExecuteNonQuery();

                await connection.CloseAsync();

                await decreaseStockAsync(locationID, productID, quantity);
            }

            await connection.CloseAsync();
        }

        /// <summary>
        ///    a method to get an orderId attached to a certain date.
        /// 
        /// </summary>
        /// <param name="date"> date of order to be searched for</param>
        /// <returns>id of order with associated date</returns>
        public async Task<int> getOrderIDFromDateAsync(string date)
        {

            using SqlConnection connection = new(connectionString);


            await connection.OpenAsync();

            string cmdText = @"SELECT OrderID FROM Invoice WHERE OrderDate = @date;";


            using SqlCommand command = new(cmdText, connection);

            command.Parameters.AddWithValue("@date", date);

            using SqlDataReader reader = command.ExecuteReader();

            await reader.ReadAsync();

            int orderID = reader.GetInt32(0);

            await connection.CloseAsync();

            return orderID;
        }

        /// <summary>
        ///    Checks to see if store has enough remaining inventory to process order
        /// 
        /// </summary>
        /// <param name="locationID"> ID of location</param>
        /// <param name="productID"> ID of product to be ordered</param>
        /// <param name="quantity"> amount to be ordered of product</param>


        /// <returns>true/false, true if there is enough inventory</returns>

        public async Task<bool> checkStoreHasEnoughAsync(string locationID, string productID, int quantity)
        {
            int locationStock = 0;

            using SqlConnection connection = new(connectionString);

            await connection.OpenAsync();

            using SqlCommand command4 = new(@"SELECT Stock FROM LocationInventory WHERE LocationID = @locationID 
                                            AND ProductID = @productID; ", connection);

            command4.Parameters.AddWithValue("@locationID", locationID);
            command4.Parameters.AddWithValue("@productID", productID);

            using SqlDataReader reader2 = command4.ExecuteReader();

            await reader2.ReadAsync();

            locationStock = reader2.GetInt32(0);

            await connection.CloseAsync();

            if (locationStock < quantity)
            {
                Console.WriteLine("Location does not have enough inventory for that order");
                return false;
            }

            else
            {
                return true;
            }
        }
        /// <summary>
        ///   Lists all products in database and their associated IDs
        /// 
        /// </summary>
        /// <returns>List of products available and their associated IDs</returns>
        public IEnumerable<String> productCatalogue()
        {
            List<String> result = new();

            using SqlConnection connection = new(connectionString);

            connection.Open();

            string cmdText = "SELECT * FROM PRODUCT;";

            using SqlCommand command = new(cmdText, connection);

            using SqlDataReader reader = command.ExecuteReader();


            Console.WriteLine("------PRODUCT CATALOGUE------");
            while (reader.Read())
            {

                string productID = reader.GetInt32(0).ToString();

                string productName = reader.GetString(1);

                Console.WriteLine($"ID: {productID} Name: {productName}");

                result.Add(productID);
                result.Add(productName);

            }

            connection.Close();

            return result;
        }

        /// <summary>
        /// Gets details of an order given an orderID
        /// </summary>
        /// <param name="orderID"> ID of order</param>
        /// <returns>a list of order details associated with the order</returns>

        public async Task<IEnumerable<string>> GetOrderDetailsAsync(int orderID)
        {
            List<String> result = new();

            using SqlConnection connection = new(connectionString);

            await connection.OpenAsync();

            string cmdText = @"SELECT * FROM InvoiceLine WHERE OrderID = @orderID";
            using SqlCommand command = new(cmdText, connection);

            command.Parameters.AddWithValue("@orderID", orderID);

            using SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"------Details of order # {orderID}------");
            
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Line ID: {reader.GetInt32(0)} Product ID: {reader.GetInt32(2)} Quantity: {reader.GetInt32(3)} ");
                result.Add(reader.GetInt32(0).ToString());
                result.Add(reader.GetInt32(2).ToString());
                result.Add(reader.GetInt32(3).ToString());

            }

            await connection.CloseAsync();

            return result;
        }

        /// <summary>
        ///    Places an order for a customer and updates the values in the database
        /// 
        /// </summary>
        /// <param name="locationID"> ID of location to be ordered from</param>
        /// <param name="productID"> ID of product to be ordered</param>
        /// <param name="quantity"> amount to be ordered of product</param>
        /// <param name="date"> date the order is processed</param>
        ///<param name="customerID"> ID of customer doing the ordering</param>
        /// <returns>void</returns>
        public async Task PlaceOrderAsync(string customerID, string locationID, string date, string productID, string quantity)
        {
         
            
            using SqlConnection connection = new(connectionString);

           
              if (await checkStoreHasEnoughAsync(locationID, productID, Convert.ToInt32(quantity)))
              {
             
                await connection.OpenAsync();

                string cmdText = @"INSERT INTO Invoice (CustomerId, LocationId, OrderDate) 
                                    VALUES (@customerID, @locationID, @date);";
                using SqlCommand command = new(cmdText, connection);

                command.Parameters.AddWithValue("@customerID", customerID);
                command.Parameters.AddWithValue("@locationID", locationID);
                command.Parameters.AddWithValue("@date", date);

                command.ExecuteNonQuery();
                await connection.CloseAsync();


                int orderID = await getOrderIDFromDateAsync(date);

                await connection.OpenAsync();

                cmdText = @"INSERT INTO InvoiceLine (OrderID, ProductID, Quantity)
                             VALUES (@orderID, @productID, @quantity);";

                using SqlCommand command3 = new(cmdText, connection);



                command3.Parameters.AddWithValue("@orderID", orderID);
                command3.Parameters.AddWithValue("@productID", productID);
                command3.Parameters.AddWithValue("@quantity", quantity);

                command3.ExecuteNonQuery();
                await connection.CloseAsync();

                await decreaseStockAsync(locationID, productID, Convert.ToInt32(quantity));

              }
        }
    }
}