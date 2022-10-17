namespace EF.CodeFirst.Models;

public record ProductWithFeature(int Id, string CategoryName, string Name, decimal UnitPrice, KeyValuePair<string, string>? Feature);