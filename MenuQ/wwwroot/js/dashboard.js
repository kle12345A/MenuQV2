<scrip>
    function changeRevenueFilter(filter) {
        let revenue, percentage, changeText;

    if (filter === 'Today') {
        revenue = @Model.TotalRevenueToday;
    percentage = @Model.TodayRevenueChangePercentage;
        } else if (filter === 'This Month') {
        revenue = @Model.TotalRevenue;
    percentage = @Model.MonthRevenueChangePercentage;
        } else {
        revenue = @Model.TotalRevenueCurrentYear;
    percentage = @Model.YearRevenueChangePercentage;
        }

        changeText = percentage >= 0 ? 'increase' : 'decrease';

    document.getElementById('revenue-filter-label').innerText = '| ' + filter;
    document.getElementById('revenue-value').innerText = revenue.toLocaleString('en-US', {minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' VNĐ';
    document.getElementById('revenue-change-percentage').innerText = Math.abs(percentage).toFixed(2) + '%';
        document.getElementById('revenue-change-percentage').className = percentage >= 0 ? 'text-success small pt-1 fw-bold' : 'text-danger small pt-1 fw-bold';
    document.querySelector('#revenue-change-text').innerText = changeText;

    let currentUrl = new URL(window.location.href);
    currentUrl.searchParams.set('filter', filter);
    window.history.pushState({ }, '', currentUrl);
    }

    // Hàm xử lý filter cho Customers Card
    function changeCustomerFilter(filter) {
        let count, percentage, changeText;

    if (filter === 'Today') {
        count = @Model.TotalCustomersToday;
    percentage = @Model.TodayCustomersChangePercentage;
        } else if (filter === 'This Month') {
        count = @Model.TotalCustomersCurrentMonth;
    percentage = @Model.MonthCustomersChangePercentage;
        } else {
        count = @Model.TotalCustomersCurrentYear;
    percentage = @Model.YearCustomersChangePercentage;
        }

        changeText = percentage >= 0 ? 'increase' : 'decrease';

    // Cập nhật giao diện
    document.getElementById('customer-filter-label').innerText = '| ' + filter;
    document.getElementById('customer-count').innerText = count;
    document.getElementById('customer-change-percentage').innerText = Math.abs(percentage).toFixed(2) + '%';
        document.getElementById('customer-change-percentage').className = percentage >= 0 ? 'text-success small pt-1 fw-bold' : 'text-danger small pt-1 fw-bold';
    document.getElementById('customer-change-text').innerText = changeText;

    // Cập nhật URL mà không tải lại trang
    let currentUrl = new URL(window.location.href);
    currentUrl.searchParams.set('customerFilter', filter);
    window.history.pushState({ }, '', currentUrl);
    }

    // Hàm lấy filter hiện tại của Customers Card từ URL hoặc mặc định
    function getCurrentCustomerFilter() {
        let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('customerFilter') || 'This Year';
    }

    // Hàm lấy filter hiện tại của Reports Chart từ URL hoặc mặc định
    function getCurrentReportFilter() {
        let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('reportFilter') || 'Today';
    }

    // Hàm xử lý filter cho Reports Chart (giữ nguyên)
    let chart;
    function initChart(revenueData, customersData, timeCategories, reportFilter) {
        if (chart) {
        chart.destroy();
        }

    chart = new ApexCharts(document.querySelector("#reportsChart"), {
        series: [
    {
        name: 'Revenue (VNĐ)',
    data: revenueData
                },
    {
        name: 'Customers',
    data: customersData
                }
    ],
    chart: {
        height: 350,
    type: 'area',
    toolbar: {
        show: false
                },
            },
    markers: {
        size: 4
            },
    colors: ['#4154f1', '#ff771d'],
    fill: {
        type: "gradient",
    gradient: {
        shadeIntensity: 1,
    opacityFrom: 0.3,
    opacityTo: 0.4,
    stops: [0, 90, 100]
                }
            },
    dataLabels: {
        enabled: false
            },
    stroke: {
        curve: 'smooth',
    width: 2
            },
    xaxis: {
        type: 'datetime',
    categories: timeCategories
            },
    tooltip: {
        x: {
        format: reportFilter === 'Today' ? 'HH:mm' : reportFilter === 'This Month' ? 'dd/MM' : 'MM/yyyy'
                },
    y: {
        formatter: function (val, {seriesIndex}) {
                        if (seriesIndex === 0) {
                            return val.toLocaleString() + ' VNĐ';
                        } else {
                            return val.toString();
                        }
                    }
                }
            },
    yaxis: [
    {
        title: {
        text: 'Revenue (VNĐ)'
                    }
                },
    {
        opposite: true,
    title: {
        text: 'Customers'
                    }
                }
    ]
        });
    chart.render();
    }

    function updateChart(reportFilter) {
        fetch('@Url.Action("GetChartData", "Dashboard", new { Area = "Admin" })?reportFilter=' + reportFilter)
            .then(response => response.json())
            .then(data => {
                document.getElementById('report-filter-label').innerText = '| ' + reportFilter;
                let currentUrl = new URL(window.location.href);
                currentUrl.searchParams.set('reportFilter', reportFilter);
                window.history.pushState({}, '', currentUrl);
                initChart(data.revenueData, data.customersData, data.timeCategories, reportFilter);
            })
            .catch(error => console.error('Error fetching chart data:', error));
    }

    document.addEventListener("DOMContentLoaded", () => {
        updateChart('@ViewBag.ReportFilter');
    });

</scrip>
  