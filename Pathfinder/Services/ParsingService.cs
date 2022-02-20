using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Serialization;

namespace Pathfinder.Services;

public interface IParsingService
{
    public ITypedElement? Parse(string fhirPathInput,
        List<string> output);
}

public class ParsingService: IParsingService
{
    public ITypedElement? Parse(string input, List<string> output)
    {
        ITypedElement? typedElement = null;
        ISourceNode? node = null;
        
        try
        {
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
        }
        catch (Exception e)
        {
            output.ToList().Add(e.Message);
        }
        
        if (node is not null)
#pragma warning disable CS0618
            typedElement = SourceNodeExtensions.ToTypedElement(node);
#pragma warning restore CS0618

        return (typedElement);
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