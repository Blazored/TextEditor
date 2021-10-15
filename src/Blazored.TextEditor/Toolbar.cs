using System;
using System.Collections.Generic;
using System.Text;

namespace Blazored.TextEditor
{
    public class Toolbar
    {
        private bool _showFullToolbar;

        public bool ShowFullToolbar
        {
            get { return _showFullToolbar; }
            set
            {
                _showFullToolbar = value;
                if (value == true)
                {
                    SetAllPropertiesToTrue();
                }
            }
        }

        public bool ShowFontControls { get; set; } = true;
        public bool ShowSizeControls { get; set; }
        public bool ShowStyleControls { get; set; }
        public bool ShowColorControls { get; set; }
        public bool ShowHeaderControls { get; set; }
        public bool ShowQuotationControls { get; set; }
        public bool ShowCodeBlockControls { get; set; }
        public bool ShowListControls { get; set; }
        public bool ShowIndentationControls { get; set; }
        public bool ShowAlignmentControls { get; set; }
        public bool ShowDirectionControls { get; set; }
        public bool ShowHypertextLinkControls { get; set; }
        public bool ShowInsertImageControls { get; set; }
        public bool ShowEmbedVideoControls { get; set; }
        public bool ShowMathControls { get; set; }
        public bool ShowCleanFormattingControls { get; set; }

        public void SetAllPropertiesToTrue()
        {
            ShowFontControls = true;
            ShowSizeControls = true;
            ShowStyleControls = true;
            ShowColorControls = true;
            ShowHeaderControls = true;
            ShowQuotationControls = true;
            ShowCodeBlockControls = true;
            ShowAlignmentControls = true;
            ShowListControls = true;
            ShowIndentationControls = true;
            ShowDirectionControls = true;
            ShowHypertextLinkControls = true;
            ShowInsertImageControls = true;
            ShowEmbedVideoControls = true;
            ShowMathControls = true;
            ShowCleanFormattingControls = true;
        }

    }
}
