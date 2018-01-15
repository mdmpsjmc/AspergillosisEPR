var Search = function () {

    var initializeSearch = function () {
        $("form#advanced-search-form").on("submit", function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            var errors = false;
            $.each($('section.search-value input:text'), function (index, searchField) {
                if ($(searchField).val() === "") {
                    errors = true;
                }
            });
            if (errors) {
                alert("Search field value cannot be empty");
                return;
            } else {
                LoadingIndicator.show();
                $.getJSON("/PatientSearches/Create?" + data, function (jsonResponse) {
                    LoadingIndicator.hide();
                    $("section.search-results.hide").removeClass("hide");
                    initPatientsDataTable(jsonResponse);
                });
            }
        });
    }

    var initPatientsDataTable = function (tableData) {
        $("#search_results_datatable").DataTable().destroy();
        window.patientsTable = $("#search_results_datatable").DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            "processing": true,
            "filter": false,
            "orderMulti": false,
            "pageLength": 50,
            buttons: [
                'excel', 'pdf', 'print'
            ],
            "initComplete": function (settings, json) {
                Patients.publicSetup()
            },
            data: tableData,
            "columns": [
                { "data": "rM2Number", "name": "RM2Number", "autoWidth": true },
                { "data": "firstName", "name": "FirstName", "autoWidth": true },
                { "data": "lastName", "name": "LastName", "autoWidth": true },
                { "data": "gender", "name": "Gender", "autoWidth": true },
                {
                    "data": "dob", "name": "DOB", "autoWidth": true,
                    "render": function (data) {
                        return moment(data).format("MM/DD/YYYY");
                    }
                },
                {
                    "render": function (data, type, patient, meta) {
                        return '<a class="btn btn-info patient-details"  style="display: none" data-role="Read Role" href="/Patients/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;' +
                            '<a class="btn btn-warning patient-edit" style="display: none" data-role="Update Role" href="/Patients/Edit/' + patient.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                            '<a class="btn btn-danger patient-delete" style="display: none" data-role="Delete Role" href="javascript:void(0)" data-id="' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                    },
                    "sortable": false,
                    "width": 250,
                    "autoWidth": false
                }
            ]
        });


        window.patientsTable.on('draw.dt', function () {
            Patients.publicSetup();
        }); 

        window.patientsTable.buttons().container()
            .appendTo($('.col-sm-6:eq(0)', window.patientsTable.table().container()));
    }

    var onAddSearchCriteriaClick = function () {
        $(document).off("click.add-criteria").on("click.add-criteria", "a.add-search-criteria", function (e) {
            $(this).remove();
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.search-criteria-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("fieldset:last").after(responseHtml);
                $("fieldset:last section:first").removeClass("hide");
                setupSearchFormLayout();
            });
        });
    }

    var onSearchClassSelectChange = function () {
        $(document).off("change.select-searchclass").on("change.select-searchclass", "select.criteria-class", function () {
            var selectedValue = $(this).val();
            var requestUrl = $(this).data("url");
            var index = $("div.search-criteria-row:visible").length - 1;
            var nextSelect = $(this).parents("section").next("section").children("label");
            var fieldSelect = $($(this).parents("section").next("section").find("select")[0]);
            $.get(requestUrl + "?searchClass=" + selectedValue + "&index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                fieldSelect.html(responseHtml);
                $(fieldSelect).val($(fieldSelect).find("option:first").attr("value"));
                $(fieldSelect).trigger("change")
            });
        });

    }

    var onFieldSelectChangeAddDatepicker = function () {
        $(document).off("change.select-field").on("change.select-field", "select.criteria-field", function () {
            var selectedText = $(this).find("option:selected").text();
            var searchField = $(this).parents("section").next("section").next("section").find("input[type='text']");
            searchField.val("");
            var compareSelect = $(this).parents("section").next("section");
            var index = $("div.search-criteria-row:visible").length - 1;
            var fieldType = selectedText.match(/Date/) !== null ? "Date" : "String";
            var partialRequestUrl = "/Partials/SearchCriteria?index=" + index + "&fieldType=" + fieldType;
            updateSearchCriteria(this);
            $.get(partialRequestUrl, function (htmlResponse) {
                compareSelect.find("label.select").html(htmlResponse);
                if (selectedText.match(/Date/) !== null) {
                    $(searchField[0]).addClass("datepicker");
                    $('input.datepicker').datetimepicker({
                        format: 'YYYY-MM-DD'
                    });
                } else if (selectedText.match(/Date/) === null) {
                    compareSelect.find("select option[value='SmallerThan']").remove();
                    compareSelect.find("select option[value='GreaterThan']").remove();
                    if (searchField.hasClass("datepicker")) {
                        searchField.removeClass("datepicker");
                        searchField.datetimepicker("destroy");
                    }
                }
            });
        });
    }

    var updateSearchCriteria = function (select) {
        var selectedValue = $(select).find("option:selected").val();
        var isSelectField = ($.inArray("Select", selectedValue.split(".")) > 0)
        var compareSection = $("section.criteria-match");
        var index = $(select).data().index;
        if (index === undefined) {
            index = $("div.search-criteria-row:visible").length - 1;
        }
        var searchValueSection = $(select).parents("section").next("section").next("section");
        var originalHtml = '<label class=\"input\"><input name=\"PatientSearchViewModel[' + index + '].SearchValue\" id=\"PatientSearchViewModel_' + index + '__SearchValue\" type=\"text\" placeholder=\"Search Value\"></label>';
        if (isSelectField) {
            var klassName = selectedValue.split(".")[1];
            var field = selectedValue.split(".")[2];
            var selectFieldRequestUrl = "/Partials/SearchSelectPartial?index=" + index + "&klass=" + klassName + "&field=" + field;


            $.get(selectFieldRequestUrl, function (htmlResponse) {
                $(compareSection[index]).hide();
                searchValueSection.html(htmlResponse);
            });
        } else {
            searchValueSection.html(originalHtml);
            $(compareSection[index]).show();
        }
        var searchField = $('#PatientSearchViewModel_' + index + '__SearchValue');
        var selectedText = $(select).find("option:selected").html();
        if (selectedText.match(/Date/) !== null) {
            $(searchField[0]).addClass("datepicker");
            $('input.datepicker').datetimepicker({
                format: 'YYYY-MM-DD'
            });
        } else if (selectedText.match(/Date/) === null) {
            if (searchField.hasClass("datepicker")) {
                searchField.removeClass("datepicker");
                searchField.datetimepicker("destroy");
            }
        }
    }

    var setupSearchFormLayout = function () {
        $("a.remove-search-criteria:first").hide();
        $("a.remove-search-criteria:not(:last)").hide();
    }

    var onSearchCriteriaRemove = function () {
        $(document).off("click.crit-remove").on("click.crit-remove", "a.remove-search-criteria", function () {
            var previousRowFieldset = $($(this).parents("fieldset").prev()[0]);
            var addCriteriaButton = $(this).prev("a").parent();
            $($(this).parents("fieldset").prev()[0]).find("section.buttons").html(addCriteriaButton.html());
            $(this).parents("fieldset")[0].remove();
            setupSearchFormLayout();
        });
    }
   
    return {
        init: function () {
            initializeSearch();
            onAddSearchCriteriaClick();
            onSearchClassSelectChange();    
            onFieldSelectChangeAddDatepicker();
            setupSearchFormLayout();
            onSearchCriteriaRemove();
        }
    }
}();