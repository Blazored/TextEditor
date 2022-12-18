using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Blazorized.HtmlTextEditor.Interop;

namespace Blazorized.HtmlTextEditor;

public partial class HtmlTextEditor
{
    private MessageQueueProcessor _eventQueueProcessor = default!;

    private string _generatedToolBarId = ConstructToolbarId();

    private DotNetObjectReference<HtmlTextEditor>? _objRef;

    private ElementReference _quillElement;

    private ElementReference _toolBar;

    private bool disposedValue;

    [Inject]
    public IJSRuntime? JsRuntime { get; set; }

    [Parameter]
    public string DebugLevel { get; set; } = "error";

    [Parameter]
    public int DelayInMsBetweenStatusChanges { get; set; } = 2000;

    [Parameter]
    public RenderFragment EditorContent { get; set; } = default!;

    [Parameter]
    public string EditorStatusElementId { get; set; } = default!;

    [Parameter]
    public List<string> Fonts { get; set; } = default!;

    [Parameter]
    public string Id { get; set; } = "ql-editor-container";

    [Parameter]
    public bool ImageServerUploadEnabled { get; set; } = false;

    [Parameter]
    public Func<string, string, Stream, Task<string>>? ImageServerUploadMethod { get; set; }

    [Parameter]
    public ImageServerUploadType ImageServerUploadType { get; set; } = ImageServerUploadType.ApiPost;

    [Parameter]
    public string ImageServerUploadUrl { get; set; } = default!;

    [Parameter]
    public EventCallback OnTextChanged { get; set; }

    [Parameter]
    public EventCallback OnTextSaved { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Compose an epic...";

    [Parameter]
    public bool ReadOnly { get; set; } = false;

    [Parameter]
    public bool StickyToolBar { get; set; } = false;

    [Parameter]
    public string TextSavePostUrl { get; set; } = default!;

    [Parameter]
    public EditorTheme Theme { get; set; } = EditorTheme.Snow;

    [Parameter]
    public Toolbar Toolbar { get; set; } = default!;

    [Parameter]
    public RenderFragment ToolbarContent { get; set; } = default!;

    [Parameter]
    public bool WrapImagesInFigures { get; set; } = true;

#nullable enable
#nullable disable

    public async Task BundleAndSetQuillBlazorBridge()
    {
        _objRef = DotNetObjectReference.Create(this);
        if (JsRuntime != null)
        {
            //Wire up our bridge between quill and blazor in the javascript
            await Interop.SetQuillBlazorBridge(
                JsRuntime,
                _quillElement,
                _objRef,
                TextSavePostUrl,
                EditorStatusElementId);
        }
    }

    public async Task EnableEditor(bool mode)
    {
        await Interop.EnableQuillEditor(JsRuntime, _quillElement, mode);
    }

    [JSInvokable]
    public void EnqueueStatusMessage(string statusMessage)
    {
        _eventQueueProcessor.Enqueue(statusMessage);
    }

    [JSInvokable]
    public async Task FireTextChangedEvent()
    {
        await OnTextChanged.InvokeAsync();
    }

    [JSInvokable]
    public async Task FireTextSavedEvent()
    {
        await OnTextSaved.InvokeAsync();
    }

    public async Task<string> GetContent()
    {
        return await Interop.GetContent(JsRuntime, _quillElement);
    }

    public async Task<string> GetHtml()
    {
        return await Interop.GetHtml(JsRuntime, _quillElement);
    }

    public async Task<string> GetText()
    {
        return await Interop.GetText(JsRuntime, _quillElement);
    }

    public async Task InsertImage(string imageUrl)
    {
        await Interop.InsertQuillImage(JsRuntime, _quillElement, imageUrl);
    }

    public async Task LoadContent(string content)
    {
        await Interop.LoadQuillContent(JsRuntime, _quillElement, content);
    }

    public async Task LoadHtmlContent(string quillHtmlContent)
    {
        await Interop.LoadQuillHtmlContent(JsRuntime, _quillElement, quillHtmlContent);
    }

    [JSInvokable]
    public async Task<string> SaveImage(string imageName, string fileType)
    {
        var dataReference = await JsRuntime.InvokeAsync<IJSStreamReference>("quillImageDataStream");
        await using var dataReferenceStream = await dataReference.OpenReadStreamAsync(maxAllowedSize: 10_000_000);

        return ImageServerUploadMethod == null ? ""
            : await ImageServerUploadMethod(imageName, fileType, dataReferenceStream);
    }

    public async Task SetTextPostUrlAsync(string textSavePostUrl)
    {
        TextSavePostUrl = textSavePostUrl;
        await JsRuntime.InvokeAsync<string>("window.QuillFunctions.setTextSavePostUrl", textSavePostUrl);
    }

    public async Task ShowStatusMessage(string message)
    {
        await JsRuntime.InvokeAsync<string>("window.QuillFunctions.ShowStatusMessage", message);
    }

    protected override async Task
        OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Interop.CreateQuill(
                JsRuntime,
                _quillElement,
                _toolBar,
                ReadOnly,
                WrapImagesInFigures,
                Placeholder,
                Theme.ToString().ToLower(),
                DebugLevel,
                Id,
                ImageServerUploadEnabled,
                ImageServerUploadType,
                ImageServerUploadUrl,
                Fonts);
        }

        await ScrollEventHandler();
        await BundleAndSetQuillBlazorBridge();

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _eventQueueProcessor = new MessageQueueProcessor(this);
        if (ImageServerUploadEnabled)
        {
            switch (ImageServerUploadType)
            {
                case ImageServerUploadType.ApiPost:
                    if (string.IsNullOrEmpty(ImageServerUploadUrl))
                    {
                        throw new ArgumentNullException($"You cannot set the ImageServerUploadEnabled with ImageServerUploadType of ImageServerUploadType.ApiPost without providing the ImageServerUploadUrl parameter.");
                    }
                    break;

                case ImageServerUploadType.BlazorMethod:
                    if (ImageServerUploadMethod == null)
                    {
                        throw new ArgumentNullException($"You cannot set the ImageServerUploadEnabled with ImageServerUploadType of ImageServerUploadType.BlazorMethod without providing an implementation of the ImageServerUploadMethod Function");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static string ConstructToolbarId()
    {
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var rando = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return $"toolbar-{rando}";
    }

    private string CalculateToolBarClass(Toolbar toolbar)
    {
        var retVal = "toolbar";

        if (Theme.Equals(EditorTheme.Bubble))
        {
            return "";
        }

        if (StickyToolBar)
        {
            retVal += " sticky";
        }

        return retVal;
    }

    private async Task ScrollEventHandler()
    {
        if (StickyToolBar)
        {
            await Interop.ConfigureStickyToolbar(JsRuntime, _toolBar);
        }
    }

    public async Task InsertText(string text)
    {
        var QuillDelta = await Interop.InsertQuillText(JsRuntime, _quillElement, text);
    }

    public async Task InsertHtml(string html)
    {
        var QuillDelta = await Interop.InsertQuillHtml(JsRuntime, _quillElement, html);
    }

    #region IDisposable Pattern

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !disposedValue)
        {
            if (JsRuntime != null)
            {
                JsRuntime.InvokeAsync<string>("window.QuillFunctions.unBindToQuillTextChangeEvent").ConfigureAwait(false);
            }
        }

        if (!disposedValue)
        {
            _objRef?.Dispose();
            _objRef = null;
        }

        disposedValue = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (JsRuntime != null)
        {
            await JsRuntime.InvokeAsync<string>("window.QuillFunctions.unBindToQuillTextChangeEvent");
        }

        Dispose(disposing: false);
    }

    #endregion IDisposable Pattern
}