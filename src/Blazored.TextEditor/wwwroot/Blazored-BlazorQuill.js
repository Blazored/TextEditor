(function () {
    window.QuillFunctions = {        
        createQuill: function (
            quillElement, toolBar, readOnly,
            placeholder, theme, debugLevel) {  

            var options = {
                debug: debugLevel,
                modules: {
                    toolbar: toolBar
                },
                placeholder: placeholder,
                readOnly: readOnly,
                theme: theme
            };

            new Quill(quillElement, options);
        },
        getQuillContent: function(quillElement) {
            return JSON.stringify(quillElement.__quill.getContents());
        },
        getQuillText: function(quillElement) {
            return quillElement.__quill.getText();
        },
        getQuillHTML: function(quillElement) {
            return quillElement.__quill.root.innerHTML;
        },
        loadQuillContent: function(quillElement, quillContent) {
            content = JSON.parse(quillContent);
            return quillElement.__quill.setContents(content, 'api');
        },
        loadQuillHTMLContent: function (quillElement, quillHTMLContent) {
            return quillElement.__quill.root.innerHTML = quillHTMLContent;
        },
        enableQuillEditor: function (quillElement, mode) {
            quillElement.__quill.enable(mode);
        }
    };
})();