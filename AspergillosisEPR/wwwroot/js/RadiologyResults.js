var RadiologyResults = function(){
    var initRadiologyResultsDataTable = function () {
        window.radiologyResultsDT = $("#radiology_results_datatable").DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "serverSide": true,
            "filter": true,
            "responsive": true, 
            "orderMulti": false,
            rowGroup: {
                dataSrc: "name",
                startRender: function (rows, group) {
                    var ids = [];
                    $.each(rows.data(), function (index, el) {
                        ids.push(el.id);
                    });
                    return $('<tr/>')
                        .append('<td>' + group + '</td>')
                        .append('<td>' + '<a class="btn btn-primary edit-radiology-result edit-link disable-default" data- role="Update Role" href="/RadiologyResults/Edit/' + ids.join(',') + '" data- id="' + ids.join(',') + '" > <i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>' + '&nbsp;<a class="btn btn-danger edit-radiology-result delete-link disable-default" data-tab="radiology-results" data-warning="All patient diagnoses related to this diagnosis will be also irreversibly lost from database if you remove this item" style="display: none" data-role="Delete Role" href="/radiologyResults/Delete/' + ids.join(',') + '" data-id="' + ids.join(',') + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;' + '</td>');
                }
            },
            "initComplete": function (settings, json) {
                Users.loadDataTableWithForCurrentUserRoles();
            },
            "ajax": {
                "url": "/DataTableRadiologyResults/Load",
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "option", "name": "Option", "autoWidth": true, "sortable": false }                
            ]
        });  

        window.radiologyResultsDT.on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
        });
    }

    return {
        init: function() {
            initRadiologyResultsDataTable();        
        }
    }

}();