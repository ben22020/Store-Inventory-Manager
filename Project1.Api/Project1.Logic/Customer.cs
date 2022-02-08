namespace Project1.Logic{

    using System.Data.SqlClient;
 
    public class Customer{

        public int customerID { get; set; }
        public string firstName{get; set;}
        public string lastName {get; set;}

        public Customer(int customerID, string firstName, string lastName){

            this.customerID = customerID;
            this.firstName = firstName;
            this.lastName = lastName;
           

        }

        

        }
}
