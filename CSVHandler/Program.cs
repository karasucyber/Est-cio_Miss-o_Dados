using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

// Classe que representa o registro de dados
public class DataRecord
{
    public required string InvoiceNo { get; set; }
    public required string StockCode { get; set; }
    public required string Description { get; set; }
    public int Quantity { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal UnitPrice { get; set; }
    public int CustomerID { get; set; }
    public required string Country { get; set; }
}

// Conversor personalizado para inteiros que trata valores vazios como 0
public class IntegerConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return int.TryParse(text, out var value) ? value : 0;
    }
}

// Mapeamento simplificado
public sealed class DataRecordMap : ClassMap<DataRecord>
{
    public DataRecordMap()
    {
        Map(m => m.InvoiceNo).Name("InvoiceNo");
        Map(m => m.StockCode).Name("StockCode");
        Map(m => m.Description).Name("Description");
        Map(m => m.Quantity).Name("Quantity").TypeConverter<IntegerConverter>();
        Map(m => m.InvoiceDate).Name("InvoiceDate");
        Map(m => m.UnitPrice).Name("UnitPrice");
        Map(m => m.CustomerID).Name("CustomerID").TypeConverter<IntegerConverter>();
        Map(m => m.Country).Name("Country");
    }
}

// Programa principal
public class Program
{
    public static void Main()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",", // Usar ',' como delimitador
            HasHeaderRecord = true
        };

        // Caminho relativo ao diretório do projeto
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Online-Retail.csv");

        try
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<DataRecordMap>();

                var records = csv.GetRecords<DataRecord>().ToList();
                var first10Records = records.Take(10).ToList();
                Console.WriteLine("Primeiras 10 linhas:");
                foreach (var record in first10Records)
                {
                    Console.WriteLine($"InvoiceNo: {record.InvoiceNo}, StockCode: {record.StockCode}, Description: {record.Description}, Quantity: {record.Quantity}, InvoiceDate: {record.InvoiceDate}, UnitPrice: {record.UnitPrice}, CustomerID: {record.CustomerID}, Country: {record.Country}");
                }

                var last10Records = records.Skip(Math.Max(0, records.Count - 10)).ToList();
                Console.WriteLine("\nÚltimas 10 linhas:");
                foreach (var record in last10Records)
                {
                    Console.WriteLine($"InvoiceNo: {record.InvoiceNo}, StockCode: {record.StockCode}, Description: {record.Description}, Quantity: {record.Quantity}, InvoiceDate: {record.InvoiceDate}, UnitPrice: {record.UnitPrice}, CustomerID: {record.CustomerID}, Country: {record.Country}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
        }
    }
}
