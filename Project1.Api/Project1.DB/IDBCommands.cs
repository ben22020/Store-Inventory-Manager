namespace Project1.DB{

    using Project1.Logic;
    
    public interface IDBCommands{

        Task AddNewCustomerAsync(string firstName, string lastName);
        Task AddNewLocationAsync(string storeName);

        Task PlaceOrderAsync(string customerID, string locationID, string date, string productID, string quantity);
        Task<IEnumerable<Customer>> findCustomerAsync(string firstName, string lastName);
        Task<IEnumerable<Order>> listOrderDetailsOfCustomerAsync(string customerID);
        Task<IEnumerable<Order>> listOrderDetailsOfLocationAsync(string locationID);


    }
}