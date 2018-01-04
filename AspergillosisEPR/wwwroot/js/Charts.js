var Charts = function () {

    var initializeSGRQChart = function () {
        $(document).off("click.show-sgrq").on("click.show-sgrq", "a.show-sgrq-chart", function () {
            var patientId = $(this).data("id");
            var requestUrl = $(this).attr("href");
            $.getJSON(requestUrl, function (response) {
                var totalData = [], symptomsData = []; activityData = [], impactData = [];
                var labels = [];
                var modalId = "div#sgrq-chart-" + patientId;
                $(modalId).modal("show");
                $(modalId).on('shown.bs.modal', function (e) {
                    $('.modal-dialog', this).addClass('focused');
                    $('body').addClass('modalprinter');
                    $.each(response, function (index, chartItem) {
                        var stringDate = moment.unix(chartItem.dateTaken).format("ll");
                        totalData.push({ x: stringDate, y: chartItem.totalScore });
                        symptomsData.push({ x: stringDate, y: chartItem.symptomScore });
                        activityData.push({ x: stringDate, y: chartItem.activityScore });
                        impactData.push({ x: stringDate, y: chartItem.impactScore });
                        labels.push(stringDate);
                    });
                    var chartData = {
                        labels: labels,
                        datasets: [
                            {
                                label: "Total Score",
                                backgroundColor: "rgba(255,99,132,0.2)",                                
                                borderColor: "rgba(255,99,132,1)",
                                borderWidth: 1,
                                hoverBackgroundColor: "rgba(255,99,132,0.4)",
                                hoverBorderColor: "rgba(255,99,132,1)",
                                data: totalData
                            },
                            {
                                label: "Symptom Score",
                                backgroundColor: "rgba(124,252,0,0.2)",
                                borderColor: "rgb(0,128,0)",
                                borderWidth: 1,
                                hoverBackgroundColor: "rgba(124,252,0,0.4)",
                                hoverBorderColor: "rgb(173,255,47)",
                                data: symptomsData
                            },
                            {
                                label: "Activity Score",
                                backgroundColor: "rgba(135,206,250, 0.2)",
                                borderColor: "rgb(0,0,255)",
                                borderWidth: 1,
                                hoverBackgroundColor: "rgba(135,206,250, 0.4)",
                                hoverBorderColor: "rgb(0,0,139)",
                                data: activityData
                            },
                            {
                                label: "Impact Score",
                                backgroundColor: "rgba(218,165,32, 0.2)",
                                borderColor: "rgb(160,82,45)",
                                borderWidth: 1,
                                hoverBackgroundColor: "rgba(218,165,32, 0.4)",
                                hoverBorderColor: "rgb(139,69,19)",
                                data: impactData
                            }
                        ]
                    };
                    var ctx = document.getElementById("sgrq-chart-content");
                    var stackedLine = new Chart(ctx, {
                        type: 'line',
                        data: chartData,
                        
                    });
                }).on('hidden.bs.modal', function () {
                    $('.modal-dialog', this).removeClass('focused');
                    $('body').removeClass('modalprinter');
                });
            });            
        });
        $('div.modal-backdrop.in').on("click", function(){
            $(this).remove();
        });
    }

    return {
        init: function () {
            initializeSGRQChart();
        }
    }
}();