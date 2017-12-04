var Users = function () {

    var initUsersDataTable = function () {
            window.usersTable = $("#users-datatable").DataTable({
                "processing": true,
                "serverSide": true,
                "filter": true,
                "orderMulti": false,
                "initComplete": function (settings, json) {
                    Users.loadDataTableWithForCurrentUserRoles();
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
                            return '<a class="disable-default btn btn-warning user-edit" style="display: none" data-role="Update Role" href="/Users/Edit/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="disable-default btn btn-danger user-delete" style="display: none" data-role="Delete Role" href="/Users/Delete/' + user.id + '" data-id="' + user.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;' 
                        },
                        "sortable": false,
                        "width": 250,
                        "autoWidth": false
                    }
                ]
            });

        window.usersTable.on('draw.dt', function () {
            Users.loadDataTableWithForCurrentUserRoles();
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

    var replaceFormTargetOnTabClick = function () {
        $('[data-replace="true"]').click(function (e) {
            var $this = $(this),
                newurl = $this.attr('data-url'),
                formTarget = $this.attr('data-form-target');

        $(formTarget).attr("action", newurl);
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
                updateUser();
                initAjaxTab();
                replaceFormTargetOnTabClick();
            }).fail(function (data) {
                LoadingIndicator.hide();
                $("form#edit-user-form")[0].reset();
                $("div#edit-modal").modal("hide");
                alert("There was a problem reading this user information. Please contact database administrator");
            });
        });
    }

    var updateUser = function () {
        $(document).off("click.update-user").on("click.update-user", "button.update-user", function () {
            $("label.text-danger").remove();
            LoadingIndicator.show();
            $.ajax({
                url: $("form#edit-user-form").attr("action"),
                type: "POST",
                data: $("form#edit-user-form").serialize(),
                contentType: "application/x-www-form-urlencoded",
                dataType: 'json'
            }).done(function (data, textStatus) {
                LoadingIndicator.hide();
                if (textStatus === "success") {
                    if (data.errors) {
                        Patients.displayErrors(data.errors);
                    } else {
                        $("form#edit-user-form")[0].reset();
                        $("div#edit-user-modal").modal("hide");
                        window.usersTable.ajax.reload(function (json) {
                            Users.loadDataTableWithForCurrentUserRoles();
                        });
                    }
                }
            }).fail(function (data) {
                LoadingIndicator.hide();
                $("form#edit-user-form")[0].reset();
                $("div#edit-modal").modal("hide");
                alert("There was a problem saving this user. Please contact database administrator");
            });

        });
    }

    var bindOnDeletePatientClick = function () {
        $(document).off("click.user-delete").on("click.user-delete", "a.user-delete", function () {
            LoadingIndicator.show();
            var userId = $(this).data("id");
            var question = 'Are you sure you want to delete this user from database?';
            BootstrapDialog.confirm(question, function (result, dialog) {
                LoadingIndicator.hide();
                if (result) {
                    $.ajax({
                        url: "/Users/Delete/" + userId,
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        dataType: 'json'
                    }).done(function (data, textStatus) {
                        if (textStatus === "success") {
                            window.usersTable.ajax.reload(function (json) {
                                Users.loadDataTableWithForCurrentUserRoles();
                            });
                        }
                    }).always(function () {
                        LoadingIndicator.hide();
                    });
                }
            });
        });
    }

    var currentUserWithRoles = function () {
        $.getJSON("/Account/GetCurrentUserRolesAsync", function (response) {
            var container = $("div#current-user-roles");
            container.attr("data-id", response.user);
            container.attr("data-roles", response.roles.join(","));
            $.each(response.roles, function (index, role) {
                $("[data-role='" + role + "']").show();
                if (role === "Admin Role") {
                    $("[data-role]").show();
                }
            });
        });
    }

    return {

        loadDataTableWithForCurrentUserRoles: function () {
            currentUserWithRoles();
        },

        init: function () {
            initUsersDataTable();
            initAjaxTab();
            editUserModalShow();  
            bindOnDeletePatientClick();
        }
    }
}();