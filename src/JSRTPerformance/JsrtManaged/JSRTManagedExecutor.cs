using ReactNative.Chakra;
using System;
using System.Runtime.InteropServices;

namespace JSRTManaged
{
    public class JSRTManagedExecutor : IDisposable
    {
        private uint _currentSourceContext;
        private JavaScriptRuntime _runtime;
        private JavaScriptContext _context;
        private JavaScriptValue _global;

        public JSRTManagedExecutor()
        {
            Initialize();
            InitializeJSON();
        }

        private void Initialize()
        {
            Native.ThrowIfError(Native.JsCreateRuntime(JavaScriptRuntimeAttributes.None, null, out _runtime));
            Native.ThrowIfError(Native.JsCreateContext(_runtime, out _context));
            Native.ThrowIfError(Native.JsSetCurrentContext(_context));
            Native.ThrowIfError(Native.JsGetGlobalObject(out _global));
        }

        private JavaScriptValue _jsonParse;
        private JavaScriptValue _jsonStringify;

        private void InitializeJSON()
        {
            JavaScriptPropertyId jsonId;
            Native.ThrowIfError(Native.JsGetPropertyIdFromName("JSON", out jsonId));

            JavaScriptValue jsonObj;
            Native.ThrowIfError(Native.JsGetProperty(_global, jsonId, out jsonObj));

            JavaScriptPropertyId jsonParseId;
            Native.ThrowIfError(Native.JsGetPropertyIdFromName("parse", out jsonParseId));
            Native.ThrowIfError(Native.JsGetProperty(jsonObj, jsonParseId, out _jsonParse));

            JavaScriptPropertyId jsonStringifyId;
            Native.ThrowIfError(Native.JsGetPropertyIdFromName("stringify", out jsonStringifyId));
            Native.ThrowIfError(Native.JsGetProperty(jsonObj, jsonStringifyId, out _jsonStringify));
        }

        public void Dispose()
        {
            Native.ThrowIfError(Native.JsSetCurrentContext(JavaScriptContext.Invalid));
            Native.ThrowIfError(Native.JsDisposeRuntime(_runtime));
        }

        public string GetGlobalVariable(string variable)
        {
            JavaScriptPropertyId variableId;
            Native.ThrowIfError(Native.JsGetPropertyIdFromName(variable, out variableId));

            JavaScriptValue variableValue;
            Native.ThrowIfError(Native.JsGetProperty(_global, variableId, out variableValue));

            JavaScriptValue stringifiedValue;
            JavaScriptValue[] args = new [] { _global, variableValue };
            Native.ThrowIfError(Native.JsCallFunction(_jsonStringify, args, 2, out stringifiedValue));

            IntPtr str;
            UIntPtr strLen;
            Native.ThrowIfError(Native.JsStringToPointer(stringifiedValue, out str, out strLen));
            return Marshal.PtrToStringUni(str, (int)strLen);
        }

        public void SetGlobalVariable(string variable, string value)
        {
            JavaScriptPropertyId variableId;
            Native.ThrowIfError(Native.JsGetPropertyIdFromName(variable, out variableId));

            JavaScriptValue stringValue;
            Native.ThrowIfError(Native.JsPointerToString(value, (UIntPtr)value.Length, out stringValue));

            JavaScriptValue parsedValue;
            JavaScriptValue[] args = new[] { _global, stringValue };
            Native.ThrowIfError(Native.JsCallFunction(_jsonParse, args, 2, out parsedValue));

            Native.ThrowIfError(Native.JsSetProperty(_global, variableId, parsedValue, true));
        }
    }
}
