namespace ZonaCounter;

/// <summary>
/// An individual product.
/// </summary>
class ZonaProduct(string name, int totalCount, float totalCost, float unitPrice = 0.99f) 
{
    public string Name { get; set; } = name;
    public int TotalCount { get; set; } = totalCount;
    public float TotalCost { get; set; } = totalCost;
    public float UnitPrice { get; set; } = unitPrice;
}


/// <summary>
/// All of the saved data about the products.
/// </summary>
class ZonaData(List<ZonaProduct> products)
{
    public List<ZonaProduct> Products = products ?? [];
    public string DefaultProduct { get; set; } = "Ginseng and Honey";
    
    public float SumCost() => Products.Sum((zona) => zona.TotalCost);
    public float SumCount() => Products.Sum((zona) => zona.TotalCount);
    
    /// <summary>Increment the product data.</summary>
    public ZonaProduct IncrementProduct(string name, int count, float cost = -1) 
    {
        ZonaProduct? product = Products.Where((p) => p.Name == name).FirstOrDefault();
        
        // It doesnt exist yet.
        if (product is null) 
        {
            ZonaProduct newProduct;
            if (cost == -1) 
                newProduct = new ZonaProduct(name, count, 0.99f * count);
            else 
                newProduct = new ZonaProduct(name, count, cost * count, cost);

            Products.Add(newProduct);
            return newProduct;
        }

        // -1 means use default unit price
        if (cost == -1)
            product.TotalCost += product.UnitPrice * count;
        else
            product.TotalCost += cost * count;

        product.TotalCount += count;
        return product;
    }
    
    /// <summary>Change unit price</summary>
    public void ChangePrice(string name, float price) 
    {
        ZonaProduct? product = Products.Where(p => p.Name == name).FirstOrDefault() ?? new ZonaProduct(name, 0, 0, price);
        product.UnitPrice = price;
    }
}