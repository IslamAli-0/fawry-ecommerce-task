namespace e_commerece_system
{


    abstract class Product
    {
        public string Name;
        public decimal Price;
        public int Quantity;
        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    public interface Ishippable
    {
        string getname();
        double getweight();
    }

    public interface Iexpirable
    {
        bool IsExpired();
    }



     class Cheese : Product, Ishippable, Iexpirable
    {
        public double Weight;
        public DateTime ExpirationDate;
        public Cheese(string name, decimal price, int quantity, double weight, DateTime expirationDate) : base(name, price, quantity)
        {
            Weight = weight;
            ExpirationDate = expirationDate;
        }

        public string getname() => Name;

        public double getweight() => Weight;

        public bool IsExpired() => DateTime.Now > ExpirationDate;
    }

    class Biscuits : Product, Ishippable, Iexpirable
    {
        public double Weight;
        public DateTime ExpirationDate;
        public Biscuits(string name, decimal price, int quantity, double weight, DateTime expirationDate) : base(name, price, quantity)
        {
            Weight = weight;
            ExpirationDate = expirationDate;
        }
        public string getname() => Name;
        public double getweight() => Weight;
        public bool IsExpired() => DateTime.Now > ExpirationDate;
    }

    class TV : Product, Ishippable
    {
        public double Weight;
        public TV(string name, decimal price, int quantity, double weight) : base(name, price, quantity)
        {
            Weight = weight;
        }
        public string getname() => Name;
        public double getweight() => Weight;
    }

    class ScratchCard : Product
    {
        public ScratchCard(string name, decimal price, int quantity) : base(name, price, quantity)
        {
        }
    }
    
    class Customer
    {
        public string Name;
        public decimal Balance;

        public Customer(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }
    }

    class Cart
    { private Dictionary<Product,int> items = new Dictionary<Product, int>();
        public void Add(Product product, int quantity)
        {
            if (product.Quantity < quantity)
            {
                throw new InvalidOperationException("Not enough stock available.");
            }
            if (items.ContainsKey(product))
            {
                items[product] += quantity;
            }
            else
            {
                items[product] = quantity;
            }
        }
        public Dictionary<Product, int> GetItems() => items;
    }

    class ShippingService // could have made it into a static class
    {
        public void Ship(List<Ishippable> item)
        {
            double totalWeight = 0;
            Console.WriteLine("** Shippment Notice **");
            foreach (var i in item)
            {
                Console.WriteLine($"{i.getname()} {i.getweight()}g");
                totalWeight += i.getweight();
            }
            Console.WriteLine($"Total package weight {totalWeight / 1000.0}kg");
        }
    }

    internal class Program
    {

        static void Checkout(Customer customer, Cart cart)
        {
            var items = cart.GetItems();
            if (items.Count == 0)
                throw new Exception("Cart is empty");

            decimal subtotal = 0;
            List<Ishippable> shippables = new();

            foreach (var pair in items)
            {
                var product = pair.Key;
                int qty = pair.Value;

                if (product is Iexpirable expirable && expirable.IsExpired())
                    throw new Exception($"{product.Name} is expired");

                if (qty > product.Quantity)
                    throw new Exception($"{product.Name} out of stock");

                subtotal += product.Price * qty;

                if (product is Ishippable shippable)
                    shippables.Add(shippable);
            }

            decimal shipping = shippables.Count > 0 ? 30 : 0;
            decimal total = subtotal + shipping;

            if (customer.Balance < total)
                throw new Exception("Insufficient balance");

            customer.Balance -= total;

            if (shippables.Count > 0)
                new ShippingService().Ship(shippables);

            Console.WriteLine("\n** Checkout receipt **");
            foreach (var pair in items)
            {
                Console.WriteLine($"{pair.Value}x {pair.Key.Name} \t {pair.Key.Price * pair.Value}");
            }
            Console.WriteLine("----------------------");
            Console.WriteLine($"Subtotal \t {subtotal}");
            Console.WriteLine($"Shipping \t {shipping}");
            Console.WriteLine($"Amount   \t {total}");
            Console.WriteLine($"Balance  \t {customer.Balance}");
        }
        static void Main(string[] args)
        {
            var cheese = new Cheese("Cheese", 100, 5, 400, DateTime.Now.AddDays(1));
            var biscuits = new Biscuits("Biscuits", 150, 2, 700, DateTime.Now.AddDays(1));
            var scratchCard = new ScratchCard("ScratchCard", 50, 10);
            var tv = new TV("TV", 300, 3, 5000);

            var customer = new Customer("Ali", 1000);
            var cart = new Cart();
            cart.Add(cheese, 2);
            cart.Add(biscuits, 1);
            cart.Add(scratchCard, 1);

            Checkout(customer, cart);


            // Edge case: empty cart
            // var emptyCart = new Cart();
            // Checkout(customer, emptyCart); // Should throw "Cart is empty"

            // Edge case: expired product
            // var expiredCheese = new Cheese("Old Cheese", 80, 2, 400, DateTime.Now.AddDays(-1));
            // var cartWithExpired = new Cart();
            // cartWithExpired.Add(expiredCheese, 1);
            // Checkout(customer, cartWithExpired); // Should throw "Old Cheese is expired"

            // Edge case: insufficient stock
            // var cartWithTooMuch = new Cart();
            // cartWithTooMuch.Add(cheese, 100); // Should throw "Cheese out of stock"

            // Edge case: insufficient balance
            // var poorCustomer = new Customer("LowCash", 50);
            // var poorCart = new Cart();
            // poorCart.Add(tv, 1);
            // Checkout(poorCustomer, poorCart); // Should throw "Insufficient balance"
        }
    }
}
