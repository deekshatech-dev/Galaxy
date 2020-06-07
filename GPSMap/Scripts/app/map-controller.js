var galexy = galexy || {};
galexy.MapController = function (config) {

    var $trend = $("[data-trend-id='" + config.filter.id + "']");
    var $kpi = $("[data-kip-id='" + config.filter.id + "']");
    var $period = $("[data-period-id='" + config.filter.id + "']");
    var $chartContainer = $("#chart-container-" + config.filter.id);
    var $showSeperate = $("[data-seperate-id='" + config.filter.id + "']");

    var getRandomChart = function () {
        var max = window.chartColors.length;
        return window.chartColors[Math.floor(Math.random() * Math.floor(max))];
    }

    var generateChart = function (ctx, data) {

        var datasets = [];
        var chartLabels;
        if (Array.isArray(data)) {
            chartLabels = data[0].ChartData.Labels;
            $.each(data,
                function (index, value) {
                    var chartDatasets = {
                        label: value.KPI,
                        data: value.ChartData.ChartData,
                        borderColor: getRandomChart(),
                        fill: false
                    };
                    datasets.push(chartDatasets);
                });
        } else {
            chartLabels = data.ChartData.Labels;
            var chartDatasets = {
                label: data.KPI,
                data: data.ChartData.ChartData,
                borderColor: getRandomChart(),
                fill: false
            };
            datasets.push(chartDatasets);
        }

        var myChart = new Chart(ctx,
            {
                type: 'line',
                data: {
                    labels: chartLabels, // chartLabels
                    datasets: datasets
                },
                options: {
                    scales: {
                        xAxes: [
                            {
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Dates | Hours'
                                }
                            }
                        ],
                        yAxes: [
                            {
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'KPI Value'
                                }
                            }
                        ]
                    }
                }
            });
    };

    var bindChart = function (chart) {

        $chartContainer.empty();
        var createMultiple = false;

        if ($showSeperate.is(":checked") && chart.length > 1) {
            createMultiple = true;
        }

        var newCanvas;
        var chartId;

        var panel;
        var panelid;
        var ctx;
        if (createMultiple) {
            for (var i = 0; i < chart.length; i++) {
                chartId = 'myChart' + i;
                newCanvas = $('<canvas/>', { 'id': chartId });

                panel = `<div class="col-md-6" >
                                <div class="panel panel-default" id="panel-container-${chartId}">                                     
                                     <div class="panel-body"></div>
                                </div>
                            </div>`;

                $chartContainer.append(panel);
                panelid = `#panel-container-${chartId} .panel-body`;
                $(panelid).html(newCanvas);
                ctx = document.getElementById(chartId).getContext('2d');
                generateChart(ctx, chart[i]);
            }
        } else {
            // plot single chart
            chartId = 'myChart';
            newCanvas = $('<canvas/>', { 'id': chartId });

            panel = `<div class="col-md-12" >
                                <div class="panel panel-default" id="panel-container-${chartId}">                                     
                                     <div class="panel-body"></div>
                                </div>
                            </div>`;

            $chartContainer.append(panel);
            panelid = `#panel-container-${chartId} .panel-body`;
            $(panelid).html(newCanvas);
            ctx = document.getElementById('myChart').getContext('2d');
            generateChart(ctx, chart);
        }

    };

    var isValidate = function () {
        if ($kpi.val() == "" || $kpi.val() == []) {
            toastr.error("Please select at least one KPI.");
            return false;
        }
        if ($trend.val() == "") {
            toastr.error("Please select chart type.");
            return false;
        }
        return true;
    };

    var getChartData = function () {
        if (isValidate()) {
            $.blockUI();
            $.ajax({
                type: "POST",
                url: config.url.chart,
                data: JSON.stringify({
                    "Trend": $trend.val(),
                    "KpiId": $kpi.val(),
                    "Date": $period.data('date')
                }),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    bindChart(data);
                },
                "error": function (data) {
                    alert("Some Error Occured!");
                }
            })
                .always(function () {
                    $.unblockUI();
                });
            return false;
        } else {
            return false;
        }
    };

    var registerEvents = function () {

        $kpi.multiselect({
            includeSelectAllOption: false,
            numberDisplayed: 1
        });

        $period.datetimepicker({
            format: 'DD/MM/YYYY',
            showClear: true
        });

        $("[data-gen-chart-id='" + config.filter.id + "']").on('click', getChartData);
    };

    var init = function () {
        registerEvents();
    };

    return {
        init: init
    };
};
