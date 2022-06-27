using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence;

namespace Janus.Wrapper.CsvFiles.Tests;

public class WrapperCsvFilesTests
{
    [Fact(DisplayName = "Infer a schema in the CSV files example")]
    public void InferSchemaOnCsvFilesExample()
    {
        var expectedSchemaModel = SchemaModelBuilder.InitDataSource("DataSet")
            .AddSchema("CarsSchema", 
                conf => conf.AddTableau("cars", conf => conf.AddAttribute("Car", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(0))
                                                            .AddAttribute("MPG", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(1))
                                                            .AddAttribute("Cylinders", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(2))
                                                            .AddAttribute("Displacement", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(3))
                                                            .AddAttribute("Horsepower", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(4))
                                                            .AddAttribute("Weight", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(5))
                                                            .AddAttribute("Acceleration", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(6))
                                                            .AddAttribute("Model", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(7))
                                                            .AddAttribute("Origin", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(8))))
            .AddSchema("CerealSchema",
                conf => conf.AddTableau("cereal", conf => conf.AddAttribute("name", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(0))
                                                              .AddAttribute("mfr", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(1))
                                                              .AddAttribute("type", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(2))
                                                              .AddAttribute("calories", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(3))
                                                              .AddAttribute("protein", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(4))
                                                              .AddAttribute("fat", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(5))
                                                              .AddAttribute("sodium", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(6))
                                                              .AddAttribute("fiber", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(7))
                                                              .AddAttribute("carbo", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(8))
                                                              .AddAttribute("sugars", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(9))
                                                              .AddAttribute("potass", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(10))
                                                              .AddAttribute("vitamins", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(11))
                                                              .AddAttribute("shelf", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(12))
                                                              .AddAttribute("weight", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(13))
                                                              .AddAttribute("cups", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(14))
                                                              .AddAttribute("rating", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(15))))
            .AddSchema("MiscSchema", 
                conf => conf.AddTableau("factbook", conf => conf.AddAttribute("Country", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(0))
                                                                .AddAttribute("Area(sq km)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(1))
                                                                .AddAttribute("Birth rate(births/1000 population)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(2))
                                                                .AddAttribute("Current account balance", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(3))
                                                                .AddAttribute("Death rate(deaths/1000 population)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(4))
                                                                .AddAttribute("Debt - external", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(5))
                                                                .AddAttribute("Electricity - consumption(kWh)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(6))
                                                                .AddAttribute("Electricity - production(kWh)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(7))
                                                                .AddAttribute("Exports", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(8))
                                                                .AddAttribute("GDP", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(9))
                                                                .AddAttribute("GDP - per capita", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(10))
                                                                .AddAttribute("GDP - real growth rate(%)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(11))
                                                                .AddAttribute("HIV/AIDS - adult prevalence rate(%)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(12))
                                                                .AddAttribute("HIV/AIDS - deaths", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(13))
                                                                .AddAttribute("HIV/AIDS - people living with HIV/AIDS", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(14))
                                                                .AddAttribute("Highways(km)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(15))
                                                                .AddAttribute("Imports", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(16))
                                                                .AddAttribute("Industrial production growth rate(%)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(17))
                                                                .AddAttribute("Infant mortality rate(deaths/1000 live births)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(18))
                                                                .AddAttribute("Inflation rate (consumer prices)(%)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(19))
                                                                .AddAttribute("Internet hosts", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(20))
                                                                .AddAttribute("Internet users", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(21))
                                                                .AddAttribute("Investment (gross fixed)(% of GDP)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(22))
                                                                .AddAttribute("Labor force", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(23))
                                                                .AddAttribute("Life expectancy at birth(years)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(24))
                                                                .AddAttribute("Military expenditures - dollar figure", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(25))
                                                                .AddAttribute("Military expenditures - percent of GDP(%)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(26))
                                                                .AddAttribute("Natural gas - consumption(cu m)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(27))
                                                                .AddAttribute("Natural gas - exports(cu m)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(28))
                                                                .AddAttribute("Natural gas - imports(cu m)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(29))
                                                                .AddAttribute("Natural gas - production(cu m)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(30))
                                                                .AddAttribute("Natural gas - proved reserves(cu m)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(31))
                                                                .AddAttribute("Oil - consumption(bbl/day)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(32))
                                                                .AddAttribute("Oil - exports(bbl/day)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(33))
                                                                .AddAttribute("Oil - imports(bbl/day)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(34))
                                                                .AddAttribute("Oil - production(bbl/day)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(35))
                                                                .AddAttribute("Oil - proved reserves(bbl)", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(36))
                                                                .AddAttribute("Population", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(37))
                                                                .AddAttribute("Public debt(% of GDP)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(38))
                                                                .AddAttribute("Railways(km)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(39))
                                                                .AddAttribute("Reserves of foreign exchange & gold", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(40))
                                                                .AddAttribute("Telephones - main lines in use", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(41))
                                                                .AddAttribute("Telephones - mobile cellular", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(42))
                                                                .AddAttribute("Total fertility rate(children born/woman)", conf => conf.WithDataType(DataTypes.DECIMAL).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(43))
                                                                .AddAttribute("Unemployment rate(%)", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(44)))
                            .AddTableau("film", conf => conf.AddAttribute("Year", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(0))
                                                            .AddAttribute("Length", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(1))
                                                            .AddAttribute("Title", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(2))
                                                            .AddAttribute("Subject", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(3))
                                                            .AddAttribute("Actor", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(4))
                                                            .AddAttribute("Actress", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(5))
                                                            .AddAttribute("Director", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(6))
                                                            .AddAttribute("Popularity", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(7))
                                                            .AddAttribute("Awards", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(8))
                                                            .AddAttribute("*Image", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(9)))
                            .AddTableau("smallwikipedia", conf => conf.AddAttribute("User", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(0))
                                                                      .AddAttribute("Name", conf => conf.WithDataType(DataTypes.STRING).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(1))
                                                                      .AddAttribute("Date", conf => conf.WithDataType(DataTypes.DATETIME).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(2))
                                                                      .AddAttribute("changes", conf => conf.WithDataType(DataTypes.INT).WithIsPrimaryKey(false).WithIsNullable(true).WithOrdinal(3))))
            .Build();

        var schemaModelProvider = new CsvFilesProvider("./DataSet", ';');
        var schemaInferrer = new SchemaInferrer(schemaModelProvider);

        var result = schemaInferrer.InferSchemaModel();
        var str = result.Data?.ToString();
        Assert.True(result);
        Assert.NotNull(result.Data);
        Assert.Equal(expectedSchemaModel, result.Data);

    }
}