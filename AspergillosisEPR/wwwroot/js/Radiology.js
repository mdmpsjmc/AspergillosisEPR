var Radiology = function () {

    var initializeDataTable = function (referenceName, dataTableId, collection) {
        window[referenceName] = $(dataTableId).DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "serverSide": true,
            "filter": true,
            "responsive": true,
            "orderMulti": false,
            "pageLength": 50,
            "order": [[1, "asc"]],
            "initComplete": function (settings, json) {
                Users.loadDataTableWithForCurrentUserRoles();
            },
            "ajax": {
                "url": $(dataTableId).data("url"),
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-primary edit-side-effect edit-link disable-default" style="display: none" data-role="Update Role" href="/' + collection + '/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger edit-side-effect delete-link disable-default" data-tab="side-effects" data-warning="All patient diagnoses related to this diagnosis will be also irreversibly lost from database if you remove this item" style="display: none" data-role="Delete Role" href="/' + collection + '/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]
        });  

        window[referenceName].on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
        });
    }

    return {
        init: function () {
            initializeDataTable("findingsDT", "table#findings_datatable", "Finding");
            initializeDataTable("chestLocationsDT", "table#chest_locations_datatable", "ChestLocation");
            initializeDataTable("chestDistributionsDT", "table#chest_distributions_datatable", "ChestDistribution");
            initializeDataTable("gradesDT", "table#grades_datatable", "Grade");
            initializeDataTable("responsesDT", "table#responses_datatable", "Response");
            initializeDataTable("radiologyTypesDT", "table#radiology_types_datatable", "RadiologyType");

        }
    }
}();