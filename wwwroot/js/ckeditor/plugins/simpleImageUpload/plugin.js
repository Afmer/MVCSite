CKEDITOR.plugins.add( 'simpleImageUpload', {
    icons: 'simage',
    init: function( editor ) {
        var fileDialog = $('<input type="file">');
        
        fileDialog.on('change', function (e) {
            var uploadUrl = editor.config.uploadUrl;
			var file = fileDialog[0].files[0];
            if (file) {
                const reader = new FileReader();
                var base64Data;
                reader.readAsDataURL(file);
                reader.onload = function() {
                    var ele = editor.document.createElement('img');
                    ele.setAttribute('src', reader.result);
                    editor.insertElement(ele);
                };
            }

        })
        editor.ui.addButton( 'SImage', {
            label: 'Вставить изображение',
            command: 'openDialog',
            toolbar: 'insert'
        });
        editor.addCommand('openDialog', {
            exec: function(editor) {
                fileDialog.click();
            }
        })
    }
});