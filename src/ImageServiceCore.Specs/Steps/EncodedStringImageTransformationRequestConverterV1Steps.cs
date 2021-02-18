﻿using FluentAssertions;
using ImageServiceCore.ImageServiceCore;
using ImageServiceCore.ImageServiceRequestConverter;
using System;
using TechTalk.SpecFlow;

namespace ImageServiceCore.Specs.Steps
{
    [Binding]
    public class EncodedStringImageTransformationRequestConverterV1Steps
    {
        ImageTransformationModel inputRequest = new ImageTransformationModel();
        string outputEncodedString = null;

        string inputEncodedString = null;
        ImageTransformationModel outputRequest = new ImageTransformationModel();
        

        [Given(@"Request\.Name is '(.*)'")]
        public void GivenRequest_NameIs(string name) => inputRequest.Name = name;

        [Given(@"Request\.Format is (.*)")]
        public void GivenRequest_FormatIs(string format) => inputRequest.Format = format;

        [Given(@"Request\.MaxSize\.Width is (.*)")]
        public void GivenRequest_MaxSize_WidthIs(int maxWidth) => inputRequest.MaxWidth = maxWidth;
        
        [Given(@"Request\.MaxSize\.Height is (.*)")]
        public void GivenRequest_MaxSize_HeightIs(int maxHeight) => inputRequest.MaxHeight = maxHeight;

        [Given(@"Request\.Colour is '(.*)'")]
        public void GivenRequest_ColourIs(string colour) => inputRequest.BackgroundColour = colour;
        
        [Given(@"Request\.Watermark is '(.*)'")]
        public void GivenRequest_WatermarkIs(string watermark) => inputRequest.Watermark = watermark;
        
        [Given(@"the encoded string is '(.*)'")]
        public void GivenTheEncodedStringIs(string encodedString) => inputEncodedString = encodedString;
        
        [When(@"we convert from request to a string")]
        public void WhenWeConvertFromRequestToAString()
        {
            var service = new EncodedStringImageTransformationRequestConverterV1();
            outputEncodedString = service.ConvertTo(inputRequest);
        }
        
        [When(@"we convert from string to a request")]
        public void WhenWeConvertFromStringToARequest()
        {
            var service = new EncodedStringImageTransformationRequestConverterV1();
            outputRequest = service.ConvertFrom(inputEncodedString);
        }
        
        [Then(@"the resulting converted string is '(.*)'")]
        public void ThenTheResultingConvertedStringIs(string encodedString) => outputEncodedString.Should().Be(encodedString);

        [Then(@"Request\.Name is '(.*)'")] public void ThenRequest_NameIs(string name) => outputRequest.Name.Should().Be(name);
        [Then(@"Request\.Name is empty")] public void ThenRequest_NameIsEmpty() => outputRequest.Name.Should().BeNull();

        [Then(@"Request\.Format is '(.*)'")] public void ThenRequest_FormatIs(string format) => outputRequest.Format.Should().Be(format);
        [Then(@"Request\.Format is empty")] public void ThenRequest_FormatIsEmpty() => outputRequest.Format.Should().BeNull();

        [Then(@"Request\.MaxSize\.Width is (.*)")] public void ThenRequest_MaxSize_WidthIs(int maxWidth) => outputRequest.MaxWidth.Should().Be(maxWidth);
        [Then(@"Request\.MaxSize\.Width is empty")] public void ThenRequest_MaxSize_WidthIsEmpty() => outputRequest.MaxWidth.Should().BeNull();

        [Then(@"Request\.MaxSize\.Height is (.*)")] public void ThenRequest_MaxSize_HeightIs(int maxHeight) => outputRequest.MaxHeight.Should().Be(maxHeight);
        [Then(@"Request\.MaxSize\.Height is empty")] public void ThenRequest_MaxSize_HeightIsEmpty() => outputRequest.MaxHeight.Should().BeNull();

        [Then(@"Request\.Colour is '(.*)'")] public void ThenRequest_ColourIs(string colour) => outputRequest.BackgroundColour.Should().Be(colour);
        [Then(@"Request\.Colour is empty")] public void ThenRequest_ColourIsEmpty() => outputRequest.BackgroundColour.Should().BeNull();

        [Then(@"Request\.Watermark is '(.*)'")] public void ThenRequest_WatermarkIs(string watermark) => outputRequest.Watermark.Should().Be(watermark);
        [Then(@"Request\.Watermark is empty")] public void ThenRequest_WatermarkIsEmpty() => outputRequest.Watermark.Should().BeNull();
    }
}
