Feature: Image Transformation
	Scenario: Request original image
		Given the original is a png image that is 640 x 480
		And we request no transformations
		When we make the transformation request
		Then the result width is 640
		And the result height is 480
		And the result format is png
		And the result is the original

	Scenario: Request resized image
		Given the original is a png image that is 640 x 480
		And we request max width is 64
		And we request max height is 64
		When we make the transformation request
		Then the result width is 64
		And the result height is 48
		
	Scenario: Request original image in different format
		Given the original is a png image that is 640 x 480
		And we request the format is jpg
		When we make the transformation request
		Then the result width is 640
		And the result height is 480
		And the result format is jpg
		
	Scenario: Request resized image in different format
		Given the original is a png image that is 640 x 480
		And we request the format is jpg
		And we request max width is 64
		And we request max height is 64
		When we make the transformation request
		Then the result width is 64
		And the result height is 48
		And the result format is jpg
		
	Scenario: Request original image with invisible watermark
		Given the original is a bmp image that is 100 x 100
		And we request the watermark is ' '
		When we make the transformation request
		Then the result width is 100
		And the result height is 100
		And the result format is bmp
		And the result looks the same as the original
		
	Scenario: Request original image with visible watermark
		Given the original is a bmp image that is 100 x 100
		And we request the watermark is '###'
		When we make the transformation request
		Then the result width is 100
		And the result height is 100
		And the result format is bmp
		And the result looks different to the original
		
	Scenario: Request original image with coloured background
		Given the original is a bmp image that is 100 x 100
		And we request the background colour is 'red'
		When we make the transformation request
		Then the result width is 100
		And the result height is 100
		And the result format is bmp
		And the result looks different to the original
		
