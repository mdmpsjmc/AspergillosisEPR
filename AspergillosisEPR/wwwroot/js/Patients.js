var Patients = function () {

    var initPatientsDataTable = function () {
            window.patientsTable = $("#patients_datatable").DataTable({
                dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
                "processing": true,
                "serverSide": true,
                "filter": true,
                "orderMulti": false,
                "initComplete": function (settings, json) {
                    newPatientsModalShow();
                    addFilteringColumns();
                    moveSearchFieldsFromFooterToHead();
                    currentUserWithRoles();
                },
                "ajax": {
                    "url": "/DataTablePatients/Load",
                    "type": "POST",
                    "datatype": "json"
                },
                buttons: [
                    {
                        'extend': 'excel',
                        'exportOptions': {
                            'columns': [0, 1, 2, 3, 4]
                        }
                    },
                    {
                        'extend': 'pdf',
                        'exportOptions': {
                            'columns': [0, 1, 2, 3, 4]
                        }
                    },
                    {
                        'extend': 'print',
                        'exportOptions': {
                            'columns': [0, 1, 2, 3, 4]
                        },

                    },
                    {
                        'extend': 'colvis'
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
                            return '<a class="btn btn-info patient-details"  style="display: none" data-role="Read Role" href="/Patients/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;' +
                                '<a class="btn btn-warning patient-edit" style="display: none" data-role="Update Role" href="/Patients/Edit/' + patient.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="btn btn-danger patient-delete" style="display: none" data-role="Delete Role" href="javascript:void(0)" data-id="' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
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


        window.patientsTable.buttons().container()
            .appendTo($('.col-sm-6:eq(0)', window.patientsTable.table().container()));
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
            if (field.match("diagnoses") || field.match("drugs") || field.match("sTGQuestionnaires")) {
                field = field.charAt(0).toUpperCase() + field.slice(1).replace("[", "_").replace("].", "__");            
            }            
            var htmlCode = "<label for='" + field + "' class='text-danger'></label>";
            var fieldError = errors[Object.keys(errors)[i]];
            $(htmlCode).html(fieldError).appendTo($("input#" + field + ", select#" + field).parent());
        }
    }

    var displayUpdateErrors = function (errors) {
        for (var i = 0; i < Object.keys(errors).length; i++) {
            var field = Object.keys(errors)[i];
            if (field.match("diagnoses") || field.match("drugs") || field.match("sTGQuestionnaires")) {
                field = field.charAt(0).toUpperCase() + field.slice(1).replace("[", "_").replace("].", "__");          
            }
            var htmlCode = "<label for='" + field + "' class='text-danger'></label>";
            var fieldError = errors[Object.keys(errors)[i]];
            $(htmlCode).html(fieldError).appendTo($("input#" + field + ", select#" + field).parent());
        }
    }

    var enableAntiForgeryProtectionWithAjax = function () {
        $(document)
            .ajaxSend(function (event, jqxhr, settings) {
                if (settings.type.toUpperCase() !== "POST") return;
                jqxhr.setRequestHeader('RequestVerificationToken', $(".AntiForge" + " input").val())
            })
    };

    var newPatientsModalShow = function () {
        $(document).off("click.launch-new-patient-modal").on("click.launch-new-patient-modal", "a.new-patient-modal-show", function () {
            LoadingIndicator.show();
            $.get("/Patients/New", function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#new-patient-modal").modal("show");
                initPatientsDateTimePickers();
            });
        });
    }

    var bindDiagnosisFormOnClick = function () {
        $(document).off("click.add-diagnosis").on("click.add-diagnosis", "a.add-diagnosis", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                LoadingIndicator.hide();
                $("div.diagnosis-form").append(responseHtml);
            });
        })
    }

    var bindDiagnosisEditFormOnClick = function () {
        $(document).off("click.add-edit-diagnosis").on("click.add-edit-diagnosis", "a.add-edit-diagnosis", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.diagnosis-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("div.diagnosis-form").append(responseHtml);
            });
        })
    }

    var bindSTGEditFormOnClick = function () {
        $(document).off("click.add-edit-stg").on("click.add-edit-stg", "a.add-edit-stg", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.stg-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("div.stg-form").append(responseHtml);
                initPatientsDateTimePickers();
            });
        })
    }

    var bindDrugsFormOnClick = function () {
        $(document).off("click.add-drug").on("click.add-drug", "a.add-drug", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                LoadingIndicator.hide();
                $("div.drug-form").append(responseHtml);
                initPatientsDateTimePickers();
                $("select.select2").select2({
                    minimumResultsForSearch: -1,
                    placeholder: function () {
                        $(this).data('placeholder');
                    }
                });
            })
        });
    }

    var bindDrugsEditFormOnClick = function () {
        $(document).off("click.add-edit-drug").on("click.add-edit-drug", "a.add-edit-drug", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.drug-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("div.drug-form").append(responseHtml);
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

    var bindSTGFormOnClick = function () {
        $(document).off("click.add-stg").on("click.add-stg", "a.add-stg-entry", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                LoadingIndicator.hide();
                $("div.stg-form").append(responseHtml);
                initPatientsDateTimePickers();
                $("select.select2").select2({
                    minimumResultsForSearch: -1,
                    placeholder: function () {
                        $(this).data('placeholder');
                    }
                });
            })
        });
    }

    var bindPatientDetailsShow = function () {
        $(document).off("click.patient-details").on("click.patient-details", "a.patient-details", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#details-modal").modal("show");
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

    var deletePatientPartialFromPopup = function () {
        $(document).off("click.delete-partial").on("click.delete-partial", "a.remove-new-diagnosis, a.remove-new-drug, a.remove-new-stg", function () {
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

    var deletePatientDbPartialFromPopup = function () {
        $(document).off("click.delete-db-partial").on("click.delete-db-partial", "a.remove-existing-diagnosis, a.remove-existing-drug, a.remove-existing-stg", function () {
            var itemId = $(this).data("id");
            var whatToRemove = $(this).data("what");
            var button = $(this);
            var question = "Are you sure you want to remove this " + whatToRemove + "?";


            BootstrapDialog.confirm(question, function (result, dialog) {
                if (result) {
                    var requestUrl = function () {
                        if (whatToRemove === "diagnosis") {
                            return "/PatientDiagnoses/Delete/" + itemId;
                        } else if (whatToRemove === "drug") {
                            return "/PatientDrugs/Delete/" + itemId;
                        } else {
                            return "/PatientSTGQuestionnaires/Delete/" + itemId;
                        }
                    }
                    $.ajax({
                        url: requestUrl(),
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        dataType: 'json'
                    }).done(function (data, textStatus) {
                        $('#loading-indicator').hide();
                        $('#loading').hide();
                        if (textStatus === "success") {
                            button.parent().parent().remove();
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
        $("input#DOB, input.datepicker, input#DateOfDeath").on("click", function () {
            $(this).datetimepicker("show");
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
            bindDiagnosisFormOnClick();
            bindDrugsFormOnClick();
            bindPatientDetailsShow();
            bindPatientEdit();
            bindDiagnosisEditFormOnClick();
            bindDrugsEditFormOnClick();
            bindOnDeletePatientClick();
            deletePatientPartialFromPopup();
            deletePatientDbPartialFromPopup();
            onPatientStatusChange();
            bindSTGFormOnClick();
            onModalClose();
            bindSTGEditFormOnClick();
            initPatientsDateTimePickers();
        },

        bindPatientsModals: function() {
            newPatientsModalShow();
            bindDiagnosisFormOnClick();
            bindDrugsFormOnClick();
            bindPatientDetailsShow();
            bindPatientEdit();
            bindDiagnosisEditFormOnClick();
            bindDrugsEditFormOnClick();
            bindOnDeletePatientClick();
            deletePatientPartialFromPopup();
            deletePatientDbPartialFromPopup();
            onPatientStatusChange();
            bindSTGFormOnClick();
            onModalClose();
            bindSTGEditFormOnClick();
            initPatientsDateTimePickers();
        },

        setupForm: function() {
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();
        },

        displayErrors: function (errors) {
            displayErrors(errors);
        },

        init: function() {
            initPatientsDataTable();
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();

        }
    }
}();