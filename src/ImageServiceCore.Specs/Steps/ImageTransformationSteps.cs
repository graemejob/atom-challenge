using FluentAssertions;
using ImageServiceCore.ImageServiceCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TechTalk.SpecFlow;

namespace ImageServiceCore.Specs.Steps
{
    [Binding]
    public class ImageTransformationSteps: IDisposable
    {
        private readonly ScenarioContext scenarioContext;

        private byte[] originalImageBytes;
        private ImageTransformationModel request;
        private byte[] outputImageBytes;
        private Image outputImage;

        public ImageTransformationSteps(ScenarioContext scenarioContext)
        {
            request = new();
            this.scenarioContext = scenarioContext;
        }

        private ImageFormat ParseImageFormat(string format)
        {
            return format switch {
                "png" => ImageFormat.Png,
                "bmp" => ImageFormat.Bmp,
                "jpg" => ImageFormat.Jpeg,
                _ => throw new NotSupportedException($"Format requested '{format}' is not yet supported")
            };
        }

        public void Dispose()
        {
            if (outputImage != null) outputImage.Dispose();
        }

        // Givens

        [Given(@"the original is a (.*) image that is (.*) x (.*)")]
        public void GivenTheOriginalIsAFmtImageOfWxH(string format, int width, int height)
        {
            using (var image = new Bitmap(width, height))
            using (var ms = new MemoryStream()) 
            {
                image.Save(ms, ParseImageFormat(format));
                originalImageBytes = ms.ToArray();
            }
        }

        [Given(@"we request no transformations")]
        public void GivenWeRequestNoTransformations() => request = new();

        [Given(@"we request max width is (.*)")]
        public void GivenWeRequestMaxWidthIs(int maxWidth) => request.MaxWidth = maxWidth;

        [Given(@"we request max height is (.*)")]
        public void GivenWeRequestMaxHeightIs(int maxHeight) => request.MaxHeight = maxHeight;

        [Given(@"we request the format is (.*)")]
        public void GivenWeRequestAFormatOfPng(string format) => request.Format = format;

        [Given(@"we request the watermark is '(.*)'")]
        public void GivenWeRequestTheWatermarkIs(string watermark) => request.Watermark = watermark;

        [Given(@"we request the background colour is '(.*)'")]
        public void GivenWeRequestTheBackgroundColourIs(string colour) => request.BackgroundColour = colour;

        // Whens

        [When(@"we make the transformation request")]
        public void WhenWeMakeTheTransformationRequest()
        {
            var service = new BitmapImageTransformer(NullLogger<BitmapImageTransformer>.Instance);
            outputImageBytes = service.Transform(originalImageBytes, request);
            using (var ms = new MemoryStream(outputImageBytes))
            {
                outputImage = new Bitmap(ms);
            }
        }

        // Thens

        [Then(@"the result width is (.*)")]
        public void ThenTheResultWidthIs(int expectedWidth)
        {
            outputImage.Width.Should().Be(expectedWidth);
        }

        [Then(@"the result height is (.*)")]
        public void ThenTheResultHeightIs(int expectedHeight)
        {
            outputImage.Height.Should().Be(expectedHeight);
        }

        [Then(@"the result format is (.*)")]
        public void ThenTheResultFormatIs(string format)
        {
            var imageFormat = ParseImageFormat(format);
            outputImage.RawFormat.Should().Be(imageFormat);
        }

        [Then(@"the result looks different to the original")]
        public void ThenTheResultLooksDifferentToTheOriginal()
        {
            outputImageBytes.Should().NotEqual(originalImageBytes);
        }

        [Then(@"the result looks the same as the original")]
        public void ThenTheResultLooksTheSameAsTheOriginal()
        {
            outputImageBytes.Should().Equal(originalImageBytes);
        }

        [Then(@"the result is the original")]
        public void ThenTheResultIsTheOriginal()
        {
            outputImageBytes.Should().BeSameAs(originalImageBytes);
        }
    }
}
