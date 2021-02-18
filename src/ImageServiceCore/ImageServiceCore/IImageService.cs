namespace ImageServiceCore.ImageServiceCore
{
    public interface IImageService
    {
        /// <summary>
        /// Get or transform image given the requested options
        /// </summary>
        /// <param name="request">Options to transform image</param>
        /// <returns>Byte array of image file</returns>
        public byte[] Get(ImageTransformationModel request);
    }
}
