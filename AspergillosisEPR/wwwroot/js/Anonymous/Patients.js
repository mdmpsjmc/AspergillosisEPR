var AnonPatients = function () {

    var initializeDetailsModal = function () {
        $(document).off("click.patient-details-anon").on("click.patient-details-anon", "a.anon-patient-details", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var url = $(this).attr("href")
            $.get(url, function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#anon-details-modal").modal("show");
                var patientId = url.split("/")[3];
                $.getJSON("/Patients/" + patientId + "/Charts/SGRQ", function (response) {
                    Charts.sgrqChartFromResponse(response);
                });
                $.getJSON("/Patients/" + patientId + "/Charts/Immunology", function (response) {
                    $("div#ig-charts").html(""); //clears hidden charts div. 
                    Charts.igChartsFromResponse(response, true);
                });
            });
        });
    }

    return {
        init: function () {
            initializeDetailsModal();
            SimpleDataTable.initializeWithColumns("anonPatientsDT", "table#anonymous_patients_datatable", "Patient", [
                { "data": "id", "name": "ID", "autoWidth": true, sortable: true },
                { "data": "initials", "name": "Initials", "autoWidth": true, "sortable": true},
                { "data": "primaryDiagnosis", "name": "PrimaryDiagnosis", "autoWidth": true, "sortable": true },
                {
                    "render": function (data, type, patient, meta) {
                        return '<a class="btn btn-info anon-patient-details disable-default" data-role="Anonymous Role" href="/AnonymousPatients/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;';
                    },
                    "sortable": false,
                    "width": 350,
                    "autoWidth": false
                }
              ]
            );
        }
    }
}();