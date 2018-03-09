var CaseReportForms = function () {

    var addNewPartial = function () {
        $(document).off("click.new-partial").on("click.new-partial", "a.add-form-field", function (e) {
            e.preventDefault();
            var visibleRow = $(this).data("visible-row");
            var insertIntoClass = $(this).data("insert-into-class");
            var index = $(visibleRow).length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                $(insertIntoClass).append(responseHtml);               
            });
        })
    }

    var showCRFModal = function () {
        $(document).off("click.asm").on("click.asm", "a.add-crf-item, a.edit-crf-link", function () {
            LoadingIndicator.show();
            $.get((this).href + "?klass=" + $(this).data("klass"), function (html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div.new-settings-modal").modal("show");
                $("select.select2").select2({
                    tags: true,
                    tokenSeparators: [',', ' '],
                    placeholder: "Type option"
                });
                $("form#new-case-report-form-option-group span.select2-container.select2-container--default").css("width", "100%");
            });
        });
    }

    var onOptionGroupSelectChange = function () {
        $(document).off("change").on("change", "select.option-group", function () {
            var optionGroupId = $(this).val();
            var requestUrl = "/CaseReportFormOptionGroups/Show/" + optionGroupId;
            $.get(requestUrl, function (responseHtml) {
                $("section.options").html(responseHtml);
                $("section.options").removeClass("hide");
                $("select[multiple='multiple']").multiSelect();
                if (optionGroupId === "") $("section.options").addClass("hide");
            });
        });
    }

    return {
        init: function () {
            addNewPartial();
            onOptionGroupSelectChange();
            showCRFModal();
   
            SimpleDataTable.initialize("crf_sectionsDT", "table#case_report_forms_sections_datatable", "CaseReportFormSection");
            SimpleDataTable.initializeWithColumns("crf_option_groupsDT", "table#case_report_forms_option_groups_datatable", "CaseReportFormOptionGroup", [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                { "data": "options", "name": "Options", "autoWidth": true },
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-primary edit-crf-link disable-default" data-klass="CaseReportFormOptionGroup" style="display: none" data-role="Update Role" href="/CaseReportFormOptionGroups/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="CaseReportFormOptionGroup" data-tab="crfs" data-child-tab="CaseReportFormOptionGroup" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportFormOptionGroups/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]);
            SimpleDataTable.initializeWithColumns("crf_categoriesDT", "table#case_report_forms_categories_datatable", "CaseReportFormCategory", [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-primary edit-crf-link disable-default" data-klass="CaseReportFormCategory"  data-tab="crfs"  style="display: none" data-role="Update Role" href="/CaseReportFormCategories/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="CaseReportFormCategory" data-tab="crfs" data-child-tab="CaseReportFormCategory" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportFormCategories/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]);
            SimpleDataTable.initializeWithColumns("crf_field_typesDT", "table#case_report_forms_field_types_datatable", "CaseReportFormFieldType", [
                { "data": "id", "name": "ID", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-primary edit-crf-link disable-default" data-klass="CaseReportFormFieldType"  data-tab="crfs"  style="display: none" data-role="Update Role" href="/CaseReportFormFieldTypes/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="CaseReportFormFieldType" data-tab="crfs" data-child-tab="CaseReportFormCategory" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportFormFieldTypes/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]);
        }
    }
}();