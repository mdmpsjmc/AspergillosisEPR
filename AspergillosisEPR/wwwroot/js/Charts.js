var Charts = function () {

    var initializeChart = function (binding, button, modalId, chartName, chartForPdf) {
        $(document).off(binding).on(binding, button, function () {
            var patientId = $(this).data("id");
            var requestUrl = $(this).attr("href");
            var chartModal = modalId + patientId;
            requestAndDrawChart(patientId, requestUrl, chartModal, chartName, chartForPdf);
        });
        $('div.modal-backdrop.in').on("click", function () {
            $(this).remove();
        });
    }

    var requestAndDrawChart = function (patientId, requestUrl, modalId, chartName, chartForPdf) {
        $.getJSON(requestUrl, function (response) {
            $(modalId).modal("show");
            $(modalId).on('shown.bs.modal', function (e) {
                $('.modal-dialog', this).addClass('focused');
                $('body').addClass('modalprinter');
                chartFromResponse(chartName, response, chartForPdf);
            }).on('hidden.bs.modal', function () {
                $('.modal-dialog', this).removeClass('focused');
                $('body').removeClass('modalprinter');
            });
        });   
    }

    var chartFromResponse = function (chartName, response, chartForPdf) {
        switch (chartName) {
            case "sgrq":
                sgrqChartFromResponse(response);
                break;
            case "ig":
                igChartsFromResponse(response, chartForPdf);
                break;
        }
    }

    var igChartsFromResponse = function (response, chartForPdf) {
        $("div.ig-chart-modal .modal-body").html("");        
        Object.keys(response).forEach(function (key, index) {
            var chartData = {
                labels: [],
                datasets: []
            };

            var chartEntries = response[key];    
            var seriesData = [];
            if (chartEntries.length > 0) {
                $.each(chartEntries, function (index, entry) {
                    var stringDate = moment.unix(entry.dateTaken).format("ll");
                    chartData.labels.push(stringDate);
                    seriesData.push({ x: stringDate, y: entry.value });
                });
                var chartDataset = { label: key, data: seriesData }
                var serie = $.extend({}, chartDataset, randomUIChartSettings());
                var chartHtml = "<canvas id='ig-chart-content-popup-" + (index + 1) + "' style='width: 400px; height: 200px'></canvas>";              

                chartData.datasets.push(serie);
                $("div.ig-chart-modal .modal-body").append(chartHtml);              

                var chartId = "canvas#ig-chart-content-popup-" + (index + 1);
                var uiContext = $(chartId)[0];                           

                window["chart" + index] = new Chart(uiContext, {
                    type: 'line',
                    data: chartData
                });

                if (chartForPdf) {
                    var chartPdfHtml = "<canvas id='ig-chart-content-" + (index + 1) + "' style='width: 400px; height: 200px'></canvas>";
                    $("div#ig-charts").append(chartPdfHtml);
                    var pdfChartId = "canvas#ig-chart-content-" + (index + 1);
                    var pdfContext = $(pdfChartId)[0];

                    var options = {
                        animation: {
                            duration: 2000,

                            onProgress: function (animation) {

                            },
                            onComplete: function (animation) {
                                if (!img) {
                                    if (chartForPdf) {
                                        pdfContext.setAttribute('href', window["pdfChart" + index].toBase64Image());
                                        var img = new Image;
                                        img.setAttribute("class", "ig-chart");
                                        img.src = pdfContext.getAttribute('href');
                                        pdfContext.insertAdjacentHTML("afterend", img.outerHTML);
                                    }
                                }
                            }
                        },
                    };

                    window["pdfChart" + index] = new Chart(pdfContext, {
                        type: 'line',
                        data: chartData,
                        options: options
                    });
                }
            }
        });        
        $("div#ig-charts").addClass("hide");
    }
    //
    var chartUIOptions = function () {
        return [
            {
                backgroundColor: "rgba(255,99,132,0.2)",
                borderColor: "rgba(255,99,132,1)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(255,99,132,0.4)",
                hoverBorderColor: "rgba(255,99,132,1)"
            }, {
                backgroundColor: "rgba(124,252,0,0.2)",
                borderColor: "rgb(0,128,0)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(124,252,0,0.4)",
                hoverBorderColor: "rgb(173,255,47)"
            }, {
                backgroundColor: "rgba(135,206,250, 0.2)",
                borderColor: "rgb(0,0,255)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(135,206,250, 0.4)",
                hoverBorderColor: "rgb(0,0,139)"
            }, {
                backgroundColor: "rgba(218,165,32, 0.2)",
                borderColor: "rgb(160,82,45)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(218,165,32, 0.4)",
                hoverBorderColor: "rgb(139,69,19)"
            }, {
                backgroundColor: "rgba(229, 68, 36, 0.2)",
                borderColor: "rgba(137, 36, 16, 1)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(239, 138, 118, 0.4)",
                hoverBorderColor: "rgba(87, 23, 10, 1)"
            }
        ];
    }

    var randomUIChartSettings = function () {
        return chartUIOptions()[Math.floor(Math.random() * chartUIOptions().length)];
    }

    var sgrqChartFromResponse = function (response) {
        var totalData = [], symptomsData = []; activityData = [], impactData = [];
        var labels = [];

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
        var context = document.getElementById("sgrq-chart-content");
        var uiContext = document.getElementById("sgrq-chart-content-popup");

        var options = {
            animation: {
                duration: 2000,
                onComplete: function (animation) {
                    var imageUrl = stackedLine.toBase64Image(); 
                    console.log(imageUrl);
                    $("img#sgrq-chart-image").attr("src", imageUrl);
                    $("a.export-trigger").removeAttr("disabled");
                }
            },

        };
        var stackedLine = new Chart(context, {
            type: 'line',
            data: chartData,
            options: options
        });

        var uiStackedLine = new Chart(uiContext, {
            type: 'line',
            data: chartData,
        });
    }

    return {
        init: function () {
            initializeChart("click.show-sgrq", "a.show-sgrq-chart", "div#sgrq-chart-", "sgrq");
            initializeChart("click.show-ig-chart", "a.show-immunology-chart", "div#ig-chart-", "ig", false);
        },

        sgrqChartFromResponse: function (response) {
            return chartFromResponse("sgrq", response);
        },

        igChartsFromResponse: function (response, isFromPdf) {
            return chartFromResponse("ig", response, isFromPdf);
        }


    }
}();

