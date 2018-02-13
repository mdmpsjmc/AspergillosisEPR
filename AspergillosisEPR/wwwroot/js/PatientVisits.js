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
            "orderMulti": false,
            "order": [[1, "asc"]],
            "initComplete": function (settings, json) {
                Users.loadDataTableWithForCurrentUserRoles();
            },
            "ajax": {
                "url": "/DataTablePatientVisits/Load",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                {
                    "data": "visitDate", "name": "VisitDate", "autoWidth": true,
                    "render": function(data) {
                        return moment.unix(data).format("MM/DD/YYYY");
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
                        return '<a class="btn btn-info patient-details" style="display: none" data-role="Read Role" href="/Patients/Details/' + visit.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;' +
                            '<a class="btn btn-warning patient-edit" style="display: none" data-role="Update Role" href="/Patients/Edit/' + visit.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger patient-delete" style="display: none" data-role="Delete Role" href="javascript:void(0)" data-id="' + visit.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
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

    var newPatientVisitsModalShow = function () {
        $(document).off("click.launch-new-patient-visits-modal").on("click.launch-new-patient-visits-modal", "a.add-patient-visit", function () {
            LoadingIndicator.show();
            $.get("/PatientVisits/New", function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#new-patient-visit-modal").modal("show");
                $('input.visit-date').datetimepicker({
                    format: 'DD/MM/YYYY',                  
                }).on("dp.change", function () {
                        var currentDate = $("input.visit-date").val();
                        var unixDate = moment(currentDate.split("/").reverse().join("-")).format("X");
                        $("input.visit-date").attr("data-unix-date", unixDate);
                        $("input.visit-date").attr("data-iso-date", moment(currentDate.split("/").reverse().join("-")).toISOString());
                        $("tr.row-with-date[data-unix-date='" + unixDate + "']").addClass("success");
                        $("tr.row-with-date:not([data-unix-date='" + unixDate + "'])").removeClass("success");
                    });
                initalizeSelect2PatientSearch();
            });
        });
        //$.fn.modal.Constructor.prototype.enforceFocus = function () { };
    }

    var newPatientsVisitsItemModalShow = function (binding, button, modalId) {
        $(document).off(binding).on(binding, button, function () {
            LoadingIndicator.show();
            var requestUrl = $(this).attr("href");
            $.get(requestUrl, function (responseHtml) {
                LoadingIndicator.hide();
                $("div#patient-visits-modal").html(responseHtml);
                $(modalId).modal("show");
                $(modalId).on("shown.bs.modal", function () {
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
                    console.log(jqXHR, textStatus, errorThrown);
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
            placeholder: 'Search for a patient by RM2Number or Name',
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

    return {

        init: function () {
            initializeDataTable();
            newPatientVisitsModalShow();
            newPatientsVisitsItemModalShow("click.show-patient-visits-modal", "a.add-measurement", "#new-measurement-modal");
            newPatientsVisitsItemSubmit("click.submit-patient-visits-item", "button.submit-patient-visit-item", "form#new-measurement-form", "div.measurement-form");
        }
    }
}();