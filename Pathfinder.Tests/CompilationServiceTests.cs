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
        var fhirPath = new CompilationService();
        
        var result = fhirPath.Compile("Patient.name.where(use='usual').given.first()", TestConstants.Resource).ToList();

        result.Count.Should().Be(1);
        result.First().Should().Be("Jim");
    }
    
    [Fact]
    public void TestSuccessfulMultipleOutput()
    {
        var fhirPath = new CompilationService();

        var result = fhirPath.Compile("Patient.name", TestConstants.Resource);

        result.Count().Should().Be(3);
    }
    
    [Fact]
    public void TestSuccessfulBooleanOutput()
    {
        var fhirPath = new CompilationService();

        var result = fhirPath.Compile("Patient.name.where(use='usual').exists()", TestConstants.Resource).ToList();
        result.Count.Should().Be(1);
        result.Should().Contain("True");
    }
    
    [Fact]
    public void TestPathDoesNotExistOutput()
    {
        var fhirPath = new CompilationService();

        var result = fhirPath.Compile("Patient.na", TestConstants.Resource).ToList();
        result.Count.Should().Be(1);
        result.Should().Contain(Constants.ApplicationConstants.PathIsEmptyMessage);
    }
}