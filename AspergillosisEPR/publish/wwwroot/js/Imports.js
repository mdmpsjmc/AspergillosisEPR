var Imports = function () {

    var initializeAjaxImport = function () {
        $('#btn-import-data').on('click', function (e) {
            e.preventDefault();
            $('#upload-response').html("");
            var fileExtension = ['xls', 'xlsx', 'csv', 'pdf', 'docx', 'doc'];
            var filename = $('#fileToImport').val();
            if (filename.length === 0) {
                alert("Please select a valid file.");
                return false;
            }
            else {
                var extension = filename.replace(/^.*\./, '');
                if ($.inArray(extension, fileExtension) === -1) {
                    alert("Please select only excel, word or pdf files.");
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

    var initializeBatchAjaxImport = function () {
        $('#batch-import-form-submit').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ClinicLettersBatchImport/Create",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };

    var initializePdfBatchAjaxImport = function () {
        $('button#batch-observation-points-form-submit').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ClinicLettersBatchImport/Update",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };
    
    var initializeExternalICD10DiagnosisImport = function () {
        $('button#external-icd-10').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/Imports/ExternalDatabaseICD10Diagnosis",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };

    var initializeExternalIgImport = function () {
        $('button#external-ig').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ExternalImports/Ig",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };

    var initializeExternalLabResultImport = function () {
        $('button#external-lab').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ExternalImports/LabTests",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };

    var initializeVoriconazoleLevelImport = function () {
        $('button#external-voriconazole').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ExternalImports/Vori",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };


    var initializeRadiologyImport = function () {
        $('button#external-radiology').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ExternalImports/Radiology",
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.length === 0) {
                        LoadingIndicator.hide();
                        alert('Some error occured while uploading');
                    } else {
                        $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
                        LoadingIndicator.hide();
                    }
                },
                error: function (e) {
                    LoadingIndicator.hide();
                    $('#batch-upload-response').html(e.responseText);
                }
            });
        });
    };


  var initializeSGRQImport = function () {
    $('button#external-sgrq').on('click', function (e) {
      e.preventDefault();
      $.ajax({
        type: "POST",
        url: "/ExternalImports/SGRQ",
        contentType: false,
        processData: false,
        success: function (response) {
          if (response.length === 0) {
            LoadingIndicator.hide();
            alert('Some error occured while uploading');
          } else {
            $('#batch-upload-response').html("<div class='alert alert-info'><i class='fa fa-info-circle'></i> &nbsp; Processed (added or updated)" + response.result + " records from orginal file</div>");
            LoadingIndicator.hide();
          }
        },
        error: function (e) {
          LoadingIndicator.hide();
          $('#batch-upload-response').html(e.responseText);
        }
      });
    });
  };

    return {
        init: function () {
          initializeAjaxImport();
          initializeBatchAjaxImport();
          initImportsDataTable();
          initializePdfBatchAjaxImport();
          initializeExternalICD10DiagnosisImport();
          initializeExternalLabResultImport();
          initializeVoriconazoleLevelImport();
          initializeRadiologyImport();
          initializeSGRQImport();
        }
    };
}();