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
                { "data": "visitDate", "name": "VisitDate", "autoWidth": true },
                { "data": "patientName", "name": "PatientName", "autoWidth": true },
                { "data": "rm2Number", "name": "RM2Number", "autoWidth": true },
                { "data": "examinations", "name": "Examinations", "autoWidth": true }
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
                    format: 'DD/MM/YYYY'
                });
                initalizeSelect2PatientSearch();
            });
        });
        //$.fn.modal.Constructor.prototype.enforceFocus = function () { };
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

    return {

        init: function () {
            initializeDataTable();
            newPatientVisitsModalShow();
        }
    }
}();