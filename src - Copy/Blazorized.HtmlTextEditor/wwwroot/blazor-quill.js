(
    function () {
        const Module = Quill.import('core/module');
        const InlineBlot = Quill.import("blots/block");
        const BlockEmbed = Quill.import('blots/block/embed');
        const Embed = Quill.import("blots/embed");
        /**
         * The Loading Image acts as a placeholder when the user adds an image to Quill and
         * that image is to be uploaded either via the TextEditor's ImageServerUploadMethod or
         * via a POST url defined by the TextEditor's ImageServerUploadUrl.  While either is taking place
         * the Loading Image shows a phony baloney version of the imnage while the real one is being uploaded
         * and its web URL returned to Quill.
          **/
        class LoadingImage extends BlockEmbed {
            static create(src) {
                const node = super.create(src);
                if (src === true) return node;

                const image = document.createElement("img");
                image.setAttribute("src", src);
                node.appendChild(image);
                return node;
            }
            deleteAt(index, length) {
                super.deleteAt(index, length);
                this.cache = {};
            }
            static value(domNode) {
                const { src, custom } = domNode.dataset;
                return { src, custom };
            }
        }

        LoadingImage.blotName = "imageBlot";
        LoadingImage.className = "image-uploading";
        LoadingImage.tagName = "span";

        /**
         *
         *  XToken is a custom blot created for example on custom html elements
         *
         *
         **/
        class XTokenBlot extends Embed {
            static create(value) {
                let node = super.create();
                if (value) {
                    let key = value.attributes["key"];
                    if (key) {
                        let text = document.createTextNode(value.innerText);
                        node.appendChild(text)
                        node.setAttribute('key', key.value);
                        node.setAttribute('class', 'token-show');
                    }
                }

                return node;
            }

            static value(node) {
                return node;
            }
        }

        XTokenBlot.blotName = 'xtoken';
        XTokenBlot.tagName = 'x-token';
        Quill.register(XTokenBlot);

        /**
         * Image Uploader does the uploading when a user adds an image to Quill.
         * The Image Uploader ties into the Quill Functions in that the user can specify
         * at a TextEditor level whether to return the Image uploading to the calling Blazor page or component,
         * or to post the image to a URL.
         *
         * The Image Uploader listens for inbound images (either added or pasted in) and then fires the
         * appropriate logic to upload per the user's configuration.
         **/
        class ImageUploader {
            constructor(quill, options) {
                this.quill = quill;
                this.options = options;
                this.range = null;

                if (typeof this.options.upload !== "function")
                    console.warn(
                        "[Missing config] upload function that returns a promise is required"
                    );

                var toolbar = this.quill.getModule("toolbar");
                toolbar.addHandler("image", this.selectLocalImage.bind(this));

                this.handleDrop = this.handleDrop.bind(this);
                this.handlePaste = this.handlePaste.bind(this);

                this.quill.root.addEventListener("drop", this.handleDrop, false);
                this.quill.root.addEventListener("paste", this.handlePaste, false);
            }

            selectLocalImage() {
                this.range = this.quill.getSelection();
                this.fileHolder = document.createElement("input");
                this.fileHolder.setAttribute("type", "file");
                this.fileHolder.setAttribute("accept", "image/*");
                this.fileHolder.setAttribute("style", "visibility:hidden");

                this.fileHolder.onchange = this.fileChanged.bind(this);

                document.body.appendChild(this.fileHolder);

                this.fileHolder.click();

                window.requestAnimationFrame(() => {
                    document.body.removeChild(this.fileHolder);
                });
            }

            handleDrop(evt) {
                evt.stopPropagation();
                evt.preventDefault();
                if (
                    evt.dataTransfer &&
                    evt.dataTransfer.files &&
                    evt.dataTransfer.files.length
                ) {
                    if (document.caretRangeFromPoint) {
                        const selection = document.getSelection();
                        const range = document.caretRangeFromPoint(evt.clientX, evt.clientY);
                        if (selection && range) {
                            selection.setBaseAndExtent(
                                range.startContainer,
                                range.startOffset,
                                range.startContainer,
                                range.startOffset
                            );
                        }
                    } else {
                        const selection = document.getSelection();
                        const range = document.caretPositionFromPoint(evt.clientX, evt.clientY);
                        if (selection && range) {
                            selection.setBaseAndExtent(
                                range.offsetNode,
                                range.offset,
                                range.offsetNode,
                                range.offset
                            );
                        }
                    }

                    this.range = this.quill.getSelection();
                    let file = evt.dataTransfer.files[0];

                    setTimeout(() => {
                        this.range = this.quill.getSelection();
                        this.readAndUploadFile(file);
                    }, 0);
                }
            }

            handlePaste(evt) {
                let clipboard = evt.clipboardData || window.clipboardData;

                // IE 11 is .files other browsers are .items
                if (clipboard && (clipboard.items || clipboard.files)) {
                    let items = clipboard.items || clipboard.files;
                    const IMAGE_MIME_REGEX = /^image\/(jpe?g|gif|png|svg|webp)$/i;

                    for (let i = 0; i < items.length; i++) {
                        if (IMAGE_MIME_REGEX.test(items[i].type)) {
                            let file = items[i].getAsFile ? items[i].getAsFile() : items[i];

                            if (file) {
                                this.range = this.quill.getSelection();
                                evt.preventDefault();
                                setTimeout(() => {
                                    this.range = this.quill.getSelection();
                                    this.readAndUploadFile(file);
                                }, 0);
                            }
                        }
                    }
                }
            }

            readAndUploadFile(file) {
                let isUploadReject = false;

                const fileReader = new FileReader();

                fileReader.addEventListener(
                    "load",
                    () => {
                        if (!isUploadReject) {
                            let base64ImageSrc = fileReader.result;
                            this.insertBase64Image(base64ImageSrc);
                        }
                    },
                    false
                );

                if (file) {
                    fileReader.readAsDataURL(file);
                }

                this.options.upload(file).then(
                    (imageUrl) => {
                        this.insertToEditor(imageUrl);
                    },
                    (error) => {
                        isUploadReject = true;
                        this.removeBase64Image();
                        console.warn(error);
                    }
                );
            }

            fileChanged() {
                const file = this.fileHolder.files[0];
                this.readAndUploadFile(file);
            }

            insertBase64Image(url) {
                const range = this.range;
                this.quill.insertEmbed(
                    range.index,
                    LoadingImage.blotName,
                    `${url}`,
                    "user"
                );
            }

            insertToEditor(url) {
                const range = this.range;
                // Delete the placeholder image
                this.quill.deleteText(range.index, 3, "user");
                // Insert the server saved image
                this.quill.insertEmbed(range.index, "image", `${url}`, "user");

                range.index++;
                this.quill.setSelection(range, "user");
            }

            removeBase64Image() {
                const range = this.range;
                this.quill.deleteText(range.index, 3, "user");
            }
        }
        window.ImageUploader = ImageUploader;

        /**
         * The Figure Image Blot makes it possible to wrap an image in a <Figure> tag and to add a FigCaption with the
         * image.  This is particularly useful when dealing in the publishing world, as most images embedded in a book
         * require a figure and caption associated with any image.
         **/
        class FigureImageBlot extends BlockEmbed {
            static blotName = 'image';
            static tagName = ['figure', 'image'];

            static create(value) {
                let node = super.create();
                let img = window.document.createElement('img');
                let caption = window.document.createElement('figcaption');
                img.classList.add("ql-card-img");
                if (value.alt || value.caption) {
                    img.setAttribute('alt', value.alt || value.caption);
                }
                if (value.src || typeof value === 'string') {
                    img.setAttribute('src', value.src || value);
                }

                if (value.height) {
                    img.setAttribute('height', value.height);
                }

                if (value.width) {
                    img.setAttribute('width', value.width);
                }

                node.appendChild(img);
                node.appendChild(caption);

                if (value.caption) {
                    caption.innerHTML = value.caption;
                }
                node.className = 'ql-card-editable ql-card-figure';
                return node;
            }

            constructor(node) {
                super(node);
                node.__onSelect = () => {
                    if (!node.querySelector('input')) {
                        let caption = node.querySelector('figcaption');
                        let captionInput = window.document.createElement('input');
                        captionInput.placeholder = 'Type your caption...';
                        if (caption) {
                            captionInput.value = caption.innerText;
                            let imgTag = node.querySelector('img');
                            let imgWidth = imgTag.width;

                            let generatedWidth = ((captionInput.value.length + 1) * 8);
                            if (imgWidth && generatedWidth > imgWidth) {
                                generatedWidth = imgWidth;
                            }
                            captionInput.style.width = generatedWidth + 'px';
                            caption.innerHTML = '';
                            caption.appendChild(captionInput);
                        } else {
                            caption = window.document.createElement('figcaption');
                            caption.appendChild(captionInput);
                            node.appendChild(caption);
                        }
                        captionInput.addEventListener('blur', () => {
                            let value = captionInput.value;
                            if (!value || value === '') {
                                caption.remove();
                            } else {
                                captionInput.remove();
                                caption.innerText = value;
                            }
                        });
                        captionInput.focus();
                    }
                }
            }

            static value(node) {
                let altString = "";
                let img = node.querySelector('img');
                if (!img) return false;

                let figcaption = node.querySelector('figcaption');
                if (figcaption) {
                    altString = figcaption.innerText;
                } else {
                    altString = img.getAttribute('alt');
                }

                return {
                    height: img.getAttribute('height'),
                    width: img.getAttribute('width'),
                    alt: altString,
                    src: img.getAttribute('src'),
                    caption: altString
                };
            }
        }

        /**
         * The CardEditableMode is used when someone is editing their Quill document and they wish to edit the Figure caption
         * on their image.  This lets them edit the text of the caption inline with the image.
         **/
        class CardEditableModule extends Module {
            constructor(quill, options) {
                super(quill, options);
                let listener = (e) => {
                    if (!document.body.contains(quill.root)) {
                        return document.body.removeEventListener('click', listener);
                    }
                    let elm = e.target.closest('.ql-card-editable');
                    let deselectCard = () => {
                        if (elm.__onDeselect) {
                            elm.__onDeselect(quill);
                        } else {
                            quill.setSelection(quill.getIndex(elm.__blot.blot) + 1, 0, Quill.sources.USER);
                        }
                    }

                    let deleteCard = () => {
                        if (elm) {
                            var blotToNuke = elm.__blot.blot;
                            blotToNuke.deleteAt(0);
                        }
                    }

                    if (elm && elm.__blot && elm.__onSelect) {
                        quill.disable();
                        elm.__onSelect(quill);
                        let handleKeyPress = (e) => {
                            if (e.keyCode === 27 || e.keyCode === 13) {
                                window.removeEventListener('keydown', handleKeyPress);
                                quill.enable(true);
                                deselectCard();
                            }
                            //user hit delete
                            if (e.keyCode === 46) {
                                window.removeEventListener('keydown', handleKeyPress);
                                quill.enable(true);
                                deleteCard();
                            }
                        }
                        let handleClick = (e) => {
                            if (e.which === 1 && !elm.contains(e.target)) {
                                window.removeEventListener('click', handleClick);
                                quill.enable(true);
                                deselectCard();
                            }
                        }
                        window.addEventListener('keydown', handleKeyPress);
                        window.addEventListener('click', handleClick);
                    }
                };
                quill.emitter.listenDOM('click', document.body, listener);
            }
        }

        /** QuillBlazorBridge acts as a communications protocol between javascript and blazor c# logic in reln to Quill JS.
         *  It's particularly concerned with passing data about how the quill element's saving of text or image content
         *  is handled.
         **/
        class QuillBlazorBridge {
            constructor(quillElement, blazorHook, postFileTextUrl, statusDisplayElement) {
                this.quillElement = quillElement;
                this.blazorHook = blazorHook;
                this.postFileTextUrl = postFileTextUrl;
                this.statusDisplayElement = statusDisplayElement;
            }

            //timer guff
            static typingTimer;                //timer identifier for when user stops typing
            static doneTypingInterval = 1000;  //time in ms to delay after quill activity to save

            /**
             * Call back on Blazor with the details of the image to save.
             * @param {any} fileName
             * @param {any} fileType
             */
            fireImageUploadCallbackToBlazorMethod(fileName, fileType) {
                return this.blazorHook.invokeMethodAsync('SaveImage',
                    fileName,
                    fileType);
            }

            /**
             * Given quillElement, gets the html stored inside of it.
             * @param {any} quillElement The element containing quill
             */
            getQuillHTML(quillElement) {
                return this.quillElement.__quill.root.innerHTML;
            }

            /**
             * Enqueues a status message to be shown on screen.
             * @param {any} statusMessage
             */
            setStatus(statusMessage) {
                this.blazorHook.invokeMethodAsync('EnqueueStatusMessage',
                    statusMessage);
            }

            /**
             * Using an element id reference passed in constructor, we attempt to set status messages
             * as various actions take place on the Quill JS instance running on the page.  This ensures
             * we aren't making calls in Blazor server code but instead just using Javascript to set the text
             * on the status element as changes occur.
             * @param {any} statusMessage The status message to display
             */
            showStatus(statusMessage) {
                var elementId = this.statusDisplayElement;
                if (elementId != null) {
                    var elementFound = document.getElementById(elementId);
                    if (elementFound != null) {
                        elementFound.innerText = statusMessage;
                    }
                }
            }

            /**
             * Notify blazor that the quill text has been changed by the user in some manner.
             * Fire the TextEditor's TextChangedEvent so the user can act accordingly.
             * If the user has set / configured a postFileTextUrl,
             * then POST the contents of Quill to that API endpoint.
             *
             * If successful POST of the quill data, fire the TextEditor's TextSavedEvent
             * */
            fireQuilLContentsChanged() {
                this.blazorHook.invokeMethodAsync('FireTextChangedEvent');
                const pageContents = this.getQuillHTML();

                const postUrl = this.postFileTextUrl;

                if (postUrl) {
                    this.setStatus("saving...");
                    window.fetch(postUrl,
                        {
                            method: 'POST',
                            headers: {
                            },
                            body: pageContents
                        })
                        .then(response => {
                            if (response.status === 200) {
                                this.blazorHook.invokeMethodAsync('FireTextSavedEvent');
                                this.setStatus("saved");
                            } else {
                                this.setStatus("Failed to save content." + response.status + ": " + response.statusText);
                            }
                        });
                }
            }
        }

        window.QuillFunctions = {
            createQuill: function (
                quillElement,
                toolBar,
                readOnly,
                wrapImagesInFigures,
                placeholder,
                theme,
                debugLevel,
                editorContainerId,
                imageServerUploadEnabled,
                imageServerUploadType,
                imageServerUploadUrl,
                customFonts) {
                const modulesToLoad = {
                    toolbar: (readOnly) ? false : toolBar,
                    blotFormatter: {},
                    cardEditable: wrapImagesInFigures
                };

                if (wrapImagesInFigures) {
                    Quill.register({
                        'formats/image': FigureImageBlot,
                        'modules/cardEditable': CardEditableModule,
                    }, true);
                }

                if (!readOnly && imageServerUploadEnabled) {
                    Quill.register("modules/imageUploader", ImageUploader);
                    Quill.register({ "formats/imageBlot": LoadingImage });
                    modulesToLoad["imageUploader"] = {
                        upload: (file) => {
                            const fileReader = new FileReader();
                            return new Promise((resolve, reject) => {
                                fileReader.addEventListener(
                                    "load",
                                    () => {
                                        const base64ImageSrc = new Uint8Array(fileReader.result);
                                        setTimeout(() => {
                                            const formData = new FormData();
                                            formData.append('imageFile', file);
                                            switch (imageServerUploadType) {
                                                case "BlazorMethod":
                                                    window.quillImageDataStream = function () {
                                                        return base64ImageSrc;
                                                    };
                                                    if (window.quillBlazorBridge != null) {
                                                        window.quillBlazorBridge.fireImageUploadCallbackToBlazorMethod(
                                                            file.name,
                                                            file.type)
                                                            .then(result => {
                                                                resolve(result);
                                                            });
                                                    }
                                                    break;
                                                default:
                                                    window.fetch(imageServerUploadUrl,
                                                        {
                                                            method: 'POST',
                                                            headers: {
                                                            },
                                                            body: formData
                                                        })
                                                        .then(response => {
                                                            if (response.status === 200) {
                                                                const data = response.text();
                                                                resolve(data);
                                                            }
                                                        });
                                            }
                                        }, 1500);
                                    },
                                    false
                                );

                                if (file) {
                                    fileReader.readAsArrayBuffer(file);
                                } else {
                                    reject("No file selected");
                                }
                            });
                        }
                    }
                }
                Quill.register('modules/blotFormatter', QuillBlotFormatter.default);

                var options = {
                    debug: debugLevel,
                    modules: modulesToLoad,
                    scrollingContainer: editorContainerId,
                    placeholder: placeholder,
                    readOnly: readOnly,
                    theme: theme
                };

                if (customFonts != null) {
                    var fontAttributor = Quill.import('formats/font');
                    fontAttributor.whitelist = customFonts;
                    Quill.register(fontAttributor, true);
                }
                let quill = new Quill(quillElement, options);
            },

            getQuillContent: function (quillElement) {
                return JSON.stringify(quillElement.__quill.getContents());
            },

            getQuillText: function (quillElement) {
                return quillElement.__quill.getText();
            },

            getQuillHTML: function (quillElement) {
                return quillElement.__quill.root.innerHTML;
            },

            loadQuillContent: function (quillElement, quillContent) {
                if (quillElement.__quill) {
                    var content = JSON.parse(quillContent);
                    return quillElement.__quill.setContents(content, 'api');
                }
            },

            loadQuillHTMLContent: function (quillElement, quillHTMLContent) {
                if (quillElement.__quill && quillElement.__quill.clipboard) {
                    const delta = quillElement.__quill.clipboard.convert(quillHTMLContent);
                    quillElement.__quill.setContents(delta, 'silent');
                }
            },

            enableQuillEditor: function (quillElement, mode) {
                quillElement.__quill.enable(mode);
            },

            insertQuillImage: function (quillElement, imageURL) {
                var Delta = Quill.import('delta');
                editorIndex = 0;

                if (quillElement.__quill.getSelection() !== null) {
                    editorIndex = quillElement.__quill.getSelection().index;
                }

                return quillElement.__quill.updateContents(
                    new Delta()
                        .retain(editorIndex)
                        .insert({ image: imageURL },
                            { alt: imageURL }));
            },
            insertQuillText: function (quillElement, text) {
                editorIndex = 0;
                selectionLength = 0;

                if (quillElement.__quill.getSelection() !== null) {
                    selection = quillElement.__quill.getSelection();
                    editorIndex = selection.index;
                    selectionLength = selection.length;
                }

                return quillElement.__quill.deleteText(editorIndex, selectionLength)
                    .concat(quillElement.__quill.insertText(editorIndex, text));
            },
            insertQuillHtml: function (quillElement, quillHTMLContent) {
                var Delta = Quill.import('delta');

                editorIndex = 0;
                selectionLength = 0;

                if (quillElement.__quill.getSelection() !== null) {
                    selection = quillElement.__quill.getSelection();
                    editorIndex = selection.index;
                    selectionLength = selection.length;
                }
                quillElement.__quill.updateContents(
                    new Delta()
                        .retain(editorIndex)
                        .delete(selectionLength));

                return quillElement.__quill.clipboard.dangerouslyPasteHTML(editorIndex, quillHTMLContent);

                //const delta = quillElement.__quill.clipboard.convert({ html: quillHTMLContent });

                //return quillElement.__quill.updateContents(
                //    new Delta()
                //        .retain(editorIndex)
                //        .delete(selectionLength)
                //        .insert(delta));

                //return quillElement.__quill.deleteText(editorIndex, selectionLength)
                //    .concat(quillElement.__quill.insertText(editorIndex, quillHTMLContent));
            },
            configureStickyToolbar: function (toolbarElement) {
                window.onscroll = function (e) {
                    var verticalPosition = 0;
                    if (pageYOffset) //usual
                        verticalPosition = pageYOffset;
                    else if (document.documentElement.clientHeight) //ie
                        verticalPosition = document.documentElement.scrollTop;
                    else if (document.body) //ie quirks
                        verticalPosition = document.body.scrollTop;
                    //var toolbarDiv = document.getElementById('toolBar');
                    var toolbarDiv = toolbarElement;
                    if (verticalPosition > 200) {
                        var scrollDiff = verticalPosition - 200;
                        toolbarDiv.style.top = (verticalPosition - scrollDiff) + 'px';
                    } else {
                        toolbarDiv.style.top = (verticalPosition) + 'px';
                    }
                }
            },

            setQuillBlazorBridge: function (quillElement, blazorHook, postFileTextUrl, statusDisplayElement) {
                window.quillBlazorBridge = new QuillBlazorBridge(quillElement, blazorHook, postFileTextUrl, statusDisplayElement);

                quillElement.__quill.on('text-change', window.QuillFunctions.quillTextChangedHandler);
            },

            unBindToQuillTextChangeEvent: function (quillElement) {
                if (quillElement != null && quillElement.__quill != null) {
                    quillElement.__quill.off('text-change', window.QuillFunctions.quillTextChangedHandler);
                }
            },

            setTextSavePostUrl: function (textSavePostUrl) {
                if (window.quillBlazorBridge != null) {
                    window.quillBlazorBridge.postFileTextUrl = textSavePostUrl;
                }
            },

            ShowStatusMessage: function (statusMessage) {
                if (window.quillBlazorBridge != null) {
                    window.quillBlazorBridge.showStatus(statusMessage);
                }
            },

            quillTextChangedHandler: function (delta, oldDelta, source) {
                if (source == 'user') {
                    clearTimeout(QuillBlazorBridge.typingTimer);
                    QuillBlazorBridge.typingTimer = setTimeout(fireContentsChanged, QuillBlazorBridge.doneTypingInterval);
                }

                function fireContentsChanged() {
                    window.quillBlazorBridge.fireQuilLContentsChanged();
                }
            }
        };
    })();