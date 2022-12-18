using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using BlazorClientSide;
using BlazorClientSide.Shared;
using Blazorized.HtmlTextEditor;

namespace BlazorClientSide.Pages;

public partial class Index
{
    HtmlTextEditor? QuillHtml;
    HtmlTextEditor? QuillNative;
    HtmlTextEditor? QuillReadOnly;
    string QuillHTMLContent = string.Empty;
    string? QuillContent;
    readonly string QuillReadOnlyContent = @"<span><b>Read Only</b> <u>Content</u></span>";
    bool mode = false;
    public async void GetHtml()
    {
        QuillHTMLContent = await QuillHtml!.GetHtml();
        StateHasChanged();
    }

    public async void SetHTML()
    {
        string QuillContent = @"<a href='http://BlazorHelpWebsite.com/'>" + "<img src='images/BlazorHelpWebsite.gif' /></a>";
        await QuillHtml!.LoadHtmlContent(QuillContent);
        StateHasChanged();
    }

    public async void GetContent()
    {
        QuillContent = await QuillNative!.GetContent();
        StateHasChanged();
    }

    public async void LoadContent()
    {
        await QuillNative!.LoadContent(QuillContent);
        StateHasChanged();
    }

    public async void InsertImage()
    {
        await QuillNative!.InsertImage("images/BlazorHelpWebsite.gif");
        StateHasChanged();
    }

    public async void InsertText()
    {
        await QuillNative!.InsertText("Some Text");
        StateHasChanged();
    }

    async Task ToggleQuillEditor()
    {
        mode = (mode) ? false : true;
        await QuillReadOnly!.EnableEditor(mode);
    }
}