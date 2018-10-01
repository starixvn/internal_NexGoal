import React, { Component } from 'react';
import axios from 'axios';
import { Pie } from 'react-chartjs-2';
import 'chartjs-plugin-datalabels';
import memoize from "memoize-one";

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class WorkLoad extends Component {
    constructor(props) {
        super(props);
    }

    chartOptions = {
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var dataset = data.datasets[tooltipItem.datasetIndex];
                    var meta = dataset._meta[0];
                    var currentValue = dataset.data[tooltipItem.index];
                    var percentage = parseFloat((currentValue / meta.total * 100).toFixed(1));

                    return `${currentValue} (${percentage}%)`;
                },
                title: function (tooltipItem, data) {
                    return data.labels[tooltipItem[0].index];
                }
            }
        },
        plugins: {
            datalabels: {
                backgroundColor: '#fff',
                color: '#000',
                anchor: 'end'
            }
        }
    }

    generateColors = number => {
        let colors = [];

        for (let i = 0; i < number; i++) {
            let hex = "0123456789ABCDEF",
                color = "#";
            for (let j = 1; j <= 6; j++) {
                color += hex[Math.floor(Math.random() * 16)];
            }

            colors.push(color);
        }

        return colors;
    }

    getChartData = memoize(
        workLoad => (
            {
                datasets: [{
                    data: workLoad.map(e => e.totalHours),
                    backgroundColor: this.generateColors(workLoad.length)
                }],
                labels: workLoad.map(e => e.name)
            }
        )
    )

    render() {
        var chartData = this.getChartData(this.props.workLoad);

        return (
            <Pie data={chartData} options={this.chartOptions} />
        );
    }
}