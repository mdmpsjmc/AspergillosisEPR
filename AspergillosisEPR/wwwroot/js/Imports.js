var Imports = function () {

    var initializeAjaxImport = function () {
        $('#btn-import-data').on('click', function (e) {
            e.preventDefault();
            $('#upload-response').html("");
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
            LoadingIndicator.show();
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
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Imported "+ response.result + " patients into database</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#upload-response').html(e.responseText);
                }
            });
        });           
    }

    var initImportsDataTable = function () {
        window.importsTable = $("#imports_datatable").DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            "orderMulti": false,
            "initComplete": function (settings, json) {               
            },
            "ajax": {
                "url": "/DataTableJson/LoadDbImports",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "importedDate", "name": "ImportedDate", "autoWidth": true },
                { "data": "importedFileName", "name": "ImportedFileName", "autoWidth": true },
                { "data": "patientsCount", "name": "PatientsCount", "autoWidth": true }              
            ]
        });
    }

    return {
        init: function () {
            initializeAjaxImport();
            initImportsDataTable();
        }
    }
}();