Feature: Transformed Image Caching
	Scenario: Check existence of cached image with width, height, format and watermark transformations
		Given encoded string name is 'foo.jpg!w64h48t61746F6D.png'
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!w64h48t61746F6D.png'
		