Feature: Transformed Image Caching
	Scenario: Check existence of cached image with width, height, format and watermark transformations
		Given image name is 'foo.jpg'
		And transform width is 64
		And transform height is 48
		And transform format is png
		And watermark is 'atom'
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!w64h48t61746F6D.png'
		
	Scenario: Check existence of cached image with width, height and format transformations
		Given image name is 'foo.jpg'
		And transform width is 64
		And transform height is 48
		And transform format is png
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!w64h48.png'
		
	Scenario: Check existence of cached image with width and height transformations
		Given image name is 'foo.jpg'
		And transform width is 64
		And transform height is 48
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!w64h48.jpg'
		
	Scenario: Check existence of cached image with just height transformation
		Given image name is 'foo.jpg'
		And transform height is 48
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!h48.jpg'
		
	Scenario: Check existence of cached image with just watermark transformation
		Given image name is 'foo.jpg'
		And watermark is 'atom'
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg!t61746F6D.jpg'

	Scenario: Check existence of cached image with no transformations
		Given image name is 'foo.jpg'
		When we make the cache exists request
		Then the storage will be checked for the existence of 'foo.jpg'


