Feature: Image Service
	Scenario: Request original image
		Given the image exists in blob storage
		And the image name is 'foo'
		When requesting the image from Image Service
		Then blob storage is checked for image
		And cache storage is not checked for image
		And image is read from blob storage
		And image is not written to cache storage
		And image is not transformed

	Scenario: Request image that exists in cache
		Given the image exists in cache storage
		And the image name is 'foo'
		And the width is constrained to 64
		And the height is constrained to 64
		And the required format is png
		And the required watermark is 'atom'
		When requesting the image from Image Service
		Then cache storage is checked for image
		And blob storage is not checked for image
		And image is read from cache storage
		And image is not written to cache storage
		And image is not transformed
		
	Scenario: Request transformed image that does not exist in cache
		Given the image exists in blob storage
		And the image name is 'foo'
		And the width is constrained to 64
		And the height is constrained to 64
		And the required format is png
		And the required watermark is 'atom'
		When requesting the image from Image Service
		Then cache storage is checked for image
		And blob storage is checked for image
		And image is read from blob storage
		And image is transformed
		And image is written to cache storage
		
	Scenario: Request image that doesn't exist
		Given the image does not exist
		And the image name is 'foo'
		And the width is constrained to 64
		And the height is constrained to 64
		And the required format is png
		And the required watermark is 'atom'
		When requesting the image from Image Service
		Then cache storage is checked for image
		And blob storage is checked for image
		And image is not read from blob storage
		And image is not read from cache storage
		And image is not transformed
		And image is not written to cache storage
		

