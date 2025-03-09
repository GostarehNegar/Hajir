using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using ExcelDna.Integration;
using Hajir.Crm.Excel.AddIn;
using static System.Net.WebRequestMethods;

public static class StringFunctions
{
    [ExcelFunction(Description = "Reverses a given string")]
    public static string ReverseString([ExcelArgument(Name = "Input", Description = "The string to reverse")] string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public static class DataseehtFunctions
{
    private static string ProductsJson;
    private static string baseUrl = "http://localhost:5000";
    private static Product[] DoGetProducts()
    {
        var json = GetProductsJson(null);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Product[]>(json);
    }
    private static Datasheet[] _datasheets;
    private static Datasheet[] _GetDatasheets()
    {
        if (_datasheets == null)
        {
            using (var client = new HttpClient() {BaseAddress= new Uri(baseUrl) })
            {
                var response = client.GetAsync("/api/datasheet/all")
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                var str = response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                _datasheets= Newtonsoft.Json.JsonConvert.DeserializeObject<Datasheet[]>(str);
            }
        }
        return _datasheets;

    }
    private static string GetProductsJson(string url, bool refersh = false)
    {

        url = string.IsNullOrWhiteSpace(url) ? "http://localhost:5000/api/datasheet/products" : url;
        if (string.IsNullOrWhiteSpace(ProductsJson) || refersh)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();

                ProductsJson = response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
        return ProductsJson;
    }
    [ExcelFunction(Description = "Reverses a given string")]
    public static object GetProduct(
        [ExcelArgument(Name = "Code")] string code)
    {
        try
        {
            return _GetDatasheets()
                .FirstOrDefault(x => x.ProductCode == code)
                .ProductName;
        }
        catch
        {
            return ExcelError.ExcelErrorNA;
        }
    }

    [ExcelFunction(Description = "Reverses a given string")]
    public static object GetProductCode(
        [ExcelArgument(Name = "index")] int idx)
    {
        try
        {
            return _GetDatasheets()[idx].ProductCode;
            return DoGetProducts()[idx].Id;
        }
        catch(Exception err)
        {
            return err.GetBaseException().Message;
            return ExcelError.ExcelErrorNA;
        }
    }
    [ExcelFunction(Description = "Reverses a given string")]
    public static object GetPropName(
        [ExcelArgument(Name = "index")] int idx)
    {
        try
        {
            return _GetDatasheets()[0].Properties[idx].Name;
            return DoGetProducts()[idx].Id;
        }
        catch
        {
            return ExcelError.ExcelErrorNA;
        }
    }
    [ExcelFunction(Description = "Reverses a given string")]
    public static object GetProp(
        [ExcelArgument(Name = "code")] string code, string propName)
    {
        try
        {
            var product = _GetDatasheets().FirstOrDefault(x=>x.ProductCode==code);
            return product.Properties.FirstOrDefault(x => x.Name == propName).Value;
        }
        catch
        {
            return ExcelError.ExcelErrorNA;
        }
    }
}