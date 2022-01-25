using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Hl7.FhirPath.Expressions;

namespace Pathfinder.Services;

public class FhirPath
{
    private FhirPathCompiler _compiler;

    public FhirPath()
    {
        var symbolTable = new SymbolTable();
        symbolTable.AddStandardFP();
        _compiler = new FhirPathCompiler(symbolTable);
    }
        
    public bool Compile(string fhirPathInput, string resourceInput)
    {
        var compiledFhirPath = _compiler.Compile(fhirPathInput);
        
        
        if (compiledFhirPath == null)
        {
            
        }
        var node = FhirJsonNode.Parse(resourceInput);
        var typedElement = node.ToTypedElement();
        var result = compiledFhirPath(null, EvaluationContext.CreateDefault());
        return true;
    }
}