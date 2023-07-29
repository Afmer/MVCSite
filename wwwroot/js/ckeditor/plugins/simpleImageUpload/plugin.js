CKEDITOR.plugins.add( 'simpleImageUpload', {
    icons: 'simage',
    init: function( editor ) {
        var fileDialog = $('<input type="file">');
        
        fileDialog.on('change', function (e) {
            var uploadUrl = editor.config.uploadUrl;
			var file = fileDialog[0].files[0];
			var imageData = new FormData();
			imageData.append('file', file);

			$.ajax({
				url: uploadUrl,
				type: 'POST',
				contentType: false,
				processData: false,
				data: imageData,
			}).done(function(done) {
                var tempImg = new Image();
                tempImg.src = done.url;
                tempImg.onload = function () {
                  var maxWidth = 400;
                  var maxHeight = 300;

                  var newWidth, newHeight;
                  if (tempImg.width > maxWidth || tempImg.height > maxHeight) {
                    var widthRatio = maxWidth / tempImg.width;
                    var heightRatio = maxHeight / tempImg.height;
                    var scaleRatio = Math.min(widthRatio, heightRatio);
                    newWidth = Math.floor(tempImg.width * scaleRatio);
                    newHeight = Math.floor(tempImg.height * scaleRatio);
                  } else {
                    newWidth = tempImg.width;
                    newHeight = tempImg.height;
                  }
        
                  var ele = editor.document.createElement('img');
                  ele.setAttribute('src', done.url);
                  ele.setAttribute('width', newWidth);
                  ele.setAttribute('height', newHeight);
                  editor.insertElement(ele);
                };
			});

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