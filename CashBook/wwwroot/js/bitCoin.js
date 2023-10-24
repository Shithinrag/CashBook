const chartBitcoin = document.querySelector("#chart-bitcoin").getContext("2d");
const labelBitcoin = [
    "06/07", "07/07", "08/07", "09/07",
    "10/07", "11/07", "12/07", "13/07",
    "14/07"
];
const datasetsBitcoin = [
    12914, 11720, 9831, 11847, 10810,
    11837, 12187, 9833, 12345,];
const gradientColorBitcoin = chartBitcoin.createLinearGradient(0, 90, 450, 90);
gradientColorBitcoin.addColorStop(0.3, "#D37416");
gradientColorBitcoin.addColorStop(1, "#D3A246");

const dataBitcoin = {
    labels: labelBitcoin,
    datasets: [{
        data: datasetsBitcoin,
        backgroundColor: gradientColorBitcoin,
    },
    ],
};

const optionsBitcoin = {
    elements: {
        line: {
            fill: true,
            borderColor: "#FDB92A",
            borderWidth: 3,
            tension: 0.4,
        },
        point: {
            pointBackgroundColor: "#FA7E0C",
            pointBorderColor: "#ffffff",
            hoverBorderWidth: 3,
            hoverRadius: 6,
            radius: 0,
        },
    },
    interaction: {
        intersect: false,
        mode: "index",
    },
    maintainAspectRatio: false,
    scales: {
        y: { display: false, min: 7000 },
        x: { display: false },
    },
    plugins: {
        legend: { display: false },
    },
};

new Chart(chartBitcoin, {
    type: "line",
    data: dataBitcoin,
    options: optionsBitcoin,
});