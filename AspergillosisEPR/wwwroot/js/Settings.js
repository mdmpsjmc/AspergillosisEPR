var Settings = function () {

    var disableDefaultOnClickAction = function () {
        $(document).on("click", "a.disable-default", function () {
            event.preventDefault();
        });
    }

    var showSettingsModal = function () {
        $(document).off("click.add-settings-modal").on("click.add-settings-modal", "a.add-settings-item", function () {
            LoadingIndicator.show();
            $.get((this).href + "?klass=" + $(this).data("klass"), function(html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div.new-settings-modal").modal("show");

                $("div.new-settings-modal").on('shown.bs.modal', function (e) {
                    $("select.select2").select2({
                        minimumResultsForSearch: -1,
                        placeholder: function () {
                            $(this).data('placeholder');
                        }
                    });    
                });                
            });
        });
    }

    var showSettingsEditModal = function () {
        $(document).off("click.edit-settings-modal").on("click.edit-settings-modal", "a.edit-link", function () {
            LoadingIndicator.show();
            $.get((this).href, function (html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div.edit-settings-modal").modal("show");
            });
        });
    }

    var goToTab = function () {
        var hash = document.location.hash;
        var prefix = "";
        if (hash) {
            $('.nav-tabs a[href="' + hash.replace(prefix, "") + '"]').tab('show');
        }

        $('.nav-tabs a').on('shown.bs.tab', function (e) {
            window.location.hash = e.target.hash.replace("#", "#" + prefix);
        });
    }

    var onSettingsItemSubmit = function () {
        $(document).off("click.save-settings-item").on("click.save-settings-item", "button.submit-settings-item", function () {
            LoadingIndicator.show();
            var loadTab = $(this).data("tab");
            $("label.text-danger").remove();
            $.ajax({
                url: $("form.settings-form").attr("action"),
                type: "POST",
                data: $("form.settings-form").serialize(),
                contentType: "application/x-www-form-urlencoded",
                dataType: 'json'
            }).done(function (data, textStatus) {
                LoadingIndicator.hide();
                if (textStatus === "success") {
                    if (data.errors) {
                        displayErrors(data.errors);
                    } else {
                        $("form.settings-form")[0].reset();
                        $("div.new-settings-modal").modal("hide");
                        window.location.hash = loadTab;
                        window.location.reload();
                    }
                }
            }).fail(function (data) {
                LoadingIndicator.hide();
                $("form.settings-form")[0].reset();
                $("div.new-settings-modal").modal("hide");
                alert("There was a problem saving this information. Please contact administrator");
            });
        });
    }

    var displayErrors = function (errors) {
        for (var i = 0; i < Object.keys(errors).length; i++) {
            var field = Object.keys(errors)[i];
            var htmlCode = "<label for='" + field + "' class='text-danger'></label>";
            var fieldError = errors[Object.keys(errors)[i]];
            var fieldInput = $("input[name='" + capitalizeFirstLetter(field) + "']");
            $(htmlCode).html(fieldError).appendTo(fieldInput.parent());
        }
    }

    var capitalizeFirstLetter = function(string) {
        return string.charAt(0).toUpperCase() + string.slice(1);
    }

    var bindOnDeleteSettingsItemClick = function () {
        $(document).off("click.delete-link").on("click.delete-link", "a.delete-link", function () {
            LoadingIndicator.show();
            var deleteUrl = $(this).attr("href");
            var whatToDelete = $(this).data("what");
            var warningMessage = $(this).data("warning");
            window.openedTab = $(this).data("tab");
            var question = 'Are you sure you want to irreversibly delete this ' + whatToDelete +' and all related data?<br><br><div class=\'alert alert-danger\' style=\'padding: 10px\'>'+warningMessage+ '</div>';
            BootstrapDialog.confirm(question, function (result, dialog) {
                LoadingIndicator.hide();

                if (result) {
                    $.ajax({
                        url: deleteUrl,
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        dataType: 'json'
                    }).done(function (data, textStatus) {
                        if (textStatus === "success") {
                            window.location.hash = window.openedTab;
                            window.location.reload();
                            if (window.diagnosisTypesDataTable !== undefined) {
                                window.diagnosisTypesDataTable.reload();
                            }
                        }
                    }).always(function () {
                        LoadingIndicator.hide();
                    });
                }
            });
        });
    }
         
    return {
        init: function () {
            disableDefaultOnClickAction();
            showSettingsModal();
            goToTab();
            onSettingsItemSubmit();
            showSettingsEditModal();
            bindOnDeleteSettingsItemClick();
        }
    }

}();