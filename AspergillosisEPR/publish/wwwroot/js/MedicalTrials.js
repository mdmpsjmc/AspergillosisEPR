var MedicalTrials = function () {

    var showTrialDetails = function () {
        $(document).off("click.trial-details").on("click.trial-details", "a.show-trial-details", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get((this).href, function(html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div#trial-details-modal").modal("show");
            });
        });
    }

    var initializeMedicalTrialsDataTable = function () {
        SimpleDataTable.initializeWithColumns("crf_trialsDT", "table#medical_trials_datatable", "MedicalTrial", [
            { "data": "id", "name": "ID", "autoWidth": true, "sortable": true },
            { "data": "name", "name": "Name", "autoWidth": true, "sortable": true },
            { "data": "trialType", "name": "TrialType", "autoWidth": true, "sortable": true },
            { "data": "number", "name": "Number", "autoWidth": true, "sortable": true },
            { "data": "principalInvestigator", "name": "PrincipalInvestigator", "autoWidth": true, "sortable": true },
            {
                "render": function (data, type, object, meta) {
                    return '<a class="btn btn-info details-link disable-default show-trial-details" data-klass="MedicalTrial" data-tab="medical-trials" style="display: none" data-role="Read Role" href="/MedicalTrials/Details/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-eye\' ></i>&nbsp;Details</a>&nbsp;' +
                        '<a class="btn btn-primary edit-crf-link disable-default" data-klass="MedicalTrial" style="display: none" data-role="Update Role" href="/MedicalTrials/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                        '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="MedicalTrial" data-tab="medical-trials"  data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/MedicalTrials/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                },
                "sortable": false,
                "width": 250,
                "autoWidth": false
            }
        ]);
    }

    var initializePrincipalInvestigatorsDataTable = function () {
        SimpleDataTable.initializeWithColumns("crf_investigatorsDT", "table#principal_investigators_datatable", "PrincipalInvestigator", [
            { "data": "id", "name": "ID", "autoWidth": true, "sortable": true },
            { "data": "title", "name": "Title", "autoWidth": true, "sortable": true },
            { "data": "firstName", "name": "FirstName", "autoWidth": true, "sortable": true },
            { "data": "lastName", "name": "LastName", "autoWidth": true, "sortable": true },
            {
                "render": function (data, type, object, meta) {
                    return '<a class="btn btn-primary edit-crf-link disable-default" data-klass="PrincipalInvestigator" style="display: none" data-role="Update Role" href="/PrincipalInvestigators/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                        '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="PrincipalInvestigator" data-tab="medical-trials"  data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/PrincipalInvestigators/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                },
                "sortable": false,
                "width": 250,
                "autoWidth": false
            }
        ]);
    }

    return {
        init: function () {
            showTrialDetails();
            initializeMedicalTrialsDataTable();
            initializePrincipalInvestigatorsDataTable();
        }
    }

}();