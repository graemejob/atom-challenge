using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageServiceCore.ImageServiceRequestConverter
{
    /// <summary>
    /// Converts a string into a ImageTransformationRequest record
    /// Format: "filename"
    /// Format: "filename![w{maxWidth}]_[h{maxHeight}]_[b{backColour}]_[t{watermark}].{fileExt}"
    /// Example: "imageName.png!w64_h64_bFFF_tATOM Supplies.jpeg"
    /// </summary>
    public class EncodedStringImageTransformationRequestConverterV1 : IEncodedStringImageTransformationRequestConverter
    {
        public ImageTransformationRequest ConvertFrom(string encodedString)
        {
            var lastBang = encodedString.LastIndexOf('!');
            if (lastBang == -1) return NoTransfomration(encodedString);
            var lastDot = encodedString.LastIndexOf('.');

            var name = encodedString[..lastBang];
            var format = encodedString[(lastDot + 1)..];

            if (format.Equals(Path.GetExtension(name)[1..], System.StringComparison.OrdinalIgnoreCase)) format = null;
            var parametersPart = encodedString[(lastBang + 1)..lastDot];

            var parameters = string.IsNullOrEmpty(parametersPart) 
                ? new Dictionary<string, string>() 
                : parametersPart.Split('_').ToDictionary(k => k[..1], v => v[1..]);

            int? toNullableInt(string str) { int value; return int.TryParse(str, out value) ? value : null; }

            var maxWidth = toNullableInt(parameters.GetValueOrDefault("w"));
            var maxHeight = toNullableInt(parameters.GetValueOrDefault("h"));
            var colour = parameters.GetValueOrDefault("b");
            var watermark = parameters.GetValueOrDefault("t");

            return new(name, format, maxWidth, maxHeight, colour, watermark);
        }

        private ImageTransformationRequest NoTransfomration(string name)
        {
            return new(name, null, null, null, null, null);
        }
        public string ConvertTo(ImageTransformationRequest source)
        {
            var parameters = new[] {
                ("w", source.MaxWidth?.ToString()),
                ("h", source.MaxHeight?.ToString()),
                ("b", source.Colour),
                ("t", source.Watermark)
            }.Where(p => p.Item2 != null).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.Append(source.Name);
            if (parameters.Any())
            {
                sb.Append('!');
                sb.Append(string.Join('_', parameters.Select(s => s.Item1 + s.Item2)));
                sb.Append('.');
                sb.Append(source.Format ?? Path.GetExtension(source.Name)[1..]);
            }
            return sb.ToString();
        }

    }
}
