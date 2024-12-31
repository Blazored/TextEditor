# Syntax Highlighter Example

See [Syntax Highlighter Module](https://quilljs.com/docs/modules/syntax) from the _QuillJS_ docs for more information.

![Syntax Highlighter Example](images/SyntaxHighlighterExample.png "Syntax Highlighter Example")

To turn on _Syntax Highlighting_ in your **Blazored Text Editor** just pass the property `Syntax="true"` and add the necessary library files (css/js).

Then add the `ql-code-block` to your _Toolbar_.

```html
<BlazoredTextEditor Syntax="true">
    <ToolbarContent>
        ...
        <span class="ql-formats">
            <button class="ql-code-block"></button>
        </span>
    </ToolbarContent>
    <EditorContent>
        ...
    </EditorContent>
</BlazoredTextEditor>
```

## Blazor WASM

Add to the `index.html` both CSS and JS for a syntax highlighter, for these examples I've chosen [highlight.js](https://highlightjs.org/).

```html
<head>
    ...
    <!-- Include your favorite highlight.js stylesheet -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/atom-one-dark.min.css" rel="stylesheet">
    ...
</head>
```

Make sure to add the `js` before your Quill js library or you will get an error.

```html
<body>
    <!-- Include the highlight.js library -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js"></script>
    <!-- Quill library -->
</body>
```

## Blazor Server

Repeat the above but instead of the `index.html` you will want to add / update your `_Host.cshtml`.
