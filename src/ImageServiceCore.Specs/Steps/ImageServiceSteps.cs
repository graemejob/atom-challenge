using FluentAssertions;
using ImageServiceCore.Interfaces;
using ImageServiceCore.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ImageServiceCore.Specs.Steps
{
    [Binding]
    public class ImageServiceSteps
    {
        // Inputs
        private string name;
        private int? maxWidth;
        private int? maxHeight;
        private string format;
        private string colour;
        private string watermark;
        private bool existsInBlobStorage;
        private bool existsInCacheStorage;

        // Outputs
        private bool cacheIsChecked;
        private bool imageReadFromCacheStorage;
        private bool imageWrittenToCacheStorage;
        private bool blobStorageIsChecked;
        private bool imageReadFromBlobStorage;
        private bool imageIsTransformed;

        private readonly ScenarioContext scenarioContext;

        public ImageServiceSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        // Givens
        [Given(@"the image does not exist")]
        public void GivenTheImageDoesNotExist() {
            existsInBlobStorage = false; 
            existsInCacheStorage = false;
        }

        [Given(@"the image exists in blob storage")]
        public void GivenTheImageExistsInBlobStorage() => existsInBlobStorage = true;

        [Given(@"the image exists in cache storage")]
        public void GivenTheImageExistsInCacheStorage() => existsInCacheStorage = true;

        [Given(@"the image name is '(.*)'")]
        public void GivenTheImageNameIs(string name) => this.name = name;
        
        [Given(@"the width is constrained to (.*)")]
        public void GivenWidthIsConstrainedTo(int maxWidth) => this.maxWidth = maxWidth;

        [Given(@"the height is constrained to (.*)")]
        public void GivenHeightIsConstrainedTo(int maxHeight) => this.maxHeight = maxHeight;

        [Given(@"the required format is (.*)")]
        public void GivenTheRequiredFormatIs(string format) => this.format = format;

        [Given(@"the required background colour is (.*)")]
        public void GivenTheRequiredBackgroundColourIs(string colour) => this.colour = colour;

        [Given(@"the required watermark is '(.*)'")]
        public void GivenTheRequiredWatermarkIs(string watermark) => this.watermark = watermark;

        // Whens

        [When(@"requesting the image from Image Service")]
        public void WhenRequestingTheImageFromImageService()
        {
            var fakeImageBlobStorage = new FakeImageBlobStorage(name, existsInBlobStorage);
            var fakeTransformedImageCache = new FakeTransformedImageCache(name, format, (maxWidth, maxHeight), colour, watermark, existsInCacheStorage);
            var fakeImageTransformer = new FakeImageTransformer();

            var service = new ImageService(fakeTransformedImageCache, fakeImageBlobStorage, fakeImageTransformer, NullLogger<ImageService>.Instance);

            service.Get(name, format, (maxWidth, maxHeight), colour, watermark);
            Thread.Sleep(10); // Wait for async task to finish
            cacheIsChecked = fakeTransformedImageCache.ExistsCalled;
            imageReadFromCacheStorage = fakeTransformedImageCache.GetCalled;
            imageWrittenToCacheStorage = fakeTransformedImageCache.SetCalled;
            blobStorageIsChecked = fakeImageBlobStorage.ExistsCalled;
            imageReadFromBlobStorage = fakeImageBlobStorage.GetCalled;
            imageIsTransformed = fakeImageTransformer.TransformCalled;
        }

        // Thens

        [Then(@"blob storage is checked for image")]
        public void ThenBlobStorageIsCheckedForImage() => blobStorageIsChecked.Should().BeTrue();

        [Then(@"blob storage is not checked for image")]
        public void ThenBlobStorageIsNotCheckedForImage() => blobStorageIsChecked.Should().BeFalse();

        [Then(@"cache storage is checked for image")]
        public void ThenCacheStorageIsCheckedForImage() => cacheIsChecked.Should().BeTrue();

        [Then(@"cache storage is not checked for image")]
        public void ThenCacheStorageIsNotCheckedForImage() => cacheIsChecked.Should().BeFalse();

        [Then(@"image is read from blob storage")]
        public void ThenImageIsReadFromBlobStorage() => imageReadFromBlobStorage.Should().BeTrue();

        [Then(@"image is not read from blob storage")]
        public void ThenImageIsNotReadFromBlobStorage() => imageReadFromBlobStorage.Should().BeFalse();

        [Then(@"image is read from cache storage")]
        public void ThenImageIsReadFromCacheStorage() => imageReadFromCacheStorage.Should().BeTrue();

        [Then(@"image is not read from cache storage")]
        public void ThenImageIsNotReadFromCacheStorage() => imageReadFromCacheStorage.Should().BeFalse();

        [Then(@"image is written to cache storage")]
        public void ThenImageIsWrittenToCacheStorage() => imageWrittenToCacheStorage.Should().BeTrue();

        [Then(@"image is not written to cache storage")]
        public void ThenImageIsNotWrittenToCacheStorage() => imageWrittenToCacheStorage.Should().BeFalse();

        [Then(@"image is transformed")]
        public void ThenImageIsTransformed() => imageIsTransformed.Should().BeTrue();

        [Then(@"image is not transformed")]
        public void ThenImageIsNotTransformed() => imageIsTransformed.Should().BeFalse();

        // Fakes

        class FakeTransformedImageCache : ITransformedImageCache
        {
            private readonly string name;
            private readonly string format;
            private readonly (int? Width, int? Height) maxSize;
            private readonly string watermark;
            private readonly string colour;
            private readonly bool exists;

            public FakeTransformedImageCache(string name, string format, (int? Width, int? Height) maxSize, string colour, string watermark, bool exists)
            {
                this.name = name;
                this.format = format;
                this.maxSize = maxSize;
                this.watermark = watermark;
                this.colour = colour;
                this.exists = exists;
            }

            public bool ExistsCalled { get; set; } = false;
            public bool GetCalled { get; set; } = false;
            public bool SetCalled { get; set; } = false;
            public bool Exists(string name, string format, (int? Width, int? Height) maxSize, string colour, string watermark)
            {
                ExistsCalled = true;
                return this.exists && this.name == name && this.format == format && this.maxSize == maxSize && this.colour == colour && this.watermark == watermark;
            }

            public byte[] Get(string name, string format, (int? Width, int? Height) maxSize, string colour, string watermark)
            {
                GetCalled = true;
                return new byte[0];
            }

            public void Set(byte[] bytes, string name, string format, (int? Width, int? Height) maxSize, string colour, string watermark)
            {
                SetCalled = true;
            }
        }

        class FakeImageBlobStorage : IImageBlobStorage
        {
            private readonly string name;
            private readonly bool exists;

            public FakeImageBlobStorage(string name, bool exists)
            {
                this.name = name;
                this.exists = exists;
            }

            public bool ExistsCalled { get; set; } = false;
            public bool GetCalled { get; set; } = false;

            public bool Exists(string name)
            {
                ExistsCalled = true;
                return exists && name == this.name;
            }

            public byte[] Get(string name)
            {
                GetCalled = true;
                return new byte[0];
            }

            public void Set(string name, byte[] bytes)
            {
            }

            public string[] List()
            {
                return new string[0];
            }
        }

        class FakeImageTransformer : IImageTransformer
        {
            public bool TransformCalled { get; set; } = false;
            public byte[] Transform(byte[] bytes, string format, (int? Width, int? Height) maxSize, string colour, string watermark)
            {
                TransformCalled = true;
                return new byte[0];
            }
        }
    }
}
