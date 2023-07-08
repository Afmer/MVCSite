(function(){
    var pluginName = 'blockimagepaste';
    function replaceImgText(html) {
        var ret = html.replace( /<img[^>]*src="data:image\/(bmp|dds|gif|jpg|jpeg|png|psd|pspimage|tga|thm|tif|tiff|yuv|ai|eps|ps|svg);base64,.*?"[^>]*>/gi, function( img ){
            alert("Direct image paste is not allowed.");
            return '';
        });
        return ret;
    };

    function chkImg(editor) {
        // don't execute code if the editor is readOnly
        if (editor.readOnly)
            return;

        setTimeout( function() {
            editor.document.$.body.innerHTML = replaceImgText(editor.document.$.body.innerHTML);
        },100);
    };

    CKEDITOR.plugins.add( pluginName, {
        icons: pluginName,
        init : function( editor ){

            editor.on( 'contentDom', function() {
                // For Firefox
                editor.document.on('drop', function(e) {chkImg(editor);});
                // For IE
                editor.document.getBody().on('drop', function(e) {chkImg(editor);});

                editor.document.on( 'paste', function(e) {chkImg(editor);});

                // For IE
                editor.document.getBody().on('paste', function(e) {chkImg(editor);});
            });

        } //Init
    });

})();