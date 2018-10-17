import React, { PureComponent } from 'react';
import axios from 'axios';
import { Doughnut } from 'react-chartjs-2';
import memoize from "memoize-one";

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class WorkLoad extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            workload: [],
            currentPeriodIndex: 0
        }
    }

    componentDidMount() {
        this.getWorkload();
    }

    getWorkload = () => {
        axios.get(resolveClientUrl('Home/GetWorkload'), {
            params: {
                periodId: this.props.periods[this.state.currentPeriodIndex].periodId
            }
        }).then(response => this.setState({ workload: response.data })).catch(axiosError);
    }

    onNextPeriod = () => {
        this.setState({ currentPeriodIndex: this.state.currentPeriodIndex + 1 }, this.getWorkload);
    }

    onPrevPeriod = () => {
        this.setState({ currentPeriodIndex: this.state.currentPeriodIndex - 1 }, this.getWorkload);
    }

    chartOptions = {
        cutoutPercentage: 60,
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var dataset = data.datasets[tooltipItem.datasetIndex];
                    var currentValue = dataset.data[tooltipItem.index];

                    return `${currentValue} hours`;
                },
                title: function (tooltipItem, data) {
                    return data.labels[tooltipItem[0].index];
                }
            }
        },
        legend: {
            position: 'right',
            labels: {
                generateLabels: function (chart) {
                    let data = chart.data;

                    if (data.labels.length && data.datasets.length) {
                        var meta = chart.getDatasetMeta(0);
                        var ds = data.datasets[0];
                        var total = ds.data.reduce((accumulator, currentValue) => accumulator + currentValue);

                        return data.labels.map(function (label, i) {
                            var percentage = (ds.data[i] / total * 100).toFixed(1);
                            var arc = meta.data[i];
                            var custom = arc && arc.custom || {};
                            var arcOpts = chart.options.elements.arc;
                            var fill = custom.backgroundColor ? custom.backgroundColor : ds.backgroundColor[i] || arcOpts.backgroundColor;
                            var stroke = arcOpts.borderColor;
                            var bw = arcOpts.borderWidth;

                            return {
                                text: `${label}: ${percentage}%`,
                                fillStyle: fill,
                                strokeStyle: stroke,
                                lineWidth: bw,
                                hidden: isNaN(ds.data[i]) || meta.data[i].hidden,
                                index: i
                            }
                        })
                    }

                    return [];
                }
            }
        }
        //plugins: {
        //    datalabels: {
        //        backgroundColor: '#fff',
        //        color: '#000',
        //        anchor: 'end'
        //    }
        //}
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
        workload => (
            {
                datasets: [{
                    data: workload.map(e => e.totalHours),
                    backgroundColor: this.generateColors(workload.length)
                }],
                labels: workload.map(e => e.name)
            }
        )
    )

    render() {
        if (this.state.workload.length > 0) {
            var chartData = this.getChartData(this.state.workload);

            return (
                <Doughnut data={chartData} options={this.chartOptions} />
            );
        }

        return '';
    }
}