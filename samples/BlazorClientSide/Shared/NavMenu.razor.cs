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

namespace BlazorClientSide.Shared
{
    public partial class NavMenu
    {
        bool collapseNavMenu = true;
        string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}