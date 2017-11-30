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
                    { "data": "roles", "name": "Roles", "autoWidth": true, "sortable": false },             
                    {
                        "render": function (data, type, user, meta) {
                            return '<a class="disable-default btn btn-warning user-edit" href="/Users/Edit/' + user.id + '" data-id="' + user.id +'"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="disable-default btn btn-danger user-delete" href="javascript:void(0)" data-id="' + user.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
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

    var editUserModalShow = function () {
        $(document).off("click.launch-edit-user-modal").on("click.launch-edit-user-modal", "a.user-edit", function () {
            LoadingIndicator.show();
            var userId = $(this).data("id");
            $.get("/Users/Edit/"+userId, function (responseHtml) {
                LoadingIndicator.hide();
                $("div#modal-container").html(responseHtml);
                $("div#edit-user-modal").modal("show");
                $("select.select2").select2();
            });
        });
    }

    return {
        init: function () {
            initUsersDataTable();
            initAjaxTab();
            editUserModalShow();            
        }
    }

}();