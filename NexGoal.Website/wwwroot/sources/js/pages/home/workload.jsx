import React, { Component } from 'react';
import axios from 'axios';
import { Pie } from 'react-chartjs-2';
import 'chartjs-plugin-datalabels';

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class WorkLoad extends Component {
    constructor(props) {
        super(props);

        this.state = {
            workLoad: [],
            chartData: null
        };
    }

    componentDidMount() {
        this.getWorkLoad();
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

    getWorkLoad = () => {
        axios.get(resolveClientUrl('Home/GetWorkLoad'))
            .then(response => {
                this.setState({
                    workLoad: response.data,
                    chartData: {
                        datasets: [{
                            data: response.data.map(e => e.totalHours),
                            backgroundColor: this.generateColors(response.data.length)
                        }],

                        labels: response.data.map(e => e.name)
                    }
                });
            }).catch(axiosError);
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

    render() {
        return (
            this.state.chartData &&
            <Pie data={this.state.chartData} options={this.chartOptions} />
        );
    }
}