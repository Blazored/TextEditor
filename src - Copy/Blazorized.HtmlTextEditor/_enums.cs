namespace Blazorized.HtmlTextEditor;

public enum ImageServerUploadType
{
    /// <summary>
    /// An API Post method, where you provide a url where you'd like
    /// Penman.Blazor.Quill to post the IFormFile image data.
    /// Particularly useful if you have an API for storing / retrieving images
    /// Choosing this value, you will need to provide the ImageServerUploadUrl on your TextEditor
    /// to which the data will be posted
    /// </summary>
    ApiPost,
    /// <summary>
    /// A specified Func<string, string, byte[], out string> ImageServerUploadMethod which
    /// you specify on the TextEditor parameters and implement in our own Blazor code.
    /// You will need to provide an implementation of the ImageServerUploadMethod on your TextEditor
    /// so the image data can be sent to your blazor page / component
    /// </summary>
    BlazorMethod
}
public enum EditorTheme
{
    Snow,
    Bubble
}