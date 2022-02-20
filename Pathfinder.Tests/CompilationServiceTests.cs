using System.Linq;
using FluentAssertions;
using Pathfinder.Services;
using Pathfinder.Tests.Resources;
using Xunit;

namespace Pathfinder.Tests;

public class CompilationServiceTests
{
    [Fact]
    public void TestSuccessfulSingleOutput()
    {
        var compilationService = new CompilationService(new ParsingService());
        
        var result = compilationService.Compile("Patient.name.where(use='usual').given.first()", TestConstants.PatientJson).ToList();

        result.Count.Should().Be(1);
        result.First().Should().Be("Jim");
    }
    
    [Fact]
    public void TestSuccessfulMultipleOutput()
    {
        var compilationService = new CompilationService(new ParsingService());

        var result = compilationService.Compile("Patient.name", TestConstants.PatientJson);

        result.Count().Should().Be(3);
    }
    
    [Fact]
    public void TestSuccessfulBooleanOutput()
    {
        var compilationService = new CompilationService(new ParsingService());

        var result = compilationService.Compile("Patient.name.where(use='usual').exists()", TestConstants.PatientJson).ToList();
        result.Count.Should().Be(1);
        result.Should().Contain("True");
    }
    
    [Fact]
    public void TestPathDoesNotExistOutput()
    {
        var compilationService = new CompilationService(new ParsingService());

        var result = compilationService.Compile("Patient.na", TestConstants.PatientJson).ToList();
        result.Count.Should().Be(1);
        result.Should().Contain(Constants.ApplicationConstants.PathIsEmptyMessage);
    }
}