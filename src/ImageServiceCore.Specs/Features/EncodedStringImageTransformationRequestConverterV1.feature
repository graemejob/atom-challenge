Feature: Encoded String Image Transformation Request Converter V1

	Scenario: Converting from full request to encoded string
		Given Request.Name is 'foo.bar'
		And Request.Format is tub
		And Request.MaxSize.Width is 32
		And Request.MaxSize.Height is 64
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32_h64_bred_tatoms.tub'
		
	Scenario: Converting from full request without new format to encoded string
		Given Request.Name is 'foo.bar'
		And Request.MaxSize.Width is 32
		And Request.MaxSize.Height is 64
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32_h64_bred_tatoms.bar'

	Scenario: Converting from width, colour, watermark request to encoded string
		Given Request.Name is 'foo.bar'
		And Request.MaxSize.Width is 32
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32_bred_tatoms.bar'
		
	Scenario: Converting from width, colour request to encoded string
		Given Request.Name is 'foo.bar'
		And Request.MaxSize.Width is 32
		And Request.Colour is 'red'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32_bred.bar'

	
	Scenario: Converting from width request to encoded string
		Given Request.Name is 'foo.bar'
		And Request.MaxSize.Width is 32
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar!w32.bar'

	Scenario: Converting from empty request to encoded string
		Given Request.Name is 'foo.bar'
		When we convert from request to a string
		Then the resulting converted string is 'foo.bar'

	
	Scenario: Converting from encoded string to request
		Given the encoded string is 'foo.bar!w32_h64_bred_tatoms.tub'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is 'tub'
		And Request.MaxSize.Width is 32
		And Request.MaxSize.Height is 64
		And Request.Colour is 'red'
		And Request.Watermark is 'atoms'
		
	Scenario: Converting from encoded string containing name to request
		Given the encoded string is 'foo.bar'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is empty
		And Request.MaxSize.Width is empty
		And Request.MaxSize.Height is empty
		And Request.Colour is empty
		And Request.Watermark is empty
		
	Scenario: Converting from encoded string containing name, format to request
		Given the encoded string is 'foo.bar!.tub'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is 'tub'
		And Request.MaxSize.Width is empty
		And Request.MaxSize.Height is empty
		And Request.Colour is empty
		And Request.Watermark is empty
		
	Scenario: Converting from encoded string containing name, width, format to request
		Given the encoded string is 'foo.bar!w64.tub'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is 'tub'
		And Request.MaxSize.Width is 64
		And Request.MaxSize.Height is empty
		And Request.Colour is empty
		And Request.Watermark is empty
		
	Scenario: Converting from encoded string containing name, width, height, format to request
		Given the encoded string is 'foo.bar!w64_h32.tub'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is 'tub'
		And Request.MaxSize.Width is 64
		And Request.MaxSize.Height is 32
		And Request.Colour is empty
		And Request.Watermark is empty

	Scenario: Converting from encoded string containing name, colour to request
		Given the encoded string is 'foo.bar!bred.bar'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is empty
		And Request.MaxSize.Width is empty
		And Request.MaxSize.Height is empty
		And Request.Colour is 'red'
		And Request.Watermark is empty

	Scenario: Converting from encoded string containing name, colour, watermark to request
		Given the encoded string is 'foo.bar!bred_thello world.bar'
		When we convert from string to a request
		Then Request.Name is 'foo.bar'
		And Request.Format is empty
		And Request.MaxSize.Width is empty
		And Request.MaxSize.Height is empty
		And Request.Colour is 'red'
		And Request.Watermark is 'hello world'
