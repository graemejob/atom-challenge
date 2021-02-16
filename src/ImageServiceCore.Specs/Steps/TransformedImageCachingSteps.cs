using FluentAssertions;
using ImageServiceCore.Interfaces;
using ImageServiceCore.Services;
using System;
using TechTalk.SpecFlow;

namespace ImageServiceCore.Specs.Steps
{
    [Binding]
    public class TransformedImageCachingSteps
    {
        private readonly ScenarioContext scenarioContext;
        private (string Name, (int? Width, int? Height) MaxSize, string Format, string Watermark) request;
        private string transformFilename;
        public TransformedImageCachingSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given(@"image name is '(.*)'")]
        public void GivenImageNameIs(string name)
        {
            request.Name = name;
        }
        
        [Given(@"transform width is (.*)")]
        public void GivenTransformWidthIs(int maxWidth)
        {
            request.MaxSize.Width = maxWidth;
        }
        
        [Given(@"transform height is (.*)")]
        public void GivenTransformHeightIs(int maxHeight)
        {
            request.MaxSize.Height = maxHeight;
        }
        
        [Given(@"transform format is (.*)")]
        public void GivenTransformFormatIs(string format)
        {
            request.Format = format;
        }
        
        [Given(@"watermark is '(.*)'")]
        public void GivenWatermarkIs(string watermark)
        {
            request.Watermark = watermark;
        }
        
        [When(@"we make the cache exists request")]
        public void WhenWeMakeTheCacheExistsRequest()
        {
            var fakeStorage = new FakeCacheStorage();
            var service = new TransformedImageCache(fakeStorage);
            service.Exists(request.Name, request.Format, request.MaxSize, request.Watermark);
            transformFilename = fakeStorage.NameParameter;
        }
        
        [Then(@"the storage will be checked for the existence of '(.*)'")]
        public void ThenTheStorageWillBeCheckedForTheExistenceOf(string filename)
        {
            transformFilename.Should().Be(filename);
        }


        class FakeCacheStorage : ICacheBlobStorage
        {
            public string NameParameter { get; private set; }
            public bool ExistsReturns { get; set; }
            public bool Exists(string name)
            {
                NameParameter = name;
                return ExistsReturns;
            }

            public byte[] Get(string name)
            {
                NameParameter = name;
                return null;
            }

            public void Set(string name, byte[] bytes)
            {
                NameParameter = name;
            }
        }
    }
}
