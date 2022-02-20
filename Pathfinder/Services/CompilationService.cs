using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Hl7.FhirPath.Expressions;
using static Pathfinder.Constants.ApplicationConstants;

namespace Pathfinder.Services;

public interface ICompilationService
{
    public IEnumerable<string> Compile(string fhirPathInput, string resourceInput);
}

public class CompilationService : ICompilationService
{
    private readonly FhirPathCompiler _compiler;
    private CompiledExpression? _compiledFhirPath;
    private readonly IParsingService _parsingService;

    public CompilationService(IParsingService parsingService)
    {
        _parsingService = parsingService;
        var symbolTable = new SymbolTable();
        symbolTable.AddStandardFP();
        _compiler = new FhirPathCompiler(symbolTable);
    }

    public IEnumerable<string> Compile(string fhirPathInput, string resourceInput)
    {
        var output = new List<string>();

        // Compile fhirPath
        try
        {
            _compiledFhirPath = _compiler.Compile(fhirPathInput);
        }
        catch (Exception e)
        {
            output.Add(e.Message);
            return output;
        }

        var typedElement = _parsingService.Parse(resourceInput, output);
        if (typedElement is null)
        {
            return output;
        }

        // Build expression
        var compilationResults = _compiledFhirPath(typedElement, EvaluationContext.CreateDefault()).ToList();

        AddCompilationResultsToOutput(compilationResults, output);

        return output;
    }

    private static void AddCompilationResultsToOutput(List<ITypedElement> compilationResults, List<string> output)
    {
        // Generate output
        if (compilationResults.Any())
        {
            foreach (var compilationResult in compilationResults)
                if (compilationResult.Value is not null)
                    switch (compilationResult.Value)
                    {
                        case string valueAsString:
                            output.Add(valueAsString);
                            break;
                        case bool valueAsBoolean:
                            output.Add(valueAsBoolean.ToString());
                            break;
                    }
                else
                {
                    var resultAsJson = compilationResult.ToJson(new FhirJsonSerializationSettings
                    {
                        Pretty = true,
                        IgnoreUnknownElements = true
                    });

                    if (resultAsJson is not null)
                    {
                        output.Add(resultAsJson);
                    }
                }
        }
        else
        {
            output.Add(PathIsEmptyMessage);
        }
    }
}