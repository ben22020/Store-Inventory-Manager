namespace Project1.Logic{

    public class Order{

        public string storeID { get; set; }
        public string customerID { get; set; }
        public DateTime date { get; set; }

        public string productID { get; set; }

        public string quantity { get; set; }

        public Order(string storeID, string customerID, DateTime date, string productID, string quantity)
        {
            this.storeID = storeID;
            this.customerID = customerID;
            this.date = date;
            this.productID = productID;
            this.quantity = quantity; 
        }

        
        




    }
}