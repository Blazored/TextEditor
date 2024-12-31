using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace Blazored.TextEditor
{
    public static class Interop
    {
        internal static ValueTask<IJSObjectReference> CreateQuill(
            IJSRuntime jsRuntime,
            ElementReference quillElement,
            ElementReference toolbar,
            bool readOnly,
            string placeholder,
            string theme,
            string[] formats,
            string debugLevel)
        {
            return jsRuntime.InvokeAsync<IJSObjectReference>(
                "QuillFunctions.createQuill",
                quillElement, toolbar, readOnly,
                placeholder, theme, formats, debugLevel);
        }

        internal static ValueTask<string> GetText(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillText",
                cancellationToken,
                quillObject);
        }

        internal static ValueTask<string> GetHTML(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillHTML",
                cancellationToken,
                quillObject);
        }

        internal static ValueTask<string> GetContent(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<string>(
                "QuillFunctions.getQuillContent",
                cancellationToken,
                quillObject);
        }

        internal static ValueTask<object> LoadQuillContent(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            string Content,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.loadQuillContent",
                cancellationToken,
                quillObject, Content);
        }

        internal static ValueTask<object> LoadQuillHTMLContent(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            string quillHTMLContent,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.loadQuillHTMLContent",
                cancellationToken,
                quillObject, quillHTMLContent);
        }

        internal static ValueTask EnableQuillEditor(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            bool mode,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeVoidAsync(
                "QuillFunctions.enableQuillEditor",
                cancellationToken,
                quillObject, mode);
        }

        internal static ValueTask<object> InsertQuillImage(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            string imageURL,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.insertQuillImage",
                cancellationToken,
                quillObject, imageURL);
        }

        internal static ValueTask<object> InsertQuillText(
            IJSRuntime jsRuntime,
            IJSObjectReference quillObject,
            string text,
            CancellationToken cancellationToken = default)
        {
            return jsRuntime.InvokeAsync<object>(
                "QuillFunctions.insertQuillText",
                cancellationToken,
                quillObject, text);
        }
    }
}