using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using BlazorWasmApp;
using Blazorized.HtmlTextEditor;

namespace BlazorWasmApp.Pages
{
    public partial class Index
    {
        HtmlTextEditor? QuillHtml;
        HtmlTextEditor? QuillNative;
        HtmlTextEditor? QuillReadOnly;
        string QuillHTMLContent = string.Empty;
        string? QuillContent;
        readonly string QuillReadOnlyContent = @"<span><b>Read Only</b> <u>Content</u></span>";
        bool readOnly = true;
        public async void GetHtml()
        {
            QuillHTMLContent = await QuillHtml!.GetHtml();
            StateHasChanged();
        }

        public async void LoadContent()
        {
            QuillContent = @"<a href='http://BlazorHelpWebsite.com/'>" + "<img src='images/blazorized.png' /></a>";
            await QuillHtml!.LoadHtmlContent(QuillContent);
            StateHasChanged();
        }

        public async void GetContent()
        {
            QuillContent = await QuillNative!.GetContent();
            StateHasChanged();
        }

        public async void InsertHtml()
        {
            await QuillNative!.InsertHtml(QuillContent);
            StateHasChanged();
        }

        public async void InsertImage()
        {
            await QuillNative!.InsertImage("images/blazorized.png");
            StateHasChanged();
        }

        public async void InsertText()
        {
            await QuillNative!.InsertText("Some Text");
            StateHasChanged();
        }

        async Task ToggleQuillEditor()
        {
            readOnly = (readOnly) ? false : true;
            await QuillReadOnly!.EnableEditor(readOnly);
            StateHasChanged();
        }
    }
}