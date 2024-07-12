using System.Reflection;

using Microsoft.EntityFrameworkCore;

using P02_FootballBetting.Data;
using P02_FootballBetting.Data.Models;

[TestFixture]
public class Test_001
{
    [Test]
    public void ValidateFootballBettingContext()
    {
        //Get type of Context class. Will not compile if the class is not found!
        var context = typeof(FootballBettingContext);

        var properties = new Dictionary<string, string>
        {
            { "Teams", "Team" },
            { "Colors", "Color" },
            { "Towns", "Town"},
            { "Countries", "Country" },
            { "Players", "Player" },
            { "Positions", "Position"},
            { "PlayersStatistics", "PlayerStatistic"},
            { "Games", "Game"},
            { "Bets", "Bet"},
            { "Users", "User"}
        };

        foreach (var p in properties)
        {
            AssertDbSet(context, p.Key, p.Value);
        }
    }

    public static PropertyInfo GetPropertyByName(Type type, string propName)
    {
        var properties = type.GetProperties();

        var firstOrDefault = properties.FirstOrDefault(p => p.Name == propName);
        return firstOrDefault;
    }

    public static void AssertCollectionIsOfType(Type type, string propertyName, Type collectionType)
    {
        var ordersProperty = GetPropertyByName(type, propertyName);

        var errorMessage = string.Format($"{type.Name}.{propertyName} property not fount!");

        Assert.IsNotNull(ordersProperty, errorMessage);

        Assert.That(collectionType.IsAssignableFrom(ordersProperty.PropertyType));
    }

    public static void AssertDbSet(Type context, string dbSetName, string modelName)
    {
        var expectedDbSetType = GetDbSetType(modelName);

        AssertCollectionIsOfType(context, dbSetName, expectedDbSetType);
    }

    public static Type GetDbSetType(string modelName)
    {
        // Get assembly from a class that 100% should exist. Will not compile if the class is not found!
        var assembly = typeof(Team).Assembly;

        var modelType = GetModelType(assembly, modelName);

        var dbSetType = typeof(DbSet<>).MakeGenericType(modelType);
        return dbSetType;
    }

    public static Type GetModelType(Assembly assembly, string modelName)
    {
        var modelType = assembly.GetTypes()
            .Where(t => t.Name == modelName)
            .FirstOrDefault();

        Assert.IsNotNull(modelType, $"{modelName} model not found!");

        return modelType;
    }
}