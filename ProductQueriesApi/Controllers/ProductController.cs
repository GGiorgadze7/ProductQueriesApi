using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProductQueriesApi.DB;
using ProductQueriesApi.Entities;

namespace ProductQueriesApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly StoreDbContext _db;

    public ProductController()
    {
        _db = new StoreDbContext();
    }

    //ამოცანა 1.1. Where და OrderBy იპოვეთ პროდუქტები, რომელთა ფასი 600-ზე მეტია.დაალაგეთ ფასის მიხედვით კლებადობით.

    [HttpGet("ProductMoreThan600")]
    public ICollection<Product> GetProducts()
    {
        var products = _db.Products
            .Where(p => p.Price > 600)
            .OrderByDescending(p => p.Price)
            .ToList();
        return products;

    }

    //ამოცანა 1.2. Select წინა ამოცანის შედეგიდან დააბრუნეთ მხოლოდ Name და Price(ანონიმური ობიექტი).

    [HttpGet("Name-Price Product")]
    public IActionResult GetProductsWithName()
    {
        var products = _db.Products
            .Where(p => p.Price > 600)
            .OrderByDescending(p => p.Price)
            .Select(p => new
            {
                p.Name,
                p.Price
            })
            .ToList();
        return Ok(products);

    }


    //ამოცანა 1.3. Joinდაწერეთ Join, რომელიც დააბრუნებს პროდუქტის სახელს და კატეგორიის სახელს. რამდენ ჩანაწერს მიიღებთ და რატომ? 

    [HttpGet("ProductNameAndCategories")]
    public IActionResult GetProductNameAndCategorie()
    {
        var result = _db.Products
            .Join(_db.Categories,
            p => p.CategoryId,
            c => c.Id,
            (p, c) => new
            {
                ProductName = p.Name,
                CategoryName = c.Name
            })
            .ToList();
        return Ok(result);

    }  //მივიღეთ ოთხი ჩანაწერი რადგან Cable ს არ აქვს კატეგორია


    //ამოცანა 1.4. GroupBy + Count გამოთვალეთ პროდუქტების რაოდენობა თითო კატეგორიაში(CategoryId-ისმიხედვით).

    [HttpGet("CategoryCount")]
    public IActionResult GeGetProductCountByCategoryt()
    {
        var result = _db.Products
            .GroupBy(p => p.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                ProductCount = g.Count()
            })
            .ToList();

        return Ok(result);

    }

    //ამოცანა 1.5. აგრეგაცია გამოთვალეთ საშუალო ფასი და ჯამური ფასი თითო კატეგორიისთვის.ასევე იპოვე ყველაზე ძვირი პროდუქტის ფასი ყველა პროდუქტში.


    [HttpGet("AvaragePrice")]
    public IActionResult GetAvaragePrice()
    {
        var categoryPrice = _db.Products
            .GroupBy(p => p.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                AvaragePrice = g.Average(p => p.Price),
                TotalPrice = g.Sum(p => p.Price)
            })
            .ToList();

        var MaxPrice = _db.Products.Max(p => p.Price);
            

        return Ok(categoryPrice);

    }
    //ამოცანა 1.6. Left Join(GroupJoin) დაბეჭდეთ ყველა კატეგორია და მათი პროდუქტების რაოდენობა, მათ შორის კატეგორია, რომელსაც პროდუქტი არ აქვს(Tablets).

    [HttpGet("categoriesProductCount")]
    public IActionResult GetCategoriesProductCount()
    {
        var result = _db.Categories
            .GroupJoin(
            _db.Products,
            c => c.Id,
            p => p.CategoryId,
            (c, products) => new
            {
                CategoryName = c.Name,
                ProductCount = products.Count()
            })
            .ToList();
        return Ok(result);
    }

    //ამოცანა 2.1. Include ჩატვირთეთ ყველა კატეგორია თავიანთი პროდუქტებით ერთ მოთხოვნაში და დაბეჭდეთ: „Laptops: Laptop A, Laptop B“ და ა.შ.
    [HttpGet("categoriesWithProducts")]
    public IActionResult GetCategoriesWithProducts()
    {
        var categories = _db.Categories
            .Include(c => c.Products)
            .AsSingleQuery()
            .ToList();

        var result = categories.Select(c =>
        $"{c.Name}: {string.Join(", ", c.Products.Select(p => p.Name))}"
    ).ToList();

        return Ok(result);
    }

    //ამოცანა 2.2. Filtered Include ჩატვირთეთ კატეგორიები, მაგრამ მხოლოდ იმ პროდუქტებით, რომელთა ფასი 600 - ზემეტია.რა მოხდება Tablets-თან?
    [HttpGet("categories-with-expensive-products")]
    public IActionResult GetCategoriesWithExpensiveProducts()
    {
        var categories = _db.Categories
            .Include(c => c.Products.Where(p => p.Price > 600))
            .ToList();

        var result = categories.Select(c =>
       $"{c.Name}: {string.Join(", ", c.Products.Select(p => p.Name))}"
   ).ToList();

        return Ok(result);
    }

    //Tablet ი დარჩება მაგრამ მისი სია ცარიელი იქნება


    //ამოცანა 3.1. FromSql (SELECT) Raw SQL-ით ამოიღეთ პროდუქტები, რომელთა ფასი მეტია ცვლადში შენახულ მნიშვნელობაზე(min = 600).გამოიყენეთ პარამეტრიზაცია.

    [HttpGet("GetProductWithRawSQL")]

    public IActionResult GetProductWithRawSQL()
    {
        var min = 600;

        var products = _db.Products
            .FromSql($"SELECT Id, Name, Price, CategoryId From Products Where Products.Price > {min}")
            .ToList();

        return Ok(products);
    }


    //ამოცანა 3.2. Raw SQL + LINQ წინა მოთხოვნას დაუმატეთ LINQ-ით OrderBy(p => p.Name).

    [HttpGet("GetProductWithRawSQL+OrderBy")]

    public IActionResult GetProductWithRawSQLOrderBy()
    {
        var min = 600;

        var products = _db.Products
            .FromSql($"SELECT Id, Name, Price, CategoryId From Products Where Products.Price > {min}")
            .OrderBy(p => p.Name)
            .ToList();

        return Ok(products);
    }

    //ამოცანა 3.3. ExecuteSql (UPDATE) გაზარდეთ პირველი კატეგორიის ყველა პროდუქტის ფასი 10 % -ით.რამდენი ჩანაწერი შეიცვალა?
    [HttpPatch("UpdateFirstCategoryPrice")]
    public IActionResult UpdateFirstCategoryPrice()
    {
        var result = _db.Database.ExecuteSql($"UPDATE Products Set Price = Price * 1.1 Where CategoryId = 1");

        return Ok(new {result});

    }


    //ამოცანა 3.4. უსაფრთხოება იპოვეთ შეცდომა კოდში და გაასწორეთ:

    //var name = Console.ReadLine();
    //var list = db.Products
    // .FromSql($"SELECT * FROM Products WHERE Name = {name}")
    // .ToList();



}
