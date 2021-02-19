# Atom Code Challenge

This challenge set out a goal to write a scalable solution for serving images with certain transformation requirements, and utilise caching to reduce the time required to serve a transformed image. In this repository I have a solution with a ProofOfConcept web application project in it. When you start it up, you are presented with a kind of transformation dashboard which lets you choose from a list of original images, and specify what transformation you want.

## Getting Started
Open the solution (\atom-challenge\src\Solution.sln) and set the ProofOfConcept as the startup project. Run the application. You will be presented with an ugly dashboard page with a message saying there are no files in the folder.
Git doesn't like empty folders, so the application creates them when it starts. You should now be able to navigate to the following folders:
   * \atom-challenge\blobstorage\images
   * \atom-challenge\blobstorage\cache

Place all the original images in \atom-challenge\blobstorage\images and refresh the dashboard. You should now see a list.
You can select an image in the list, and click Set as background to view it.
You can also set whatever transformation parameters you want, and the url will update automatically to reflect the change.


## Naming Convention
The naming convention for transformation requests are as follows:
/images/ is the default path. This can be changes in Startup.cs
The convention always starts with the original image's full name.
If the image is to be processed in some way, there will be a bang(!) on the end, to indicate that.
After the bang, the following parameters may be specified in any order, and they are all optional. They are seperated by the underscore character
* w<width>  Maximum Width, eg 'w100' ensures that the result image width will be 100 pixels or less
* h<height> Maximum Height, eg 'h100' ensures that the result image height will be 100 pixels or less
* b<Colour> Background Colour, eg 'bred' means 'red', hex is also allowed by using hash 'b#ef0' or 'b#aaee77'
* t<Text>   Watermark text, eg 'tAtom Supplies' will draw a faint caption in the bottom right corner of the image before resizing. Text can be as long as you want, and will wrap to take up the whole image if necessary.

Some examples:

* "/images/foo.png" will serve the original image and not cache it.
* "/images/foo.png!.jpg" will serve a jpeg of the original image and cache it.
* "/images/foo.png!w64.png" will serve the original image, resized so that it will fit in 64x* pixels, and will cache it.
* "/images/foo.png!w64_h64.jpg" will serve a jpeg of the original image, resized so that it will fit in 64x64 pixels, and will cache it.
* "/images/foo.png!h64_bRed_tAtom.jpg" will serve a jpeg of the original image, resized so that it will fit in *x64 pixels, with a red background and "Atom" written in the bottom-right corner, and will cache it.

## Scalability

The Proof of Concept ASP.NET Core application I have put together utilises filesystem storage for both original images, and cached images. This would allow the application to be spun up across multiple servers, so long as they have access to the filesystem storage via network, etc. I've kept data access seperate between originals and cache so that different technologies could be used, and also if serving images to different regions, you may want to have a seperate cache per region if the popular product imagry differs around the world, while still having a single store of original imagry. It would be easy to impliment a Azure Cloud Storage adapter for IBlobStorage and use that instead of file system storage, if one so desires.


## Caching

This solution utilises 3 levels of caching
1. Browser cache. Standard.
   So this way, a single browser session will only call out once for each image.
3. ASP.Net Core Caching Middleware
   A single server will only process a given image request once, and all other times will be replays of that response. (No need to query or fetch from blob storage)
5. Cached image blob storage
   A single image request will only be transformed once

![Architecture](https://github.com/graemejob/atom-challenge/blob/main/docs/Architecture%20Layers%20and%20Flow.png?raw=true)

## Optimising load performance

It might be desireable to offload the image processing onto another server to keep CPU usage down on the web servers, and that can be accomplished by creating either an API  endpoint or a RabbitMQ Remote Procedure Call, and calling to it from a new implementation of IImageTransformer, just responsible for image processing and host that on its own scalable host. That would reduce the number of web endpoints needed to keep up with traffic requesting cached images. I have not explored this for this challenge.


## Endnote

Would I recommend that a company build an image server like this to serve their images? No. Absolutely not. There are plenty of CDNs with capabilities far beyond anything that's worth maintaining in-house. Azure actually have image processing capability exactly as required in this challenge. Unfortunately, it seems that capability is only available in the China region, for some reason.
https://docs.azure.cn/en-us/cdn/cdn-image-processing


