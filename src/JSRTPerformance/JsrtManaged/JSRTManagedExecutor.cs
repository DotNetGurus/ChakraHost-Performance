using ReactNative.Chakra;
using System;

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

            IntPtr str;
            UIntPtr strLen;
            Native.ThrowIfError(Native.JsStringToPointer(variableValue, out str, out strLen));
            return Marshal.PtrToStringUni(str, (int)strLen);
        }

        public void SetGlobalVariable(string variable, string value)
        {

        }
    }
}
