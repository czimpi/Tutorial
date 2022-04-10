using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Tutorial;

[MemoryDiagnoser]
public class Program
{
    public const string InputFile = "data.json";
    public const string OutputFile = "output.json";
    
    public static async Task Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);        
    }

    [IterationSetup]
    public void DeleteOutputFile()
    {
        File.Delete("output.json");
    }
    
    [Benchmark]
    public async Task TestVariant1()
    {
        await Variant1.DoWorkAsync(InputFile, OutputFile);
    }
    
    [Benchmark]
    public async Task TestVariant2()
    {
        await Variant2.DoWorkAsync(InputFile, OutputFile);
    }
    
    [Benchmark]
    public void TestVariant3()
    {
        Variant3.DoWork(InputFile, OutputFile);
    }
    
    [Benchmark]
    public void TestVariant4()
    {
        Variant4.DoWork(InputFile, OutputFile);
    }
    
    [Benchmark]
    public void TestVariant5()
    {
        Variant5.DoWork(InputFile, OutputFile);
    }
    
    [Benchmark]
    public void TestVariant6()
    {
        Variant6.DoWork(InputFile, OutputFile);
    }
}