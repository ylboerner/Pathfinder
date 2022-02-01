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

    public CompilationService()
    {
        var symbolTable = new SymbolTable();
        symbolTable.AddStandardFP();
        _compiler = new FhirPathCompiler(symbolTable);
    }

    public IEnumerable<string> Compile(string fhirPathInput, string resourceInput)
    {
        var outputList = new List<string>();

        // Compile fhirPath
        try
        {
            _compiledFhirPath = _compiler.Compile(fhirPathInput);
        }
        catch (Exception e)
        {
            outputList.Add(e.Message);
            return outputList;
        }

        ITypedElement typedElement;

        // Parse Input
        try
        {
            var node = FhirJsonNode.Parse(resourceInput);
            typedElement = node.ToTypedElement();
        }
        catch (Exception e)
        {
            outputList.Add(e.Message);
            return outputList;
        }

        // Compile FhirPath
        var compilationResults = _compiledFhirPath(typedElement, EvaluationContext.CreateDefault()).ToList();

        // Generate output
        if (compilationResults.Any())
        {
            foreach (var compilationResult in compilationResults)
                if (compilationResult.Value is not null)
                    switch (compilationResult.Value)
                    {
                        case string valueAsString:
                            outputList.Add(valueAsString);
                            break;
                        case bool valueAsBoolean:
                            outputList.Add(valueAsBoolean.ToString());
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
                        outputList.Add(resultAsJson);
                    }
                }
        }
        else
        {
            outputList.Add(PathIsEmptyMessage);
        }
       

        return outputList;
    }
}