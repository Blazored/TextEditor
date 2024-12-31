(function () {
    window.QuillFunctions = {
        createQuill: function (
            quillElement, toolBar, readOnly,
            placeholder, theme, formats, debugLevel, syntax) {

            Quill.register('modules/blotFormatter', QuillBlotFormatter.default);

            var options = {
                debug: debugLevel,
                modules: {
                    syntax: syntax,
                    toolbar: toolBar,
                    blotFormatter: {}
                },
                placeholder: placeholder,
                readOnly: readOnly,
                theme: theme
            };

            if (formats) {
                options.formats = formats;
            }

            return new Quill(quillElement, options);
        },
        getQuillContent: function (quill) {
            return JSON.stringify(quill.getContents());
        },
        getQuillText: function (quill) {
            return quill.getText();
        },
        getQuillHTML: function (quill) {
            return quill.root.innerHTML;
        },
        loadQuillContent: function (quill, quillContent) {
            content = JSON.parse(quillContent);
            return quill.setContents(content, 'api');
        },
        loadQuillHTMLContent: function (quill, quillHTMLContent) {
            return quill.root.innerHTML = quillHTMLContent;
        },
        enableQuillEditor: function (quill, mode) {
            quill.enable(mode);
        },
        insertQuillImage: function (quill, imageURL) {
            var Delta = Quill.import('delta');
            editorIndex = 0;

            if (quill.getSelection() !== null) {
                editorIndex = quill.getSelection().index;
            }

            return quill.updateContents(
                new Delta()
                    .retain(editorIndex)
                    .insert({ image: imageURL },
                        { alt: imageURL }));
        },
        insertQuillText: function (quill, text) {
            editorIndex = 0;
            selectionLength = 0;

            if (quill.getSelection() !== null) {
                selection = quill.getSelection();
                editorIndex = selection.index;
                selectionLength = selection.length;
            }

            return quill.deleteText(editorIndex, selectionLength)
                .concat(quill.insertText(editorIndex, text));
        }
    };
})();