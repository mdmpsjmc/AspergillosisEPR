var SimpleDataTable = function() {
    
    return {

        initialize: function (referenceName, dataTableId, collection) {
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
                            return '<a class="btn btn-primary edit-side-effect edit-link disable-default" data-klass="' + collection + '" style="display: none" data-role="Update Role" href="/Radiology/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="btn btn-danger edit-side-effect delete-link disable-default" data-what="item" data-klass="' + collection + '" data-tab="radiology" data-child-tab="' + collection + '" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/Radiology/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
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
        },

        initializeWithColumns: function (referenceName, dataTableId, collection, columns) {
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
                "columns": columns
            });

            window[referenceName].on('draw.dt', function () {
                Users.loadDataTableWithForCurrentUserRoles();
            });
        }
    }

}();