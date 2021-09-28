using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazored.TextEditor
{
    public partial class ToolbarContentComp
    {
        [Parameter] public List<string> Fonts { get; set; }
        [Parameter] public bool ShowFontControls { get; set; } = true;
        [Parameter] public bool ShowSizeControls { get; set; }
        [Parameter] public bool ShowStyleControls { get; set; }
        [Parameter] public bool ShowColorControls { get; set; }
        [Parameter] public bool ShowIndentationControls { get; set; }
        [Parameter] public bool ShowAlignmentControls { get; set; }
        [Parameter] public bool ShowListControls { get; set; }
        [Parameter] public bool ShowHypertextLinkControls { get; set; }


    }
}
