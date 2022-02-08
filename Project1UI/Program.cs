// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Project1UI
{
    public class Program
    {
        public static readonly HttpClient HttpClient = new();
        public static readonly ILogger<OrderService> logger;

        public async static Task Main(string[] args)
        {

            Uri server = new Uri("https://benhp1.azurewebsites.net/");
            OrderService orderService = new OrderService(server);
            CustomerService customerService = new CustomerService(server);
            LocationService locationService = new LocationService(server);

            Console.WriteLine("------------ORDER MANAGEMENT SYSTEM-----------");




            while (true)
            {


                Console.WriteLine("Enter 1 For Menu Options, otherwise enter your instruction");
                Console.Beep();

                int switchController = Convert.ToInt32(Console.ReadLine());


                switch (switchController)
                {
                    case 1:
                        MenuOptions();
                        break;
                    case 2:
                        await AddCustomerConsole(customerService);
                        break;
                    case 3:
                        await AddLocationConsole(locationService);
                        break;
                    case 4:
                        await PlaceOrderConsole(orderService);
                        break;
                    case 5:
                        await  ListLocationOrderConsole(orderService);
                        break;
                    case 6:
                        await ListCustomerOrderConsole(orderService);
                        break;
                    case 7:
                        await FindCustomerConsole(customerService);
                        break;
                    case 8:
                        ProductCatalogue();
                        break;
                    case 9:
                        Environment.Exit(0);
                        break;

                }
            }


        }
        public static void MenuOptions()
        {
            Console.Clear();
            Console.WriteLine("1: Menu Options");
            Console.WriteLine("2: Add customer");
            Console.WriteLine("3: Add Location");
            Console.WriteLine("4: Place Order");
            Console.WriteLine("5: Location Order List");
            Console.WriteLine("6: Customer Order List");
            Console.WriteLine("7: Search Customer");
            Console.WriteLine("8: Product Catalogue");
            Console.WriteLine("9: Quit Application");
        }

        public async static Task AddCustomerConsole(CustomerService customerService)
        {
            Console.Clear();
            string? firstName;
            string? lastName;
            Console.WriteLine("Enter customer first name");

            firstName = Console.ReadLine() ?? throw new ArgumentNullException(); ;

            Console.WriteLine("Enter customer last name");

            lastName = Console.ReadLine() ?? throw new ArgumentNullException(); ;


            await customerService.AddNewCustomerAsync(firstName, lastName);

            Console.WriteLine($"{firstName} {lastName} added to database");


        }
        public async static Task ProductCatalogue()
        {
            Console.WriteLine("-----Products-----");
            Console.WriteLine("1. Tacos");
            Console.WriteLine("2. Burritos");
            Console.WriteLine("3. Nachos");
            Console.WriteLine("4. Salsa");
            Console.WriteLine("5. Pico");
            Console.WriteLine("6. Guac");
        
        }


            public async static Task AddLocationConsole(LocationService locationService)
        {
            Console.Clear();
            string? storeName;

            Console.WriteLine("Enter Location Name");

            storeName = Console.ReadLine() ?? throw new ArgumentNullException();


            await locationService.AddNewLocationAsync(storeName);

            Console.WriteLine($"{storeName} added to database");


        }

        public async static Task PlaceOrderConsole(OrderService orderService)
        {
            Console.Clear();
            ProductCatalogue();
            Console.WriteLine("Enter Customer ID: ");
            string? customerID = Console.ReadLine() ?? "1";
            Console.WriteLine("Enter location ID: ");
            string? locationID = Console.ReadLine() ?? "1";
            Console.WriteLine("Enter Product ID: ");
            string? productID = Console.ReadLine() ?? "1";
            Console.WriteLine("Enter Quantity");
            string? quantity = Console.ReadLine() ?? throw new ArgumentNullException();

            string date = DateTime.Now.ToString();

            await orderService.PlaceOrderAsync(customerID, locationID, date, productID, quantity);

/*
            while (true)
            {
                Console.WriteLine("Add more items to order? (Y/N)");
                string prompt = Console.ReadLine() ?? "N";

                if (prompt == "Y")
                {
                    Console.WriteLine("Enter Product ID: ");
                    productID = Console.ReadLine() ?? "1";
                    Console.WriteLine("Enter Quantity");
                    quantity = Console.ReadLine();

                    //cmd.addItemsToOrder(cmd.getOrderIDFromDate(date).ToString(), locationID, productID, quantity);
                }
                else if (prompt == "N")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid response");
                }


            }*/
           // cmd.getOrderDetails(cmd.getOrderIDFromDate(date));
        }

        public async static Task ListLocationOrderConsole(OrderService orderService)
        {

            Console.Clear();
            Console.WriteLine("Enter the ID of the Location you would like to view the order history of");

            string locID = Console.ReadLine() ?? throw new ArgumentNullException();
            
          
            List<Order> orders;



            orders = await orderService.ListOrderDetailsOfLocationAsync(locID);

            foreach (Order order in orders)
            {
                Console.WriteLine($"Customer #{order.customerID} placed an order for {order.quantity} units of product #{order.productID} on {order.date}");
            }

        }

        public async static Task ListCustomerOrderConsole(OrderService orderService)
        {

            Console.Clear();
            Console.WriteLine("Enter the ID of the Customer you would like to view the order history of");
            string customerID = Console.ReadLine() ?? throw new ArgumentNullException();


            List<Order> orders;



            orders = await orderService.ListOrderDetailsOfCustomerAsync(customerID);

            foreach (Order order in orders)
            {
                Console.WriteLine($"Customer #{order.customerID} placed an order for {order.quantity} units of product #{order.productID} on {order.date}");
            }
        }

        public static void ListOrderDetailsConsole()
        {
            Console.WriteLine("Enter the OrderID of the order");
            int orderId = Convert.ToInt32(Console.ReadLine());

           // cmd.getOrderDetails(orderId);
        }

        public async static Task FindCustomerConsole(CustomerService customerService)
        {
            Console.Clear();
            Console.WriteLine("Enter the first name followed by the last name");

            string? firstName = Console.ReadLine() ?? throw new ArgumentNullException();
            string? lastName = Console.ReadLine() ?? throw new ArgumentNullException();

            List<Customer> customers;

            customers = await customerService.FindCustomerAsync(firstName, lastName);

            foreach (Customer customer in customers)
            {
                Console.WriteLine($"Customer {firstName} {lastName} found with ID {customer.customerID}");
            }



            //cmd.findCustomer(firstName, lastName);
        }
    }

}
