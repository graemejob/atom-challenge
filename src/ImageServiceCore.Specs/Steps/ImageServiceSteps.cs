using FluentAssertions;
using ImageServiceCore.ImageServiceRequestConverter;
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
            var fakeImageBlobStorage = new FakeImageBlobStorage(existsInBlobStorage);
            var fakeTransformedImageCache = new FakeTransformedImageCache(existsInCacheStorage);
            var fakeImageTransformer = new FakeImageTransformer();

            var service = new ImageService(fakeTransformedImageCache, fakeImageBlobStorage, fakeImageTransformer, NullLogger<ImageService>.Instance);

            service.Get(new(name, format, maxWidth, maxHeight, colour, watermark));
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
            private readonly bool exists;

            public FakeTransformedImageCache(bool exists)
            {
                this.exists = exists;
            }

            public bool ExistsCalled { get; set; } = false;
            public bool GetCalled { get; set; } = false;
            public bool SetCalled { get; set; } = false;
            public bool Exists(ImageTransformationRequest request)
            {
                ExistsCalled = true;
                return this.exists;
            }

            public byte[] Get(ImageTransformationRequest request)
            {
                GetCalled = true;
                return new byte[0];
            }

            public void Set(byte[] bytes, ImageTransformationRequest request)
            {
                SetCalled = true;
            }
        }

        class FakeImageBlobStorage : IImageBlobStorage
        {
            private readonly bool exists;

            public FakeImageBlobStorage(bool exists)
            {
                this.exists = exists;
            }

            public bool ExistsCalled { get; set; } = false;
            public bool GetCalled { get; set; } = false;

            public bool Exists(string name)
            {
                ExistsCalled = true;
                return exists;
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
            public byte[] Transform(byte[] bytes, ImageTransformationRequest request)
            {
                TransformCalled = true;
                return new byte[0];
            }
        }
    }
}
