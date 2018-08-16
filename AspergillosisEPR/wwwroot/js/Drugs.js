var Drugs = function () {

    var initDrugsDatatable = function () {
        SimpleDataTable.initializeWithColumns("drugsDT", "table#drugs_datatable", "drug", [
            { "data": "id", "name": "ID", "autoWidth": true, "sortable": true },
            { "data": "name", "name": "Name", "autoWidth": true, "sortable": true },                   
            {
                "render": function (data, type, object, meta) {
                    return '<a class="btn btn-primary edit-diagnosis-drug edit-link disable-default" data-klass="Drug" style="display: none" data-role="Read Role" href="/drugs/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-eye\' ></i>&nbsp;Edit</a>&nbsp;' +
                        '<a class="btn btn-danger edit-diagnosis-drug delete-link disable-default" data-what="item" data-tab="drugs" data-klass="Drug" data-warning="Are you sure you want to irreversibly remove this drug?" style="display: none" data-role="Delete Role" href="/Drugs/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                },
                "sortable": false,
                "width": 250,
                "autoWidth": false
            }
        ]);
    }

    return {
        init: function () {
            initDrugsDatatable();
        }
    }

}();