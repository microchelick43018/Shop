using System;
using System.Collections.Generic;
namespace Shop
{
    class Program
    {
        static void ShowMenu()
        {
            Console.WriteLine("1 - Show seller's items.");
            Console.WriteLine("2 - Buy an item.");
            Console.WriteLine("3 - Show player's stuff.");
            Console.WriteLine("4 - Get money.");
            Console.WriteLine("5 - Exit.");
        }

        static void Main(string[] args)
        {
            Player player = new Player("User", 100);
            Seller seller = new Seller();
            bool exit = false;
            int choice;
            seller.Greetings();
            
            while (exit == false)
            {
                ShowMenu();
                choice = InputChecker.MakeChoice();
                switch (choice)
                {
                    case 1:
                        seller.ShowProductList();
                        break;
                    case 2:
                        player.BuyProduct(seller);
                        break;
                    case 3:
                        player.ShowStuff();
                        break;
                    case 4:
                        player.DoTheQuest();                       
                        break;
                    case 5:
                        exit = true;
                        break;
                    default:
                        break;
                }
                Console.WriteLine("Enter any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }           
        }
    }

    class InputChecker
    {
        public static int ReadInt()
        {
            bool isCorrected = int.TryParse(Console.ReadLine(), out int choice);
            while (isCorrected == false || choice < 0)
            {
                Console.WriteLine("Wrong input. Try again: ");
                isCorrected = int.TryParse(Console.ReadLine(), out choice);
            }
            return choice;
        }

        public static int MakeChoice(int maxNumber = 5)
        {
            int choice = ReadInt();
            while (choice > maxNumber || choice < 1)
            {
                Console.Write("Wrong input. Try again: ");
                choice = ReadInt();
            }
            return choice;
        }
    }

    abstract class Person
    {
        public string Name { get; protected set; }

        public Person(string name)
        {
            Name = name;
        }
    }

    class Player : Person
    {
        private List<Product> _stuff;

        public int Rupees { get; private set; }

        public Player(string name, int rupees) : base(name)
        {
            Rupees = rupees;
            _stuff = new List<Product>();
            Name = name;
        }

        public void Pay(int sum)
        {
            Rupees -= sum;
        }

        public void AddToStuff(Product boughtProduct)
        {
            foreach (var stuff in _stuff)
            {
                if (stuff.Name == boughtProduct.Name)
                {
                    stuff.SetItemQuantity(stuff.ItemQuantity + boughtProduct.ItemQuantity);
                    return;
                }
            }
            _stuff.Add(boughtProduct);
        }

        public void BuyProduct(Seller seller)
        {
            ShowBalance();
            seller.ShowProductList();
            Console.Write($"{Name}: I actualy want...(Enter the name of the product you want to buy): ");
            string name = Console.ReadLine();
            Product boughtItem = seller.SellProduct(name, this);
            if (boughtItem != null)
            {
                AddToStuff(boughtItem);
            }
        }
        
        public void DoTheQuest()
        {
            Random random = new Random();
            int gotRupees = random.Next(5, 10);
            Console.WriteLine($"You have completed the quest! You've got {gotRupees} rupees");
            Rupees += gotRupees;
        }

        public void ShowStuff()
        {
            if (_stuff.Count != 0)
            {
                Console.WriteLine("Stuff of player: ");
                foreach (var product in _stuff)
                {
                    product.ShowInfo();
                }
            }
            else
            {
                Console.WriteLine("There's no item.");
            }
            ShowBalance();
        }

        public void ShowBalance()
        {
            Console.WriteLine($"Balance: {Rupees} rupees");
        }
    }

    class Seller : Person
    {
        private List<Product> _productList;

        public Seller(string name = "Morshu") : base(name)
        {
            _productList = new List<Product>();
            Name = name;
            CreateProductList();
        }

        public void CreateProductList()
        {
            Console.WriteLine("Creating a list of seller's products:\n");
            LampOil lampOil = new LampOil();            
            Rope rope = new Rope();            
            Bomb bomb = new Bomb();
            lampOil.SetParams();
            rope.SetParams();
            bomb.SetParams();
            _productList.Add(lampOil);
            _productList.Add(rope);
            _productList.Add(bomb);
            Console.Clear();
        }

        public void Greetings()
        {
            Console.WriteLine("LampOil? Rope? Bombs?");
            Console.WriteLine("You want it?");
            Console.WriteLine("It's all yours, my friend.");
            Console.WriteLine("As long as you have enough rupees.");
        }

        public Product MakeADeal(int numberOfProduct, int count, Player player)
        {
            Console.WriteLine($"{Name}: Deal!");
            player.Pay(count * _productList[numberOfProduct].Cost);
            Product itemForSale = _productList[numberOfProduct].Get();
            itemForSale.SetItemQuantity(count);
            RemoveProductFromWarehouse(numberOfProduct, count);
            return itemForSale;
        }

        public int FindNumberProduct(string name)
        {
            for (int i = 0; i < _productList.Count; i++)
            {
                if (_productList[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public void RemoveProductFromWarehouse(int numberOfProduct, int count)
        {
            _productList[numberOfProduct].SetItemQuantity(_productList[numberOfProduct].ItemQuantity - count);
            if (_productList[numberOfProduct].ItemQuantity == 0)
            {
                _productList.RemoveAt(numberOfProduct);
            }
        }

        public void SayToBreakDeal(Player player)
        {
            Console.WriteLine($"{Name}: Sorry, {player.Name}, i can't give you a credit.");
            Console.WriteLine("Come back when you're a little... hmmm richer.");
        }

        public void ShowProductList()
        {
            foreach (var product in _productList)
            {
                product.ShowInfo();
            }
        }

        public Product SellProduct(string name, Player player)
        {
            int numberOfProduct = FindNumberProduct(name);
            if (numberOfProduct != -1)
            {
                Product itemForSale = _productList[numberOfProduct];
                Console.WriteLine($"{Name}: I've got it! How many you want to buy?");
                int count = InputChecker.ReadInt();
                if (itemForSale.ItemQuantity >= count)
                {
                    if (count * itemForSale.Cost > player.Rupees)
                    {
                        SayToBreakDeal(player);
                        return null;
                    }
                    else
                    {
                        return MakeADeal(numberOfProduct, count, player);
                    }
                }
                else
                {
                    Console.WriteLine($"{Name}: Sorry, i don't have {name} enough for you.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"{Name}: Sorry, i can't find {name}!");
                return null;
            }
        }
    }

    class LampOil : Product
    {
        public string Model { get; private set; }

        public LampOil() : base()
        {
            Name = "LampOil";
        }

        public LampOil(LampOil obj) : base(obj)
        {
            Model = obj.Model;
        }

        public LampOil(int cost, int count, string model) : base(cost, count)
        {
            Name = "LampOil";
            Model = model;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"model: {Model}");
        }

        public override void SetParams()
        {
            base.SetParams();
            Console.Write($"Enter model: ");
            Model = Console.ReadLine();
        }

        public override Product Get()
        {
            return new LampOil(this);
        }
    }

    class Rope : Product
    {
        public int Length { get; private set; }

        public Rope() : base()
        {
            Name = "Rope";
            Length = 0;
        }

        public Rope(Rope obj) : base(obj)
        {
            Length = obj.Length;
        }

        public Rope(int cost, int count, int length) : base(cost, count)
        {
            Length = length;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"length: {Length}");
        }

        public override void SetParams()
        {
            base.SetParams();
            Console.Write($"Enter length: ");
            Length = InputChecker.ReadInt();
        }

        public override Product Get()
        {
            return new Rope(this);
        }
    }
    
    class Bomb : Product
    {
        public int Power { get; private set; }

        public Bomb() : base()
        {
            Name = "Bomb";
            Power = 0;
        }

        public Bomb(Bomb obj) : base(obj)
        {
            Power = obj.Power;
        }

        public Bomb(int cost, int count, int power) : base(cost, count)
        {
            Name = "Bomb";
            Power = power;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"power: {Power}");
        }

        public override void SetParams()
        {            
            base.SetParams();
            Console.Write($"Enter power: ");
            Power = InputChecker.ReadInt();
        }

        public override Product Get()
        {
            return new Bomb(this);
        }
    }

    class Product
    {
        public string Name { get; protected set; }
        public int Cost { get; protected set; }
        public int ItemQuantity { get; protected set; }

        public Product()
        {
            Cost = 0;
            ItemQuantity = 0;
        }

        public Product(Product obj)
        {
            Name = obj.Name;
            Cost = obj.Cost;
            ItemQuantity = obj.ItemQuantity;
        }

        public Product(int cost, int count)
        {
            Cost = cost;
            ItemQuantity = count;
        }

        public void SetItemQuantity(int count)
        {
            ItemQuantity = count;
        }

        public virtual void ShowInfo()
        {
            Console.Write($"Product name: {Name}, cost: {Cost}, count: {ItemQuantity}, ");
        }

        public virtual Product Get()
        {         
            return new Product(this);
        }

        public virtual void SetParams()
        {
            Console.WriteLine($"{Name}:");
            Console.Write("Enter name: ");
            Name = Console.ReadLine();
            Console.Write("Enter cost: ");
            Cost = InputChecker.ReadInt();
            Console.Write("Enter count: ");
            ItemQuantity = InputChecker.ReadInt();
        }
    }
}