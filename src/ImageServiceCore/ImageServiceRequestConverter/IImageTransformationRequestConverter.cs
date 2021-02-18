namespace ImageServiceCore.ImageServiceRequestConverter
{
    public class ImageTransformationRequest
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public string Colour { get; set; }
        public string Watermark { get; set; }
        public ImageTransformationRequest() { }
        public ImageTransformationRequest(string name, string format, int? maxWidth, int? maxHeight, string colour, string watermark)
        {
            Name = name;
            Format = format;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Colour = colour;
            Watermark = watermark;
        }
    }

    public interface IImageTransformationRequestConverter<T>
    {
        public ImageTransformationRequest ConvertFrom(T source);
        public T ConvertTo(ImageTransformationRequest source);
    }
}
