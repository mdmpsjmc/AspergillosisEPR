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
        $(document).off("click.asm").on("click.asm", "a.add-crf-item, a.edit-crf-link", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get((this).href + "?klass=" + $(this).data("klass"), function(html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div.new-settings-modal").modal("show");
                $("select.select2").select2({
                    tags: true,
                    tokenSeparators: [',', ' '],
                    placeholder: "Type option"
                });
                $("select[multiple='multiple']").multiSelect();
                $("form#new-case-report-form-option-group span.select2-container.select2-container--default").css("width", "100%");
            });
        });
    }

    var showRenderedSection = function () {
        $(document).off("click.asm").on("click.srs", "a.show-crf-section, a.show-crf-modal", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            $.get((this).href + "?klass=" + $(this).data("klass"), function (html) {
                LoadingIndicator.hide();
                $("div#modal-container").html(html);
                $("div#render-section-modal, div#render-crf-modal").modal("show");
                $('input.datepicker').datetimepicker({
                    format: 'DD/MM/YYYY'
                });
                $("select[multiple='multiple']").multiSelect();
            });
        });
    }

    var onOptionGroupSelectChange = function () {
        $(document).off("change").on("change", "select.option-group", function () {
            var optionGroupId = $(this).val();
            var currentIndex = $(this).data("index");
            var requestUrl = "/CaseReportFormOptionGroups/Show/" + optionGroupId + "?index=" + currentIndex;
            $.get(requestUrl, function (responseHtml) {
                $("section.options[data-index='" + currentIndex + "']").html(responseHtml);
                $("section.options[data-index='" + currentIndex + "']").removeClass("hide");
                $("select[multiple='multiple']").multiSelect();
                if (optionGroupId === "") $("section.options[data-index='" + currentIndex + "']").addClass("hide");
            });
        });
    }

    var onPatientCaseReportFormSelectChange = function () {
        $(document).off("change").on("change", "select#patient-case-report-form", function () {
            var caseReportFormId = $(this).val();
            var formIndex = $(this).data("index");
            var requestUrl = "/CaseReportForms/Patient/" + caseReportFormId + "?index=" + formIndex;
            var nextDivContainer = $(this).parents("section").next();
            $.get(requestUrl, function (responseHtml) {
                if (nextDivContainer !== null && nextDivContainer !== undefined && nextDivContainer.length > 0) {
                    nextDivContainer.html(responseHtml);
                }
                $("div#case-report-form").removeClass("hide");
                $("select[multiple='multiple']").multiSelect();
                if (caseReportFormId === "") $("div#case-report-form").addClass("hide");
                $('input.datepicker').datetimepicker({
                    format: 'DD/MM/YYYY'
                });
            });
        });
    }

    return {
        showCRFModal: function () {
            showCRFModal();
        },

        onOptionGroupSelectChange: function () {
            onOptionGroupSelectChange();
        },

        onPatientCaseReportFormSelectChange: function () {
            onPatientCaseReportFormSelectChange();
        },

        init: function () {
            addNewPartial();
            onOptionGroupSelectChange();

            showCRFModal();
            Patients.deletePartialFromPopup();
            Patients.deleteDbPartialFromPopup();
            showRenderedSection();
            onPatientCaseReportFormSelectChange();
            SimpleDataTable.initializeWithColumns("crf_sectionsDT", "table#case_report_forms_sections_datatable", "CaseReportFormSection", [
                { "data": "name", "name": "Name", "autoWidth": true, "sortable": false},               
                { "data": "fieldNames", "name": "FieldNames", "autoWidth": true, "sortable": false }, 
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-info details-link disable-default show-crf-section" data-what="item" data-klass="CaseReportFormSection" data-tab="crfs" data-child-tab="CaseReportFormSection" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportFormSections/Show/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-eye\' ></i>&nbsp;Show</a>&nbsp;' +
                               '<a class="btn btn-primary edit-crf-link disable-default" data-klass="CaseReportFormSection" style="display: none" data-role="Update Role" href="/CaseReportFormSections/Edit/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                               '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="CaseReportFormSection" data-tab="crfs" data-child-tab="CaseReportFormSection" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportFormSections/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';                        
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]);
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
            SimpleDataTable.initializeWithColumns("case_report_formDT", "table#case_report_forms_datatable", "CaseReportForm", [
                { "data": "itemId", "name": "ItemId", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                { "data": "categoryName", "name": "CategoryName", "autoWidth": true },
                { "data": "sectionsNames", "name": "SectionsNames", "autoWidth": true },
                { "data": "fieldsNames", "name": "FieldsNames", "autoWidth": true },
                {
                    "render": function (data, type, object, meta) {
                        return '<a class="btn btn-info details-link disable-default show-crf-section" data-what="item" data-klass="CaseReportForms" data-tab="crfs" data-child-tab="CaseReportFormSection" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportForms/Show/' + object.itemId + '" data-id="' + object.itemId + '"><i class=\'fa fa-eye\' ></i>&nbsp;Show</a>&nbsp;' +
                            '<a class="btn btn-primary edit-crf-link disable-default" data-klass="CaseReportForm"  data-tab="crfs"  style="display: none" data-role="Update Role" href="/CaseReportForms/Edit/' + object.itemId + '" data-id="' + object.itemId + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="CaseReportForm" data-tab="crfs" data-child-tab="CaseReportForm" data-warning="All patient information related to this items will be  irreversibly lost from database if you remove it" style="display: none" data-role="Delete Role" href="/CaseReportForms/Delete/' + object.itemId + '" data-id="' + object.itemId + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;'
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]);
        }
    }
}();