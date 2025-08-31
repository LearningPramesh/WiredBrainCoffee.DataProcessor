namespace WiredBrainCoffee.DataProcessor.Parsing;
public class CsvLineParserTests
{
    [Fact]
    public void ShouldParseValidLine()
    {
        //Arrange
        string[] csvline = new[] { "Espresso;10/27/2022 8:01:16 AM" };
        //Act
        var machineDataItems = CsvLineParser.Parse(csvline);
        //Assert
        Assert.NotNull(machineDataItems);
        Assert.Single(machineDataItems);
        Assert.Equal("Espresso", machineDataItems[0].CoffeeType);
        Assert.Equal(new DateTime(2022, 10, 27, 8, 1, 16),
            machineDataItems[0].CreatedAt);
    }
    [Fact]
    public void ShouldSkipEmptyLines()
    {
        //Arrange
        string[] csvline = new[] { "", " " };
        //Act
        var machineDataItems = CsvLineParser.Parse(csvline);
        //Assert
        Assert.NotNull(machineDataItems);
        Assert.Empty(machineDataItems);

    }
    [Fact]
    public void ShouldThrowExceptionForInvalidLine()
    {
        var csvLine = "Cappuccino";
        //Arrange
        var csvlines = new[] { csvLine };
        //Act and assert
        var exception = Assert.Throws<Exception>(
             () => CsvLineParser.Parse(csvlines));

        Assert.Equal($"Invalid line: {csvLine}", exception.Message);
    }
    [Fact]
    public void ShouldThrowExceptionForInvalidData()
    {
        var csvLine = "Cappuccino;InvalidDate";
        //Arrange
        var csvlines = new[] { csvLine };
        //Act and assert
        var exception = Assert.Throws<Exception>(
             () => CsvLineParser.Parse(csvlines));

        Assert.Equal($"Invalid datetime: {csvLine}", exception.Message);
    }

    //Data Driven test 
    [InlineData("Cappuccino", "Invalid line")]
    [InlineData("Cappuccino;InvalidDate", "Invalid datetime")]
    [Theory]
    public void ShouldThrowExceptionForInvalidLineDataDriven(string csvLine, string expectedMessagePrefix)
    {

        //Arrange
        var csvlines = new[] { csvLine };
        //Act and assert
        var exception = Assert.Throws<Exception>(
             () => CsvLineParser.Parse(csvlines));

        Assert.Equal($"{expectedMessagePrefix}: {csvLine}", exception.Message);
    }
}
