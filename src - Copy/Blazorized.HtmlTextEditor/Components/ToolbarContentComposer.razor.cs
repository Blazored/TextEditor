using Microsoft.AspNetCore.Components;
namespace Blazorized.HtmlTextEditor;

public partial class ToolbarContentComposer
{
    [CascadingParameter] public Toolbar Toolbar { get; set; } = default!;
    [CascadingParameter] public List<string> Fonts { get; set; } = new();
}