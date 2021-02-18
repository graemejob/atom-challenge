Feature: Encoded String Image Transformation Request Converter V1

	Scenario: Converting from request to encoded string
		Given Request.Name is 'foo.bar'
		And Request.Format is tub
		And Request.MaxSize.Width is 32
		And Request.MaxSize.Height is 64
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32_h64_bred_tatoms.tub'

	
	Scenario: Converting from encoded string to request
		Given the encoded string is 'foo.bar!w32_h64_bred_tatoms.tub'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is tub
		And Request.MaxSize.Width is 32
		And Request.MaxSize.Height is 64
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
