var Patients = function () {

    var initPatientsDataTable = function () {
        $("#patients_datatable").DataTable().destroy();
            window.patientsTable = $("#patients_datatable").DataTable({
                dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
                "processing": true,
                "serverSide": true,
                "filter": true,
                "pageLength" : 100,
                "orderMulti": false,
                "initComplete": function (settings, json) {
                    newPatientsModalShow();
                    addFilteringColumns();
                    moveSearchFieldsFromFooterToHead();
                    currentUserWithRoles();
                    addButtonsToDataTable();
                },
                "ajax": {
                    "url": "/DataTablePatients/Load",
                    "type": "POST",
                    "datatype": "json"
                },
                buttons: [
                    {
                        'extend': 'excel',
                        'className': 'btn btn-success btn-excel',
                        'titleAttr' : 'Export to Excel',
                        'text': '<i class="fa fa-file-excel-o"></i>',
                        'exportOptions': {
                            columns: ':visible'
                        }
                    },
                    {
                        'extend': 'pdf',
                        'className': 'btn btn-danger btn-pdf',
                        'titleAttr': 'Export to PDF',
                        'text': '<i class="fa fa-file-pdf-o"></i>',
                        'exportOptions': {
                            columns: ':visible'
                        }
                    },
                    {
                        'extend': 'print',
                        'className': 'btn btn-warning btn-print',
                        'titleAttr': 'Print',
                        'text': '<i class="fa fa-print"></i>',
                        'exportOptions': {
                            columns: ':visible'
                        }

                    },
                    {
                        'extend': 'colvis',
                        'className': 'btn btn-info btn-vis',
                        'titleAttr': 'Column visibility',
                        'text': '<i class="fa fa-eye"></i>',
                    }
                ],
                "columns": [
                    { "data": "rM2Number", "name": "RM2Number", "autoWidth": true },
                    { "data": "primaryDiagnosis", "name": "Primary Diagnosis", "autoWidth": true, "sortable": false },
                    { "data": "firstName", "name": "FirstName", "autoWidth": true },
                    { "data": "lastName", "name": "LastName", "autoWidth": true },
                    { "data": "gender", "name": "Gender", "autoWidth": true },
                    {
                        "data": "dob", "name": "DOB", "autoWidth": true,
                        "render": function(data) {
                            return moment.unix(data).format("MM/DD/YYYY");
                        }                    
                    },
                    {
                        "render": function (data, type, patient, meta) {
                            return '<a class="btn btn-info patient-details" style="display: none" data-role="Read Role" href="/Patients/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;</a>&nbsp;' +
                                '<a class="btn btn-warning patient-edit" style="display: none" data-role="Update Role" href="/Patients/Edit/' + patient.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;</a>&nbsp;' +
                                '<a class="btn btn-danger patient-delete" style="display: none" data-role="Delete Role" href="javascript:void(0)" data-id="' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;</a>&nbsp;';
                        },
                        "sortable": false,
                        "width": 250,
                        "autoWidth": false
                    }
                ]
        });

        window.patientsTable.on('draw.dt', function () {
            currentUserWithRoles();
        }); 
    }

    var submitNewPatient = function () {
        $(document).off("click.save-patient").on("click.save-patient", "button.submit-new-patient", function () {
            LoadingIndicator.show();
            $("label.text-danger").remove();
                $.ajax({
                    url: $("form#new-patient-form").attr("action"),
                    type: "POST",
                    data: $("form#new-patient-form").serialize(),
                    contentType: "application/x-www-form-urlencoded",
                    dataType: 'json'
                }).done(function (data, textStatus) {
                    LoadingIndicator.hide();
                    if (textStatus === "success") {
                        if (data.errors) {
                            displayErrors(data.errors);
                        } else {
                            $("form#new-patient-form")[0].reset();
                            $("div#new-patient-modal").modal("hide");
                            window.patientsTable.ajax.reload(function () {
                                currentUserWithRoles();

                            });
                        }
                    }
                }).fail(function (data) {
                    LoadingIndicator.hide();
                    $("form#new-patient-form")[0].reset();
                    $("div#new-patient-modal").modal("hide");
                    alert("There was a problem saving this patient. Please contact administrator");
                });
        });
    }

    var displayErrors = function (errors) {
        for (var i = 0; i < Object.keys(errors).length; i++) {
            var field = Object.keys(errors)[i];
            var fieldCapitalized = field.charAt(0).toUpperCase() + field.slice(1);

            if (field.match("diagnoses")
                || field.match("drugs")
                || field.match("sTGQuestionnaires")
                || field.match("patientImmunoglobulin")
                || field.match("Finding")
                || field.match("patientMedicalTrial")
                || field.match("caseReportFormResult")) {
                var fieldName = fieldCapitalized;
                field = fieldCapitalized.replace(new RegExp("\\[", "g"), "_").replace(new RegExp("].","g"), "__");            
            } else {
                field = fieldCapitalized;
            }           
            var htmlCode = "<label for='" + fieldCapitalized + "' class='text-danger'></label>";
            var fieldError = errors[Object.keys(errors)[i]];
            var fieldWithError = $("input#" + field + ", select#" + field + ", input#" + field.toUpperCase());
            if (fieldWithError.length > 0) {
                var fieldToAppend = fieldWithError.parent();
                $(htmlCode).html(fieldError).appendTo(fieldToAppend);
            } else {
                var fieldToAppendName = "input[name='" + fieldName + "']";
                var fieldToAppend = $(fieldToAppendName).parent();
                $(htmlCode).html(fieldError).appendTo(fieldToAppend);
            }
        }
    }

    var displayUpdateErrors = function (errors) {
        displayErrors(errors);
    }

    var enableAntiForgeryProtectionWithAjax = function () {
        $(document)
            .ajaxSend(function (event, jqxhr, settings) {
                if (settings.type.toUpperCase() !== "POST") return;
                jqxhr.setRequestHeader('RequestVerificationToken', $(".AntiForge" + " input").val())
            });
    };

    var newPatientsModalShow = function () {
        $(document).off("click.launch-new-patient-modal").on("click.launch-new-patient-modal", "a.new-patient-modal-show", function () {
            LoadingIndicator.show();
            $.get("/Patients/New", function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#new-patient-modal").modal("show");
                initPatientsDateTimePickers();
                CaseReportForms.onPatientCaseReportFormSelectChange();
            });
            CaseReportForms.deletePartialFromPopup();
            deletePartialFromPopup();
        });
    }

    var bindNewPartialOnPatientFormClick = function () {
        $(document).off("click.new-pat-partial").on("click.new-pat-partial", "a.add-new-patient-partial, a.edit-patient-partial, a.add-form-field", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var visibleRow = $(this).data("visible-row");
            var insertIntoClass = $(this).data("insert-into-class"); 
            var index = $(visibleRow).length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();                          
                $(insertIntoClass).append(responseHtml);
                initPatientsDateTimePickers();
                $("select.select2").select2({
                    minimumResultsForSearch: -1,
                    placeholder: function () {
                        $(this).data('placeholder');
                    }
                });
            });
        })
    }

    var bindPatientDetailsShow = function () {
        $(document).off("click.patient-details").on("click.patient-details", "a.patient-details", function (e) {
            LoadingIndicator.show();
            e.preventDefault();            
            var url = $(this).attr("href")
            $.get(url, function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#details-modal").modal("show");
                var patientId = url.split("/")[3];
                $.getJSON("/Patients/" + patientId + "/Charts/SGRQ", function (response) {
                    Charts.sgrqChartFromResponse(response);
                });
                $.getJSON("/Patients/" + patientId + "/Charts/Immunology", function (response) {
                    $("div#ig-charts").html(""); //clears hidden charts div. 
                    Charts.igChartsFromResponse(response, true);
                });
                $("select[multiple='multiple']").multiSelect();
            });
        });
    }

    var bindPatientEdit = function () {
        $(document).off("click.patient-edit").on("click.patient-edit", "a.patient-edit", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#edit-modal").modal("show");
                //$.validator.unobtrusive.parse("form#edit-patient-form");
                updatePatient();
                onPatientStatusChange();
                initPatientsDateTimePickers();
                $("select.select2").select2({
                    minimumResultsForSearch: -1,
                    placeholder: function () {
                        $(this).data('placeholder');
                    }
                });
                $("select[multiple='multiple']").multiSelect();
                CaseReportForms.onPatientCaseReportFormSelectChange();
                CaseReportForms.deletePartialFromPopup();
                deletePartialFromPopup();
            });
        });
    }

    var updatePatient = function () {
        $(document).off("click.update-patient").on("click.update-patient", "button.update-patient", function (e) {
            $("label.text-danger").remove();
            LoadingIndicator.show();            
            e.preventDefault();
            $.ajax({
                url: $("form#edit-patient-form").attr("action"),
                type: "POST",
                data: $("form#edit-patient-form").serialize(),
                contentType: "application/x-www-form-urlencoded",
                dataType: 'json'
            }).done(function (data, textStatus) {
                LoadingIndicator.hide();
                if (textStatus === "success") {
                    if (data.errors) {
                        displayUpdateErrors(data.errors);
                    } else {
                        $("form#edit-patient-form")[0].reset();
                        $("div#edit-modal").modal("hide");
                        window.patientsTable.ajax.reload(function () {
                            currentUserWithRoles();
                        });
                    }
                }
            }).fail(function (data) {
                LoadingIndicator.hide();
                $("form#edit-patient-form")[0].reset();
                $("div#edit-modal").modal("hide");
                alert("There was a problem saving this patient. Please contact administrator");
            });
        });
    }

    var bindOnDeletePatientClick = function () {
        $(document).off("click.patient-delete").on("click.patient-delete", "a.patient-delete", function () {
            LoadingIndicator.show();
            var patientId = $(this).data("id");
            var question = 'Are you sure you want to delete this patient and all related data?';
            BootstrapDialog.confirm(question, function (result, dialog) {
                LoadingIndicator.hide();
                if (result) {
                    $.ajax({
                        url: "/Patients/Delete/" + patientId,
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        dataType: 'json'
                    }).done(function (data, textStatus) {
                        if (textStatus === "success") {
                            window.patientsTable.ajax.reload();
                        }
                    }).always(function () {
                        LoadingIndicator.hide();
                    });
                }
            });
        });
    }

    var deletePartialFromPopup = function () {
        $(document).off("click.delete-partial").on("click.delete-partial", "a.remove-new-diagnosis, a.remove-new-drug, a.remove-new-stg, a.remove-new-ig, a.remove-partial", function () {
            var whatToRemove = $(this).data("what");
            var button = $(this);
            var question = "Are you sure you want to remove this " + whatToRemove + "?";
            BootstrapDialog.confirm(question, function (result, dialog) {
                if (result) {
                    button.parent().parent().remove();
                }
            });
        });
    }

    var deleteDbPartialFromPopup = function () {
        $(document).off("click.delete-db-partial").on("click.delete-db-partial", "a.remove-existing-diagnosis, a.remove-existing-drug, a.remove-existing-stg, a.remove-existing-item", function (e) {
            var itemId = $(this).data("id");
            var whatToRemove = $(this).data("what");
            var button = $(this);
            var question = "Are you sure you want to remove this " + whatToRemove + "?";
            var requestUrl = $(this).attr("url");
            if (requestUrl === undefined) {
                requestUrl = $(this).attr("href");
            }
            e.preventDefault();
            BootstrapDialog.confirm(question, function (result, dialog) {
                if (result) {     
                    $.ajax({
                        url: $(button).attr("href"),
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        dataType: 'json'
                    }).done(function (data, textStatus) {
                        $('#loading-indicator').hide();
                        $('#loading').hide();
                        if (textStatus === "success") {
                            button.parent().parent().remove();
                            var buttonIdx = button.attr("data-index");
                            if (buttonIdx !== undefined) {
                                $("div.panel-default[data-index='" + buttonIdx + "']").parent().remove();
                            }
                        }
                    }).always(function () {
                        LoadingIndicator.hide();
                    });
                }
            });
        });
    }

    var addFilteringColumns = function () {
        $('#patients_datatable tfoot th').each(function () {
            var title = $(this).text();
            $(this).html('<input type="text" class="form-control ' + title + '" placeholder="Search ' + title + '" />');
        });

        window.patientsTable.columns().every(function () {
            var that = this;

            $('input', this.footer()).on('keyup change', function () {
                if (that.search() !== this.value) {
                    that
                        .search(this.value)
                        .draw();
                }
            });
        });
    }

    var moveSearchFieldsFromFooterToHead = function () {
        var r = $('#patients_datatable tfoot tr');
        r.find('th').each(function () {
            $(this).css('padding', 8);
        });
        $('#patients_datatable thead').append(r);
        $('#search_0').css('text-align', 'center');
        $("div#patients_datatable_filter").hide();
        $("input.Actions").remove();
    }

    var currentUserWithRoles = function () {
        $.getJSON("/Account/GetCurrentUserRolesAsync", function (response) {
            var container = $("div#current-user-roles");
            container.attr("data-id", response.user);
            container.attr("data-roles", response.roles.join(","));
            $.each(response.roles, function (index, role) {
                $("[data-role='" + role + "']").show();
                if (role === "Admin Role") {
                    $("[data-role]").show();
                }
            });
        });
    }

    var onPatientStatusChange = function () {
        $(document).off("change.patient-status").on("change.patient-status", "select#PatientStatusId", function () {
            var selectedValue = $("select#" + $(this).attr("id") + " option:selected").text().toLowerCase();
            if (selectedValue.toLowerCase() === "deceased") {
                $("div.death").removeClass("hidden");
                initPatientsDateTimePickers();
            } else {
                $("div.death").addClass("hidden");
                $("input#DateOfDeath").val("")
            }
        });
    }

    var onModalClose = function () {
        $('#edit-modal, #new-patient-modal').on('hidden.bs.modal', function () {
            LoadingIndicator.hide();
        })
    }

    var initPatientsDateTimePickers = function () {
        $('input[type="date"]').attr('type', 'text');
        $('input#DOB, input#DateOfDeath, input.datepicker').datetimepicker({
            format: 'YYYY-MM-DD'
        });
        $('input.date-taken').datetimepicker({
            format: 'DD/MM/YYYY'
        });
        $("input#DOB, input.datepicker, input#DateOfDeath").on("click", function () {
            $(this).datetimepicker("show");
        });
    }

    var addButtonsToDataTable = function () {
        window.patientsTable
              .buttons()
              .container()
              .appendTo($('.col-sm-6:eq(0)', window.patientsTable.table().container()));
    }

    var onExportOptionsShow = function () {
        $(document).off("click.export-trigger").on("click.export-trigger", "a.export-trigger", function (e) {
            e.preventDefault();
            var exportToFile = $(this).data("file");
            $("button.download-details").attr("id", exportToFile);
            var tabs = $(this).parents("div.modal-content").find("ul#details-tab li a");
            var container = $("div.inline-group.labels");
            container.html("");            
            $.each(tabs, function (index, element) {
                var tabHtml = $(element).html();
                var tmp = document.createElement("DIV");
                tmp.innerHTML = tabHtml;
                var tabName = tmp.textContent.trim();
                var tabCapitalized = tabName.charAt(0).toUpperCase() + tabName.slice(1);
                var isChecked = index === 0 ? "checked=\"checked\"" : "";
                var isDisabled = (index === 0 && tabCapitalized === "Details") ? "disabled=\"disabled\"" : "";
                var htmlOption = '<label class=\"checkbox\"><input type= \"checkbox\" ' + isDisabled + ' name= \"Show' + tabCapitalized.replace(/\s/g, '') + '\" ' + isChecked + '/><i></i> <span class=\"checkbox-label\"> ' + tabName + '</span></label>'
                container.append(htmlOption);
            });
            $("div#export-options-modal").modal("show"); 

            $("div#export-options-modal").on('shown.bs.modal', function (e) {
                onDownloadPatientDetailsPdf();
                onDownloadPatientDetailsExcel();
            });
        });
    }
        
    var onDownloadPatientDetailsPdf = function () {
        $(document).off("click.pdf-details-download").on("click.pdf-details-download", "button#pdf.download-details", function (e) {
            e.preventDefault();
            var sgrqChartImage = encodeURIComponent($("img#sgrq-chart-image").attr("src"));
            var patientId = $(this).data("id");
            var requestUrl = "patients/" + patientId + "/exports/pdf";            
            var requestData = $("form#export-options-form").serialize() + "&sgrqChart=" + sgrqChartImage;
            var igCharts = $("div#ig-charts img.ig-chart");
            $.each(igCharts, function (index, chart) {
                requestData = requestData + "&PatientCharts[" + index + "].Image=" + encodeURIComponent($(chart).attr("src"));
            });
            requestData = requestData + "&igChartsLength=" + igCharts.length;
            AjaxFileDownload.execute(requestUrl, requestData, "Patient_Details_" + patientId + ".pdf", "application/pdf");  
        });
    }

    var onDownloadPatientDetailsExcel = function () {
        $(document).off("click.xls-details-download").on("click.xls-details-download", "button#excel.download-details", function (e) {
            e.preventDefault();
            var sgrqChartImage = encodeURIComponent($("img#sgrq-chart-image").attr("src"));
            var patientId = $(this).data("id");
            var requestUrl = "patients/" + patientId + "/exports/excel";     
            var requestData = $("form#export-options-form").serialize();
            AjaxFileDownload.execute(requestUrl, requestData, "Patient_Details_" + patientId + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        });
    }


    return {

        loadDataTableWithForCurrentUserRoles: function () {
            currentUserWithRoles();
        },

        publicSetup: function () {
            currentUserWithRoles();
            addFilteringColumns();
            moveSearchFieldsFromFooterToHead();
            newPatientsModalShow();
            bindPatientDetailsShow();
            bindPatientEdit();
            bindOnDeletePatientClick();
            deletePartialFromPopup();
            deleteDbPartialFromPopup();
            onPatientStatusChange();
            onModalClose();
            initPatientsDateTimePickers();
            onExportOptionsShow();
        },

        bindPatientsModals: function() {
            newPatientsModalShow();
            bindPatientDetailsShow();
            bindPatientEdit();
            bindOnDeletePatientClick();
            deletePartialFromPopup();
            deleteDbPartialFromPopup();
            onPatientStatusChange();
            onModalClose();
            initPatientsDateTimePickers();
            bindNewPartialOnPatientFormClick();
            onExportOptionsShow();
        },

        setupForm: function() {
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();
        },

        displayErrors: function (errors) {
            displayErrors(errors);
        },

        onExportOptionsShow: function () {
            onExportOptionsShow();
        },

        deletePartialFromPopup: function () {
            deletePartialFromPopup();
        },

        deleteDbPartialFromPopup: function () {
            deleteDbPartialFromPopup();
        },

        init: function() {
            initPatientsDataTable();
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();
        }
    }
}();