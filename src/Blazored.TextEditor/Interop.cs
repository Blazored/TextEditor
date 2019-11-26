using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazored.TextEditor
{
    public static class Interop
    {
        internal static ValueTask<object> CreateQuill(
            IJSRuntime jsRuntime,
            ElementReference quillElement,
            ElementReference toolbar,
            bool readOnly,
            string placeholder,
            string theme,
            string debugLevel)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.createQuill", 
                quillElement, toolbar, readOnly, 
                placeholder, theme, debugLevel);
        }

        internal static ValueTask<string> GetText(
            IJSRuntime jsRuntime,
            ElementReference quillElement)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillText", 
                quillElement);
        }

        internal static ValueTask<string> GetHTML(
            IJSRuntime jsRuntime,
            ElementReference quillElement)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillHTML", 
                quillElement);
        }

        internal static ValueTask<string> GetContent(
            IJSRuntime jsRuntime,
            ElementReference quillElement)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillContent", 
                quillElement);
        }

        internal static ValueTask<object> LoadQuillContent(
            IJSRuntime jsRuntime,
            ElementReference quillElement,
            string Content)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.loadQuillContent", 
                quillElement, Content);
        }

        internal static ValueTask<object> EnableQuillEditor(
            IJSRuntime jsRuntime,
            ElementReference quillElement,
            bool mode)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.enableQuillEditor", quillElement, mode);
        }
    }
}