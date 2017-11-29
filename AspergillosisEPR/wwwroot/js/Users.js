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
                    { "data": "userName", "name": "UserName", "autoWidth": true },
                    { "data": "email", "name": "Email", "autoWidth": true },   
                    { "data": "roles", "name": "Roles", "autoWidth": true },             
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

    var initAjaxTab = function () {
        $('[data-toggle="tabajax"]').click(function (e) {
            var $this = $(this),
                loadurl = $this.attr('href'),
                targ = $this.attr('data-target');

            $.get(loadurl, function (data) {
                $(targ).html(data);
            });

            $this.tab('show');
            return false;
        });
    }

    return {
        init: function () {
            initUsersDataTable();
            initAjaxTab();
        }
    }

}();