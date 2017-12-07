var Imports = function () {

    var initializeAjaxImport = function () {
        $('#btn-import-data').on('click', function (e) {
            e.preventDefault();
            var fileExtension = ['xls', 'xlsx'];
            var filename = $('#fileToImport').val();
            if (filename.length === 0) {
                alert("Please select a file.");
                return false;
            }
            else {
                var extension = filename.replace(/^.*\./, '');
                if ($.inArray(extension, fileExtension) === -1) {
                    alert("Please select only excel files.");
                    return false;
                }
            }
            var fdata = new FormData();
            var fileUpload = $("input#file").get(0);
            var files = fileUpload.files;
            fdata.append(files[0].name, files[0]);
            $.ajax({
                type: "POST",
                url: "/Imports/Create",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: fdata,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0)
                        alert('Some error occured while uploading');
                    else {
                        $('#upload-response').html(response);
                    }
                },
                error: function (e) {
                    $('#upload-response').html(e.responseText);
                }
            });
        });           
    }

    return {
        init: function () {
            initializeAjaxImport();
        }
    }
}();