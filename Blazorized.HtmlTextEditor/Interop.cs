using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorized.HtmlTextEditor;

public static class Interop
{    internal static ValueTask<object> ConfigureStickyToolbar(
        IJSRuntime jsRuntime, ElementReference toolbarElement)
    {
        return jsRuntime.InvokeAsync<object>(
            "window.QuillFunctions.configureStickyToolbar",
            toolbarElement);
    }

    internal static ValueTask<object> CreateQuill(
            IJSRuntime jsRuntime,
        ElementReference quillElement,
        ElementReference toolbar,
        bool readOnly,
        bool wrapImagesInFigures,
        string placeholder,
        string theme,
        string debugLevel,
        string scrollingContainerId,
        bool imageServerUploadEnabled,
        ImageServerUploadType imageServerUploadType,
        string imageServerUploadUrl,
        List<string>? customFonts = null)
    {
        return jsRuntime.InvokeAsync<object>(
            "window.QuillFunctions.createQuill",
            quillElement,
            toolbar,
            readOnly,
            wrapImagesInFigures,
            placeholder,
            theme,
            debugLevel,
            scrollingContainerId,
            imageServerUploadEnabled,
            imageServerUploadType.ToString(),
            imageServerUploadUrl,
            customFonts);
    }

    internal static ValueTask<object> EnableQuillEditor(
        IJSRuntime jsRuntime,
        ElementReference quillElement,
        bool mode)
    {
        return jsRuntime.InvokeAsync<object>(
            "window.QuillFunctions.enableQuillEditor",
            quillElement, mode);
    }

    internal static ValueTask<string> GetContent(
        IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return jsRuntime.InvokeAsync<string>(
            "window.QuillFunctions.getQuillContent",
            quillElement);
    }

    internal static ValueTask<string> GetHtml(
        IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return jsRuntime.InvokeAsync<string>(
            "window.QuillFunctions.getQuillHTML",
            quillElement);
    }

    internal static ValueTask<string> GetText(
                    IJSRuntime jsRuntime,
        ElementReference quillElement)
    {
        return jsRuntime.InvokeAsync<string>(
            "window.QuillFunctions.getQuillText",
            quillElement);
    }

    internal static ValueTask<object> InsertQuillHtml(IJSRuntime jsRuntime, ElementReference quillElement, string html)
    {
        return jsRuntime.InvokeAsync<object>("window.QuillFunctions.insertQuillHtml", quillElement, html);
    }

    internal static ValueTask<object> InsertQuillImage(
            IJSRuntime jsRuntime,
        ElementReference quillElement,
        string imageUrl)
    {
        return jsRuntime.InvokeAsync<object>(
            "window.QuillFunctions.insertQuillImage",
            quillElement, imageUrl);
    }

    internal static ValueTask<object> InsertQuillText(IJSRuntime jsRuntime, ElementReference quillElement, string text)
    {
        return jsRuntime.InvokeAsync<object>("window.QuillFunctions.insertQuillText", quillElement, text);
    }

    internal static ValueTask<object> LoadQuillContent(IJSRuntime jsRuntime, ElementReference quillElement, string content)
    {
        return jsRuntime.InvokeAsync<object>("window.QuillFunctions.loadQuillContent", quillElement, content);
    }

    internal static ValueTask<object> LoadQuillHtmlContent(IJSRuntime jsRuntime, ElementReference quillElement, string quillHtmlContent)
    {
        return jsRuntime.InvokeAsync<object>("window.QuillFunctions.loadQuillHTMLContent", quillElement, quillHtmlContent);
    }

    internal static ValueTask<object> SetQuillBlazorBridge(IJSRuntime jsRuntime, ElementReference quillElement, DotNetObjectReference<HtmlTextEditor> objRef,
        string editorTextSaveUrl, string editorStatusElementId)
    {
        return jsRuntime.InvokeAsync<object>("window.QuillFunctions.setQuillBlazorBridge",
            quillElement,
            objRef,
            editorTextSaveUrl,
            editorStatusElementId);
    }
}