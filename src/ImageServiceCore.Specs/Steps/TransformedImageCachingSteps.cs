using FluentAssertions;
using ImageServiceCore.ImageServiceRequestConverter;
using ImageServiceCore.Interfaces;
using ImageServiceCore.Services;
using System;
using TechTalk.SpecFlow;

namespace ImageServiceCore.Specs.Steps
{
    [Binding]
    public class TransformedImageCachingSteps
    {
        private string converterOutputFilename;
        private string transformFilename;

        [Given(@"encoded string name is '(.*)'")]
        public void GivenImageNameIs(string converterOutputFilename)
        {
            this.converterOutputFilename = converterOutputFilename;
        }
        
        [When(@"we make the cache exists request")]
        public void WhenWeMakeTheCacheExistsRequest()
        {
            var fakeStorage = new FakeCacheStorage();
            var fakeRequestConverter = new FakeRequestConverter(converterOutputFilename);
            var service = new TransformedImageCache(fakeStorage, fakeRequestConverter);
            service.Exists(new());
            transformFilename = fakeStorage.NameParameter;
        }
        
        [Then(@"the storage will be checked for the existence of '(.*)'")]
        public void ThenTheStorageWillBeCheckedForTheExistenceOf(string filename)
        {
            transformFilename.Should().Be(filename);
        }

        class FakeRequestConverter : IEncodedStringImageTransformationRequestConverter
        {
            private readonly string outputEncodedString;

            public FakeRequestConverter(string outputEncodedString)
            {
                this.outputEncodedString = outputEncodedString;
            }
            public ImageTransformationRequest ConvertFrom(string source) => null;

            public string ConvertTo(ImageTransformationRequest source) => outputEncodedString;
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
            public string[] List()
            {
                return new string[0];
            }
        }
    }
}
