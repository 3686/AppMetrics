using System.Linq;

// ReSharper disable CheckNamespace
namespace Microsoft.AspNet.Routing.Template
// ReSharper restore CheckNamespace
{
    internal static class TemplateExtensions
    {
        public static string ToTemplatePartString(this TemplatePart templatePart)
        {
            if (templatePart.IsParameter)
            {
                return "{" + (templatePart.IsCatchAll ? "*" : string.Empty) + templatePart.Name + (templatePart.IsOptional ? "?" : string.Empty) + "}";
            }

            return templatePart.Text;
        }

        public static string ToTemplateSegmentString(this TemplateSegment templateSegment) =>
            string.Join(string.Empty, templateSegment.Parts.Select(ToTemplatePartString));

        public static string ToTemplateString(this TemplateRoute templateRoute,
            string controller, string action) =>
                string.Join("/", templateRoute.ParsedTemplate.Segments
                    .Select(s => s.ToTemplateSegmentString()))
                    .Replace("{controller}", controller)
                    .Replace("{action}", action);
    }
}