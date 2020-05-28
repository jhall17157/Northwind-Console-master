using NLog;
using NorthwindConsole.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Add a Product");
                    Console.WriteLine("2) Edit a Product");
                    Console.WriteLine("3) Display Products");
                    Console.WriteLine("4) Display a specific Products");
                    Console.WriteLine("5) Display Categories");
                    Console.WriteLine("6) Add Category");
                    Console.WriteLine("7) Edit Category");
                    Console.WriteLine("8) Display Category and related products");
                    Console.WriteLine("9) Display all Categories and their related products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        Product product = new Product();
                        Console.WriteLine("Enter the Product Name");
                        product.ProductName = Console.ReadLine();


                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();


                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();

                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {

                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddProduct(product);

                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }


                    }
                    else if (choice == "2")
                    {
                        var db = new NorthwindContext();
                        var p = GetProducts(db);
                                      
                        if (p != null)
                        {
                            Product UpdatedProduct = InputProduct(db);
                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductID = p.ProductID;
                                db.EditProduct(UpdatedProduct);
                                logger.Info("Product (id: {productid}) updated", UpdatedProduct.ProductID);
                            }
                        }
                        p.ProductName = Console.ReadLine();


                    }
                    else if (choice == "3")
                    {
                        Console.WriteLine("Would you like to see all? (1)  Active? (2) Or Disconinued? (3)");
                        choice = Console.ReadLine();
                        var db = new NorthwindContext();
                        if (choice == "2")
                        {

                            var list = db.Products.OrderBy(p => p.ProductName);
                            foreach (Product p in list)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"Name: {p.ProductName} Discontinued: {p.Discontinued}");
                                }
                            }
                        }
                        else if (choice == "3")
                        {
                            var list = db.Products.OrderBy(p => p.ProductName);
                            foreach (Product p in list)
                            {
                                if (p.Discontinued == true)
                                {
                                    Console.WriteLine($"Name: {p.ProductName} Discontinued: {p.Discontinued}");
                                }
                            }
                        }
                        else
                        {
                            var list = db.Products.OrderBy(p => p.ProductName);
                            Console.WriteLine($"{list.Count()} records returned");
                            foreach (var item in list)
                            {

                                Console.WriteLine($"Name: {item.ProductName} Discontinued: {item.Discontinued}");

                            }
                        }
                    }
                    else if (choice == "4")
                    {

                        var db = new NorthwindContext();
                        var p = GetProducts(db);

                        if (p != null)
                        { 
                                Console.WriteLine($"Product name: {p.ProductName} ID: {p.ProductID} UnitPrice: {p.UnitPrice} UnitsInStock: {p.UnitsInStock}");
                            
                        }
                       

                    }
                    //End of C part of Project


                    else if (choice == "5")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "6")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();

                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {

                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddCategory(category);

                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "7")
                    {
                        var db = new NorthwindContext();
                        var p = GetCategories(db);
                        if (p != null)
                        {
                            Category UpdatedCategory = InputCategory(db);
                            if (UpdatedCategory != null)
                            {
                                UpdatedCategory.CategoryId = p.CategoryId;
                                db.EditCategory(UpdatedCategory);
                                logger.Info("Product (id: {Category id}) updated", UpdatedCategory.CategoryId);
                            }
                        }
                        p.CategoryName = Console.ReadLine();
                    }
                    else if (choice == "8")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            if (p.Discontinued == false)
                            {
                                Console.WriteLine(p.ProductName);
                            }
                        }
                    }
                    else if (choice == "9")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"\t{p.ProductName}");
                                }
                            }
                        }
                    }
                    Console.WriteLine();

                } while (choice.ToLower() != "q");

            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }


        public static Product InputProduct(NorthwindContext db)
        {
            Product product = new Product();
            
            Console.WriteLine("Enter the Product Name");
            product.ProductName = Console.ReadLine();

            Console.WriteLine("Is the product disconinued? 1 = no, anything else is yes");
            String choice = Console.ReadLine();
            if (choice == "1")
            {
                product.Discontinued = false;
            }
            else
            {
                product.Discontinued = true;
            }

            Console.WriteLine("What is the UnitPrice?");
            product.UnitPrice = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("What is the quantity per unit?");
            product.QuantityPerUnit = Console.ReadLine();

            Console.WriteLine("What is the units in stock?");
            product.UnitsInStock = Convert.ToInt16(Console.ReadLine());

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                return product;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }

        public static Product GetProducts(NorthwindContext db)
        {

            var list = db.Products.OrderBy(p => p.ProductName);
            foreach (Product p in list)
            {
               // Console.WriteLine(p.ProductName, ", ", p.ProductID);
                if (p.ProductName.Count() == 0)
                {
                    Console.WriteLine($"  <no posts>");
                }
                else
                {

                    Console.WriteLine($"{p.ProductName}, {p.ProductID}");

                }
            }

            Console.WriteLine("Enter the ID of the product you want to edit");

            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductID == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product ID");
            return null;
        }


        public static Category InputCategory(NorthwindContext db)
        {
            Category category = new Category();
            Console.WriteLine("Enter the Product title");
            category.CategoryName = Console.ReadLine();

            Console.WriteLine("Enter the new Description");
            category.Description = Console.ReadLine();


            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                return category;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }


        public static Category GetCategories(NorthwindContext db)
        {

            var list = db.Categories.OrderBy(c => c.CategoryName);
            foreach (Category c in list)
            {
                Console.WriteLine(c.CategoryName);
                if (c.CategoryName.Count() == 0)
                {
                    Console.WriteLine($"  <no posts>");
                }
                else
                {

                    Console.WriteLine($"{c.CategoryName}, {c.CategoryId}");

                }
            }
            Console.WriteLine("Enter the ID of the Category");
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Product ID");
            return null;
        }



    }
}
   
