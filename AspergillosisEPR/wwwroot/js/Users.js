var Users = function(){

    var initUsersDataTable = function () {
        $(document).ready(function () {
            window.usersTable = $("#users-datatable").DataTable({
                "processing": true,
                "serverSide": true,
                "filter": true,
                "orderMulti": false,
                "initComplete": function (settings, json) {
                },
                "ajax": {
                    "url": "/DataTableJson/LoadUsers",
                    "type": "POST",
                    "datatype": "json"
                },
                "columns": [
                    { "data": "id", "name": "ID", "autoWidth": true },                   
                    { "data": "firstName", "name": "FirstName", "autoWidth": true },
                    { "data": "lastName", "name": "LastName", "autoWidth": true },
                    { "data": "login", "name": "Login", "autoWidth": true },
                    { "data": "email", "name": "Email", "autoWidth": true },                    
                    {
                        "render": function (data, type, patient, meta) {
                            return '<a class="btn btn-info patient-details" href="/Users/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;' +
                                '<a class="btn btn-warning patient-edit" href="/Users/Edit/' + patient.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="btn btn-danger patient-delete" href="javascript:void(0)" data-id="' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                        },
                        "sortable": false
                    }
                ]
            });
        });
    }

    return {
        init: function () {
            initUsersDataTable();
        }
    }

}();