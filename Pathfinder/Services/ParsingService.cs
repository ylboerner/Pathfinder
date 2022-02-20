using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Serialization;

namespace Pathfinder.Services;

public interface IParsingService
{
    public (ITypedElement? typedElement, List<string> output) Parse(string fhirPathInput,
        IEnumerable<string> outputList);
}


public class ParsingService: IParsingService
{
    public (ITypedElement? typedElement, List<string> output) Parse(string input, IEnumerable<string> outputList)
    {
        ITypedElement? typedElement = null;
        var output = outputList.ToList();
        
        // Parse Input
        try
        {
            ISourceNode? node = null;

            if (input.IsJson())
            {
                node = FhirJsonNode.Parse(input);
            } else if (input.IsXml())
            {
                node = FhirXmlNode.Parse(input);
            }
            else
            {
                output.Add("Invalid input format");
            }
            
            if (node is not null)
#pragma warning disable CS0618
                typedElement = SourceNodeExtensions.ToTypedElement(node);
#pragma warning restore CS0618
        }
        catch (Exception e)
        {
            output.ToList().Add(e.Message);
        }

        return (typedElement, output);
    }
}

public static class StringExtensions
{
    public static bool IsJson(this string input){
        input = input.Trim();
        return input.StartsWith("{") && input.EndsWith("}") 
               || input.StartsWith("[") && input.EndsWith("]");
    }
    
    public static bool IsXml(this string input){
        input = input.Trim();
        return input.StartsWith("<");
    }
}