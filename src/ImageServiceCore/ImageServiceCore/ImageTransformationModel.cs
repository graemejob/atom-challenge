using System.Collections.Generic;

namespace ImageServiceCore.ImageServiceCore
{
    public class ImageTransformationModel
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public string BackgroundColour { get; set; }
        public string Watermark { get; set; }
        public ImageTransformationModel() { }
        public ImageTransformationModel(string name, string format, int? maxWidth, int? maxHeight, string backgroundColour, string watermark)
        {
            Name = name;
            Format = format;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            BackgroundColour = backgroundColour;
            Watermark = watermark;
        }

        public bool IsOriginalImage =>
            string.IsNullOrEmpty(Format) &&
            !MaxWidth.HasValue &&
            !MaxHeight.HasValue &&
            string.IsNullOrWhiteSpace(Watermark) &&
            string.IsNullOrWhiteSpace(BackgroundColour);

        public override string ToString()
        {
            if (IsOriginalImage) return $"{Name}";

            IEnumerable<string> tp()
            {
                if (MaxWidth.HasValue) yield return $"w<={MaxWidth.Value}";
                if (MaxHeight.HasValue) yield return $"h<={MaxHeight.Value}";
                if (!string.IsNullOrEmpty(Format)) yield return $"fmt:{Format}";
                if (!string.IsNullOrWhiteSpace(BackgroundColour)) yield return $"bg:{BackgroundColour}";
                if (!string.IsNullOrWhiteSpace(Watermark)) yield return $"txt:{Watermark}";
            };

            return $"{Name} -> {string.Join(" ", tp())}";
        }
    }
}
