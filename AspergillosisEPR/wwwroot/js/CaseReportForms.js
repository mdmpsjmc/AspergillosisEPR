var CaseReportForms = function () {

    return {
        init: function () {
            SimpleDataTable.initialize("crf_field_typesDT", "table#case_report_forms_field_types_datatable", "CaseReportFormFieldType");
        }
    }
}();