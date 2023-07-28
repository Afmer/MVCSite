CKEDITOR.plugins.add( 'simpleImageUpload', {
    icons: 'simage',
    init: function( editor ) {
        var fileDialog = $('<input type="file">');
        
        fileDialog.on('change', function (e) {
			var file = fileDialog[0].files[0];
            if(file){
                const jpgReader = new FileReader();
                jpgReader.onload = function (e) {
                    var img = new Image();
                    
                    img.onload = function () {
                        var canvas = document.createElement('canvas');
                        canvas.width = img.width;
                        canvas.height = img.height;
                        var ctx = canvas.getContext('2d');
                        ctx.drawImage(img, 0, 0);
                        var jpgFile = canvas.toDataURL('image/jpeg', 1);
                        var ele = editor.document.createElement('img');
                        ele.setAttribute('src', jpgFile);
                        editor.insertElement(ele);
                    };
                    
                    img.src = e.target.result;
                };
                jpgReader.readAsDataURL(file);
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