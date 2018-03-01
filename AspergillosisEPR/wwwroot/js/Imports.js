var Imports = function () {

    var initializeAjaxImport = function () {
        $('#btn-import-data').on('click', function (e) {
            e.preventDefault();
            $('#upload-response').html("");
            var fileExtension = ['xls', 'xlsx', 'csv', 'pdf'];
            var filename = $('#fileToImport').val();
            if (filename.length === 0) {
                alert("Please select a valid file.");
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
            var dbImportTypeId = $("select#DbImportTypeId").val();
            fdata.append("DbImportTypeId", dbImportTypeId);
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
                        $('#upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)"+ response.result + " records from orginal file</div>");
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
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "serverSide": true,
            "filter": true,
            "orderMulti": false,
            "order": [[1, "desc"]],
            "initComplete": function (settings, json) {               
            },
            "ajax": {
                "url": "/DataTableDbImports/Load",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                {
                    "data": "importedDate", "name": "ImportedDate", "autoWidth": true,
                    "render": function (data) {
                        return moment.unix(data).format("MM/DD/YYYY");
                    }
                },
                { "data": "importTypeName", "name" : "Import Type", "autoWidth": true, "orderable" : false},
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