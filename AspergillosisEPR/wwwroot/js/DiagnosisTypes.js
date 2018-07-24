var DiagnosisTypes = function(){
    var initDiagnosisTypesDataTable = function () {
        window.diagnosisTypesDataTable = $("#diagnosis_types_datatable").DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "serverSide": true,
            "filter": true,
            "orderMulti": false,
            "pageLength": 100,
            "order": [[1, "asc"]],
            "initComplete": function (settings, json) {
                Users.loadDataTableWithForCurrentUserRoles();
            },
            "ajax": {
                "url": "/DataTableDiagnosisTypes/Load",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                { "data": "shortName", "name": "ShortName", "autoWidth": true, "orderable": true },
                {
                    "render": function (data, type, user, meta) {
                        return '<a class="btn btn-primary edit-diagnosis-type edit-link disable-default" style="display: none" data-role="Update Role" href="/DiagnosisTypes/Edit/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger edit-diagnosis-type delete-link disable-default" data-tab="diagnosis-types" data-warning="All patient diagnoses related to this diagnosis will be also irreversibly lost from database if you remove this item" style="display: none" data-role="Delete Role" href="/DiagnosisTypes/Delete/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]
        });  

        window.diagnosisTypesDataTable.on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
        }); 
    }

    return {
        init: function() {
            initDiagnosisTypesDataTable();        
        }
    }

}();