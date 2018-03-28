var MedicalTrials = function () {

    var initializeMedicalTrialsDataTable = function () {
        SimpleDataTable.initializeWithColumns("crf_trialsDT", "table#medical_trials_datatable", "MedicalTrial", [
            { "data": "id", "name": "ID", "autoWidth": true, "sortable": true },
            { "data": "name", "name": "Name", "autoWidth": true, "sortable": true },
            { "data": "description", "name": "Description", "autoWidth": true, "sortable": true },
            {
                "render": function (data, type, object, meta) {
                    return '<a class="btn btn-primary edit-crf-link disable-default" data-klass="MedicalTrial" style="display: none" data-role="Update Role" href="/MedicalTrials/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                        '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="MedicalTrial" data-tab="medical-trials"  data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/MedicalTrials/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                },
                "sortable": false,
                "width": 250,
                "autoWidth": false
            }
        ]);
    }

    return {
        init: function () {
            initializeMedicalTrialsDataTable();
        }
    }

}();