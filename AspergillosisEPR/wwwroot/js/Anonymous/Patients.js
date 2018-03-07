var AnonPatients = function () {


    return {
        init: function () {
            SimpleDataTable.initializeWithColumns("anonPatientsDT", "table#anonymous_patients_datatable", "Patient", [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "initials", "name": "Initials", "autoWidth": true, "sortable": false },
                { "data": "primaryDiagnosis", "name": "PrimaryDiagnosis", "autoWidth": true, "sortable": false },
                {
                    "render": function (data, type, patient, meta) {
                                 return '<a class="btn btn-info anon-patient-details" data-role="Anonymous Role" href="/AnonymousPatients/Details/' + patient.id +
                                              + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;';
                    }
                }]
            );
        }
    }
}();