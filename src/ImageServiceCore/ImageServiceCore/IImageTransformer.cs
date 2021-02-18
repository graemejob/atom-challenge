namespace ImageServiceCore.ImageServiceCore
{
    public interface IImageTransformer
    {
        /// <summary>
        /// Transform a given image file provided as a byte array, into a new image file as a byte array, given options in ImageTransformationRequest
        /// </summary>
        /// <param name="bytes">Source image file as byte array</param>
        /// <param name="request">Transformation options</param>
        /// <returns></returns>
        public byte[] Transform(byte[] bytes, ImageTransformationModel request);
    }
}
