Feature: GET Endpoint that accepts image requests

Scenario: Client requests an image that doesn't exist
	When Client requests an image that doesn't exist
	Then Response code is 404

Scenario: Client requests an image that does exist, but not in the resolution requested
	When Client requests an image that does exist, but not in the resolution requested
	Then 
	
Extensions installed: Bridge to Kuberneties

	
	
DefaultImageResizer
ImageCache


ImageCache
LocalStorage

Cache: Get me image at this resolution
Fetch or Load image
Resize image
cache image


Single processes would probably suffice for the kind of loads stated in the challange, however, if loads singified that scaling is vital, then I would do the following:

* Move resize into its own scaling pool, taking commands from a rabbit queue, processing image, and saving to storage, and returning acknowledgement
* Cache can be isolated to each data center, as the requested images are likely to be different depending on region
* 