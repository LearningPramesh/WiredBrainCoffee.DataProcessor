using WiredBrainCoffee.DataProcessor.Data;
using WiredBrainCoffee.DataProcessor.Model;

namespace WiredBrainCoffee.DataProcessor.Processing;
public class MachineDataProcessorTests : IDisposable
{
    private readonly FakeCoffeCountStore _coffeeCountStore;
    private readonly MachineDataProcessor _machineDataProcessor;

    public MachineDataProcessorTests()
    {
        _coffeeCountStore = new FakeCoffeCountStore();
        _machineDataProcessor = new MachineDataProcessor(_coffeeCountStore);
    }

    [Fact]
    public void ProcessItemsShouldSaveCountPerCoffeeType()
    {
        //Arrange

        var items = new[]
        {

            new MachineDataItem("Cappuccino", new DateTime(2025,08,30,13,15,8)),
            new MachineDataItem("Espresso", new DateTime(2025,08,30,13,10,8)),
            new MachineDataItem("Cappuccino", DateTime.Now),
        };
        //act
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);
        var item = _coffeeCountStore.SavedItems[0];
        Assert.Equal("Cappuccino", item.CoffeeType);
        Assert.Equal(2, item.Count);
        item = _coffeeCountStore.SavedItems[1];
        Assert.Equal("Espresso", item.CoffeeType);
        Assert.Equal(1, item.Count);

    }
    [Fact]
    public void ProcessItems_WhenCalled_IgnoreOldItems()
    {
        //Arrange

        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2025,08,30,13,15,8)),
             new MachineDataItem("Espresso", DateTime.Now),
            new MachineDataItem("Espresso", new DateTime(2025,08,30,13,10,8)),
            new MachineDataItem("Cappuccino", DateTime.Now),
            new MachineDataItem("Cappuccino", new DateTime(2025,08,30,16,15,8)),
        };
        //act
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);

        var item = _coffeeCountStore.SavedItems[0];
        Assert.Equal("Cappuccino", item.CoffeeType);
        Assert.Equal(2, item.Count);

        item = _coffeeCountStore.SavedItems[1];
        Assert.Equal("Espresso", item.CoffeeType);
        Assert.Equal(1, item.Count);

    }


    [Fact]
    public void ProcessItems_WhenCalled_ClearsPreviousCoffeeCount()
    {
        //Arrange

        var items = new[]
        {
            new MachineDataItem("Cappuccino", DateTime.Now),
        };
        // Act 
        _machineDataProcessor.ProcessItems(items);
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);
        foreach (var item in _coffeeCountStore.SavedItems)
        {
            Assert.Equal("Cappuccino", item.CoffeeType);
            Assert.Equal(1, item.Count);
        }

    }

    public void Dispose()
    {
    }

}
public class FakeCoffeCountStore : ICoffeeCountStore
{
    public List<CoffeeCountItem> SavedItems { get; } = new();
    public void Save(CoffeeCountItem item)
    {
        SavedItems.Add(item);
    }
}