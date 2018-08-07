var Reports = function() {
    
    var initializeWizard = function() {
        window.reportWizard = $('#report-wizard').bootstrapWizard({
            onNext: function (tab, navigation, index) {
                if ($("div#report-data").html().trim() === "") return false;
                if (index === 2) {
                    return postWizardData();
                }
            },  
            onBack: function (tab, navigation, index) {
                if ($("div#report-data").html().trim() === "") return false;
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
                $.get("/Reports/Details/" + currentReport, function (responseHtml) {
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
        $("select#Patients").selectize({
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
                $("select#Patients input[placeholder]").attr("style", "width: 100%;");
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

    var postWizardData = function () {
        $.ajax({
            url: '/Reports/Create',
            type: 'POST',
            async: false, 
            data: $("form#wizard-form").serialize()
        }).done(function (response) {
            console.log(response);
            return response.success;
        }).fail(function () {
            return false;
        });
    }
 
    return {
        init: function () {
            initializeWizard();
            onReportSelectChange();
        }
    }


}();