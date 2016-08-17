#pragma once

using namespace Platform;

namespace JSRTNative {

public value struct ChakraStringResult
{
	int ErrorCode;
	String^ Result;
};

};