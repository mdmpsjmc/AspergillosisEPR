var Reports = function() {

    var initializeReportsDataTable = function () {
        SimpleDataTable.initializeWithColumns("reportsDT", "table#reports_datatable", "Report", [
            { "data": "id", "name": "ID", "autoWidth": true, "sortable": true },
            { "data": "reportType", "name": "ReportType", "autoWidth": true, "sortable": false },
            {
                "data": "startDate", "name": "StartDate", "autoWidth": true, "sortable": true,
                "render": function (data) {
                    return moment.unix(data).format("DD/MM/YYYY");
                },
            },
            {
                "data": "endDate", "name": "EndDate", "autoWidth": true, "sortable": true,
                "render": function (data) {
                    return moment.unix(data).format("DD/MM/YYYY");
                },
            },
            { "data": "patients", "name": "Patients", "autoWidth": true, "sortable": true },
            {
                "render": function (data, type, object, meta) {
                    return '<a class="btn btn-info" data-klass="Report" style="display: none" data-role="Read Role" href="/Reports/Generate/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-eye\' ></i>&nbsp;Show</a>&nbsp;' +
                        '<a class="btn btn-danger delete-link disable-default" data-what="item" data-klass="Report" data-warning="Are you sure you want to irreversibly remove this report?" style="display: none" data-role="Delete Role" href="/Reports/Delete/' + object.id + '" data-id="' + object.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                },
                "sortable": false,
                "width": 250,
                "autoWidth": false
            }
        ]);
    }

    var initializeWizard = function () {
        $("li.previous").remove();
        window.reportWizard = $('#report-wizard').bootstrapWizard({
            onNext: function (tab, navigation, index) {
                if ($("div#report-data").html().trim() === "") return false;
                if (index === 2) {
                    postWizardData(tab, navigation, index);
                    return false;
                }  
                if (index === 3) {
                    $("li.next.disabled").hide();
                }

            },  
            onBack: function (tab, navigation, index) {
                return false;
            },
            onTabClick: function(){
                return false;
            }
        });
    }    

    var onReportSelectChange = function () {
        $(document).off("change.report-type").on("change.report-type", "select#ReportTypeId", function () {
            $("div#report-data").html("");
            var currentReport = $(this).val();            
            if (currentReport !== "") {
                $.get("/Reports/Details?partialName=" + currentReport, function (responseHtml) {
                    $("div#report-data").html(responseHtml);
                    initializeSelectize();
                    initDatepickers();
                    $("li.next a.btn.btn-lg.txt-color-darken").click();
                });
            }       
            
        })
    }

    var initDatepickers = function () {
        $("input.datepicker").datetimepicker({
            format: "DD/MM/YYYY"
        })
    }

    var initializeSelectize = function () {
        $("select#PatientIds").selectize({
            valueField: 'id',
            labelField: 'fullName',
            searchField: ['description', 'fullName'],
            create: false,
            render: {
                option: function (item, escape) {
                    return markup(item);
                }
            },
            onChange: function (value) {
                $("select#PatientIds input[placeholder]").attr("style", "width: 100%;");
            },
            load: function (query, callback) {
                if (!query.length) return callback();
                $.ajax({
                    url: '/Select2Patients/Search?q=' + encodeURIComponent(query),
                    type: 'GET',
                }).done(function (res) {
                    callback(res);
                }).fail(function () {
                    callback();
                });
            }
        });
    }

    var markup = function (patient) {
        var markup = "<div class='select2-result-repository clearfix'>" +
            "<div class='select2-result-repository__avatar'><img src='" + patient.avatarUrl + "' style ='width: 100% !important;height: auto!important;border - radius: 2px !important;'/></div>" +
            "<div class='select2-result-repository__meta'>" +
            "<div class='select2-result-repository__title'>" + patient.fullName + "<br><strong>"+ patient.description + "</strong></div>";
        return markup;
    }

    var postWizardData = function (tab, navigation, index) {
        $(".has-error").removeClass("has-error");
        $("span.help-block").html("");
        return $.ajax({
            url: '/Reports/Create',
            type: 'POST',
            data: $("form#wizard-form").serialize() + "&PatientIds=" + $("select#PatientIds").val()
        }).then(function (response) {
            if (response.success) {
                $("a#show-report-link").data("report-id", response.id)
                window.reportWizard.bootstrapWizard('show', index);
            } else {
                showReportErors(response.errors);
                return false;
            }
            }).fail(function (response) {
                return false;
        });
    }

    var showReportErors = function (errors){
        for (var i = 0; i < Object.keys(errors).length; i++) {
            var field = Object.keys(errors)[i];
            var fieldCapitalized = field.charAt(0).toUpperCase() + field.slice(1);
            field = fieldCapitalized;
            var fieldError = errors[Object.keys(errors)[i]];
            var inputGroup = $("input#" + field + ", select#" + field).parent();
            var errorMessage = $("input#" + field + ", select#" + field).next().html(fieldError);
            inputGroup.addClass("has-error");
        }
    }
 
    return {
        init: function () {
            initializeWizard();
            onReportSelectChange();
            initializeReportsDataTable();
        }
    }
}();