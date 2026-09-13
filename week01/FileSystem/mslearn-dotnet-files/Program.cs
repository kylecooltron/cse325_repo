
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;


var salesFiles = FindFiles("stores");

foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

GenerateSalesSummaryReport(salesFiles);


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        // The file name will contain the full path, so only check the end of it
        if (file.EndsWith("sales.json"))
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}



void GenerateSalesSummaryReport(IEnumerable<string> salesFiles)
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var storesDirectory = Path.Combine(currentDirectory, "stores");

    var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
    Directory.CreateDirectory(salesTotalDir);

    var outputFilePath = Path.Combine(salesTotalDir, "totals.txt");


    List<(double, string)> totalSales = [];

    foreach (var file in salesFiles)
    {
        var data = File.ReadAllText(file);
        var salesData = JsonConvert.DeserializeObject<SalesTotal>(data);
        totalSales.Add((salesData?.Total ?? throw new Exception("Sales data is null"), file));
    }


    File.WriteAllText(outputFilePath, "Sales Summary");
    File.AppendAllText(outputFilePath, $"{Environment.NewLine}----------------------------");
    File.AppendAllText(outputFilePath, $"{Environment.NewLine} Total Sales: {totalSales.Select(s => s.Item1).Sum():C}"); // I know there are better ways to do this
    File.AppendAllText(outputFilePath, $"{Environment.NewLine}{Environment.NewLine} Details:");

    totalSales.ForEach(sale => File.AppendAllText(outputFilePath, $"{Environment.NewLine}   {sale.Item2}: {sale.Item1:C}"));


    Console.WriteLine("Sales file written succesfully");
}


class SalesTotal
{
  public double Total { get; set; }
}
