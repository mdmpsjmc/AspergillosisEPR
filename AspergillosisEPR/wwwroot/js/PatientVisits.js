var PatientVisits = function () {
    var initializeDataTable = function() {
        window.patientVisitsDT = $("#patient_visits_datatable").DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "serverSide": true,
            "filter": true,
            "responsive": true,
            "pageLength": 100,
            "orderMulti": false,
            "order": [[1, "asc"]],
            "initComplete": function (settings, json) {
                Users.loadDataTableWithForCurrentUserRoles();
                addButtonsToDataTable();
                addFilteringColumns();
                moveSearchFieldsFromFooterToHead();

            },
            "ajax": {
                "url": "/DataTablePatientVisits/Load",
                "type": "POST",
                "datatype": "json"
            },
            buttons: [
                {
                    'extend': 'excel',
                    'className': 'btn btn-success btn-excel',
                    'titleAttr': 'Export to Excel',
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
                { "data": "id", "name": "ID", "autoWidth": true },
                {
                    "data": "visitDate", "name": "VisitDate", "autoWidth": true,
                    "render": function(data) {
                        return moment.unix(data).format("DD/MM/YYYY");
                    }            
                },
                { "data": "patientName", "name": "PatientName", "autoWidth": true },
                { "data": "rM2Number", "name": "RM2Number", "autoWidth": true },
                {
                    "data": "examinations", "name": "Examinations", "autoWidth": true,
                    "render": function (data, type, items, meta) {
                        var html = "";
                        $.each(items.examinations, function (index, item) {
                            html = html + "<label class='label-primary label'>" + item + "</label>&nbsp;";
                        })
                        return html;
                    }, "sortable": false
                },
                {
                    "render": function (data, type, visit, meta) {
                        return '<a class="btn btn-info patient-visit-details" style="display: none" data-patient-id="' + visit.patientId + '" data-role="Read Role" href="/PatientVisits/Details/' + visit.id + '"><i class=\'fa fa-eye\'></i>&nbsp;</a>&nbsp;' +
                            '<a class="btn btn-warning patient-visit-edit disable-default" style="display: none" data-id="' + visit.id + '" data-role="Update Role" href="/PatientVisits/Edit/' + visit.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;</a>&nbsp;' +
                            '<a class="btn btn-danger patient-visit-delete disable-default" style="display: none" data-role="Delete Role" href="PatientVisits/Delete/' + visit.id + '" data-id="' + visit.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;</a>&nbsp;';
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]
        });

        window.patientVisitsDT.on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
        });
    }

    var addButtonsToDataTable = function () {
        window.patientVisitsDT
              .buttons()
              .container()
              .appendTo($('.col-sm-6:eq(0)', window.patientsTable.table().container()));
    }

    var addFilteringColumns = function () {
        $('#patient_visits_datatable tfoot th').each(function () {
            var title = $(this).text();
            var isDatepicker = title.match("Date") ? " table-datepicker " : "";
            $(this).html('<input type="text" class="form-control ' + title + isDatepicker + '" placeholder="Search ' + title + '" />');
        });

        window.patientVisitsDT.columns().every(function () {
            var that = this;

            $('input', this.footer()).on('keyup change', function () {
                if (that.search() !== this.value) {
                    that
                        .search(this.value)
                        .draw();
                }
            });
        });

        $("input.table-datepicker").datetimepicker({
            format: "DD/MM/YYYY"
        }).on('dp.change', function (ev) {
            $("input.table-datepicker").trigger("change");
        });
    }

    var moveSearchFieldsFromFooterToHead = function () {
        var r = $('#patient_visits_datatable tfoot tr');
        r.find('th').each(function () {
            $(this).css('padding', 8);
        });
        $('#patient_visits_datatable thead').append(r);
        $('#search_0').css('text-align', 'center');
        $("div#patient_visits_datatable_filter").hide();
        $("input.Actions, input.form-control.ID").remove();
    }

    var newPatientVisitsModalShow = function () {
        $(document).off("click.launch-new-patient-visits-modal").on("click.launch-new-patient-visits-modal", "a.add-patient-visit", function () {
            LoadingIndicator.show();
            $.get("/PatientVisits/New", function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#new-patient-visit-modal").modal("show");
                $('input.visit-date').datetimepicker({
                    format: 'DD/MM/YYYY',                  
                }).on("dp.change", function(e) {                        
                        addDateAttributes(e.target);
                }).trigger("chage");
                initalizeSelect2PatientSearch();
            });
        });
    }

    var addDateAttributes = function (target) {
        var currentDate = $(target).val();
        var unixDate = moment(currentDate, "DD/MM/YYYY").format("X");
        $(target).attr("data-unix-date", unixDate);
        $(target).attr("data-iso-date", moment(currentDate, "DD/MM/YYYY").toISOString());
        $("tr.row-with-date[data-unix-date='" + unixDate + "']").addClass("success");
        $("tr.row-with-date:not([data-unix-date='" + unixDate + "'])").removeClass("success");
    }

    var newPatientsVisitsItemModalShow = function (binding, button, modalId) {
        $(document).off(binding).on(binding, button, function () {
            LoadingIndicator.show();
            var requestUrl = $(this).attr("href");
            $.get(requestUrl, function (responseHtml) {
                LoadingIndicator.hide();
                $("div#patient-visits-modal").html(responseHtml);
                $(modalId).modal("show");
                $(modalId).off("shown.bs.modal").on("shown.bs.modal", function () {
                    var currentDate = $("input#VisitDate.visit-date").attr("data-iso-date");
                    $("input#DateTaken").val(currentDate);
                    if (currentDate === "" || currentDate === undefined) {
                        $(modalId).modal("hide");
                        alert("Select visit date first!");
                    }
                });
            });
        });
    }

    var newPatientsVisitsItemSubmit = function (binding, button, form, appendTo) {
        $(document).off(binding).on(binding, button, function () {
            $("label.text-danger").remove();
            var patientId = $("select#PatientId").val();
            var data = $(form).serialize()+"&patientId="+patientId;
            var requestUrl = $(form).attr("action");
            $.ajax({
                url: requestUrl,
                type: "POST",
                data: data,
                contentType: "application/x-www-form-urlencoded",
                dataType: 'html',
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('error');
                    console.log(jqXHR.responseText);
                    var jsonResponse = JSON.parse(jqXHR.responseText);
                    if (jsonResponse.value) {
                        Patients.displayErrors(jsonResponse.value.errors);
                    }
                }
            }).done(function (htmlData, textStatus) {
                $(appendTo).html(htmlData);
                $(".modal:first").modal("hide");
            }).fail(function () {
                 alert("Sorry. Unable to save.");
            }); 
        });
    }

    var initalizeSelect2PatientSearch = function () {
        $("select.select2-patient-search").select2({
            ajax: {
                url: "/Select2Patients/Search",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        q: params.term
                    };
                },
                processResults: function (data, params) {                  
                    return {
                        results: data
                    };
                }
            },
            placeholder: 'Search for a patient by identifier or Name',
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 2,
            dropdownParent: $("div#new-patient-visit-modal"),
            templateResult: formatPatient,
            templateSelection: formatRepoSelection
        }).on("change", function (e) {
            var patientId = $(this).val();
            LoadingIndicator.show();
            requestPatientVisitsData(patientId);
            submitNewPatientVisit();
        });;
    }

    var formatRepoSelection = function (patient) {
        return patient.fullName || patient.text;
    } 

    var requestPatientVisitsData = function (patientId) {
        $.get("/PatientVisits/ExaminationsTabs?patientId="+patientId, function (responseHtml) {
            LoadingIndicator.hide();
            $("div#examinations-tabs").html(responseHtml);
        });
    }

   var formatPatient = function(patient) {
        if (patient.loading) {
            return patient.text;
        }

        var markup = "<div class='select2-result-repository clearfix'>" +
            "<div class='select2-result-repository__avatar'><img src='" + patient.avatarUrl + "' style ='width: 100% !important;height: auto!important;border - radius: 2px !important;'/></div>" +
            "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'>" + patient.fullName + "</div>";

        if (patient.description) {
            markup += "<div class='select2-result-repository__description'>" + patient.description + "</div>";
        }

        markup += "</div></div>";

        return markup;
    }

   var submitNewPatientVisit = function () {
       $(document).off("click.save-patient-visit").on("click.save-patient-visit", "button.submit-new-patient-visit", function () {
           LoadingIndicator.show();
           $("label.text-danger").remove();
           $.ajax({
               url: $("form#new-patient-visit-form").attr("action"),
               type: "POST",
               data: $("form#new-patient-visit-form").serialize(),
               contentType: "application/x-www-form-urlencoded",
               dataType: 'json'
           }).done(function (data, textStatus) {
               LoadingIndicator.hide();
               if (textStatus === "success") {
                   if (data.errors) {
                       Patients.displayErrors(data.errors);
                   } else {
                       $("form#new-patient-visit-form")[0].reset();
                       $("div#new-patient-visit-modal").modal("hide");
                       window.patientVisitsDT.ajax.reload(function () {
                           Users.loadDataTableWithForCurrentUserRoles();
                       });
                   }
               }
           }).fail(function (data) {
               LoadingIndicator.hide();
               $("form#new-patient-visit-form")[0].reset();
               $("div#new-patient-visit-modal").modal("hide");
               alert("There was a problem saving this patient visit. Please contact administrator");
           });
       });
   }

   var submitUpdatedPatientVisit = function () {
       $(document).off("click.save-pv-update").on("click.save-pv-udpate", "button.submit-edit-patient-visit", function () {
           LoadingIndicator.show();
           $("label.text-danger").remove();
           $.ajax({
               url: $("form#edit-patient-visit-form").attr("action"),
               type: "POST",
               data: $("form#edit-patient-visit-form").serialize(),
               contentType: "application/x-www-form-urlencoded",
               dataType: 'json'
           }).done(function (data, textStatus) {
               LoadingIndicator.hide();
               if (textStatus === "success") {
                   if (data.errors) {
                       Patients.displayErrors(data.errors);
                   } else {
                       $("form#edit-patient-visit-form")[0].reset();
                       $("div#edit-patient-visit-modal").modal("hide");
                       window.patientVisitsDT.ajax.reload(function () {
                           Users.loadDataTableWithForCurrentUserRoles();
                       });
                   }
               }
           }).fail(function (data) {
               LoadingIndicator.hide();
               $("form#new-patient-visit-form")[0].reset();
               $("div#new-patient-visit-modal").modal("hide");
               alert("There was a problem saving this patient visit. Please contact administrator");
           });
       });
   }

    

   var showPatientVisitDetails = function () {
       $(document).off("click.show-pv-details").on("click.show-pv-details", "a.patient-visit-details:not(a.patient-details)", function (e) {
           var currentId = $(this).data("id");
           var requestUrl = $(this).attr("href");
           var patientId = $(this).data("patient-id");
           e.preventDefault();
           $.get(requestUrl, function (htmlResponse) {
               $("div#modal-container").html(htmlResponse);
               $("div#visit-details-modal").modal("show");
               
               $.getJSON("/Patients/" + patientId + "/Charts/SGRQ", function (response) {
                   Charts.sgrqChartFromResponse(response);
               });
           });
       });
   }

   var onPatientVisitEditShow = function () {
       $(document).off("click.show-pv-edit").on("click.show-pv-edit", "a.patient-visit-edit", function (e) {
           var currentId = $(this).data("id");
           var requestUrl = $(this).attr("href");
           e.preventDefault();
           $.ajax({
               url: requestUrl,
               type: "GET",
           }).done(function (responseHtml, textStatus) {
               LoadingIndicator.hide();
               $("div#edit-patient-visit-modal-container").html(responseHtml);
               $("div#edit-patient-visit-modal").modal("show");
               $("select#PatientId").select2();
               var visitDateUnix = $("input#VisitDate").val()
               var formattedDate = moment.unix(visitDateUnix).format("DD/MM/YYYY");
               $("input#VisitDate").attr("value", formattedDate);
               $("input#VisitDate").attr("data-iso-date", moment.unix(visitDateUnix).toISOString());
               $("input.visit-date").datetimepicker({
                   format: "DD/MM/YYYY"
               }).on("dp.change", function (e) {                   
                   addDateAttributes(e.target);
                   $(e.target).attr("value", $(e.target).val());
               }).trigger("change");
           }).fail(function (data) {
               LoadingIndicator.hide();
               //$("form#edit-patient-visit-form")[0].reset();
               alert("There was a problem requesting this patient visit. Please contact administrator");
           });
       });
   }

   var onPatientVisitDelete = function () {
       $(document).off("click.pv-delete").on("click.pv-delete", "a.patient-visit-delete", function (e) {
           LoadingIndicator.show();
           var data = { "id": $(this).data("id") };
           var question = "Delete this visit and related entries?";
           var deleteUrl = $(this).attr("href");
           BootstrapDialog.confirm(question, function (result, dialog) {
               if (result) {
                   $.post(deleteUrl, data, function (responseHtml) {
                       LoadingIndicator.hide();
                       window.location.reload(true);
                   });
               }
           });         
       });
   }

   var onExportOptionsClick = function () {
       $(document).off("click.export-pv").on("click.export-pv", "a.patient-visit-export", function (e) {
           e.preventDefault();
           var exportUrl = $(this).attr("href");
           var exportType = $(this).data("file");
           var visitId = $(this).data("visit-id");
           var sgrqChartImage = "sgrqChart="+ encodeURIComponent($("img#sgrq-chart-image").attr("src"));
           $("div#patient-visit-export-modal").modal("show");
           $(document).off("click.export-visit").on("click.export-visit", "button#export-patient-visit", function () {
               var exportUrlWithOptions = exportUrl + "?otherVisits=" + $("input#IncludeOtherVisits").prop("checked");
               AjaxFileDownload.execute(exportUrlWithOptions, sgrqChartImage, "Patient_Visits_Details_" + visitId + ".pdf", "application/pdf");
           });    
       });
   }

    return {

        init: function () {
            initializeDataTable();
            newPatientVisitsModalShow();
            newPatientsVisitsItemModalShow("click.show-patient-visits-modal", "a.add-measurement", "#new-measurement-modal");
            newPatientsVisitsItemModalShow("click.show-patient-visits-sgrq", "a.add-stg-questionnaire", "#new-stg-modal");
            newPatientsVisitsItemModalShow("click.show-patient-visits-ig", "a.add-ig", "#new-ig-modal");
            newPatientsVisitsItemModalShow("click.show-patient-visits-radiology", "a.add-radiology", "#new-radiology-modal");
            newPatientsVisitsItemSubmit("click.submit-patient-visits-item", "button.submit-patient-visit-item", "form#new-measurement-form", "div.measurement-form");
            newPatientsVisitsItemSubmit("click.submit-patient-visits-sgrq", "button.submit-patient-visit-stg", "form#new-stg-form", "div#sgrq-data");
            newPatientsVisitsItemSubmit("click.submit-patient-visits-ig", "button.submit-patient-visit-ig", "form#new-ig-form", "div#ig-data");
            newPatientsVisitsItemSubmit("click.submit-patient-visits-radiology", "button.submit-patient-visit-radiology", "form#new-radiology-form", "div#radiology-data");
            showPatientVisitDetails();
            onPatientVisitEditShow();
            submitUpdatedPatientVisit();
            onPatientVisitDelete();
            onExportOptionsClick();
        }
    }
}();