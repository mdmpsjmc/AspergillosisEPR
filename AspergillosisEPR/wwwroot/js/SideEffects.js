var SideEffects = function(){
    var initSideEffectsDataTable = function () {
        window.sideEffectsDT = $("#side_effects_datatable").DataTable({
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
                "url": "/DataTableSideEffects/Load",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                {
                    "render": function (data, type, user, meta) {
                        return '<a class="btn btn-primary edit-side-effect edit-link disable-default" style="display: none" data-role="Update Role" href="/SideEffects/Edit/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger edit-side-effect delete-link disable-default" data-tab="side-effects" data-warning="All patient diagnoses related to this diagnosis will be also irreversibly lost from database if you remove this item" style="display: none" data-role="Delete Role" href="/SideEffects/Delete/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]
        });  

        window.sideEffectsDT.on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
        });
    }

    return {
        init: function() {
            initSideEffectsDataTable();        
        }
    }

}();