using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Hl7.FhirPath.Expressions;

namespace Pathfinder.Services;

public interface ICompilationService
{
    public string Compile(string fhirPathInput, string resourceInput);
}
public class CompilationService: ICompilationService
{
    private readonly FhirPathCompiler _compiler;
    private CompiledExpression? _compiledFhirPath;

    public CompilationService()
    {
        var symbolTable = new SymbolTable();
        symbolTable.AddStandardFP();
        _compiler = new FhirPathCompiler(symbolTable);
    }
        
    public string Compile(string fhirPathInput, string resourceInput)
    {
        // Compile fhirPath
        try
        {
            _compiledFhirPath = _compiler.Compile(fhirPathInput);
        }
        catch (Exception e)
        {
            return e.Message;
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
            return e.Message;
        }
        
        // Compile FhirPath
        var compilationResults = _compiledFhirPath(typedElement, EvaluationContext.CreateDefault()).ToList();
        
        // Generate output
        var results = new List<string?>();
        foreach (var compilationResult in compilationResults)
        {
            if (compilationResult.Value != null)
            {
                results.Add(compilationResult.Value as string);
            }
            else
            {
                results.Add(compilationResult.ToJson());
            }
        }
        
        var output = String.Join(System.Environment.NewLine, results);

        return output;
    }
}