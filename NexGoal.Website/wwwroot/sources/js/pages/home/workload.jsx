import React, { PureComponent } from 'react';
import axios from 'axios';
import { Doughnut } from 'react-chartjs-2';
import memoize from "memoize-one";

import { axiosError, resolveClientUrl } from '../../common-functions';
import { ChartColors } from '../../constants';

export default class WorkLoad extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            chartData: null,
            currentPeriodIndex: 0
        };
    }

    componentDidMount() {
        this.getWorkload();
    }

    getWorkload = () => {
        axios.get(resolveClientUrl('Home/GetWorkload'), {
            params: {
                periodId: this.props.periods[this.state.currentPeriodIndex].periodId,
                memberId: this.props.memberId ? this.props.memberId : null
            }
        }).then(response => this.setState({
            chartData: this.getChartData(response.data)
        })).catch(axiosError);
    }

    getChartData = workload => {
        debugger;
        if (workload.length > 5) {
            let labels = workload.slice(0, 5).map(e => e.name);
            labels.push("Others");
            let data = workload.slice(0, 5).map(e => e.totalHours);

            let othersValue = workload.slice(5, workload.length).reduce(function (total, current) {
                return total += current.totalHours;
            }, 0);

            data.push(othersValue);

            return {
                datasets: [{
                    data: data,
                    backgroundColor: ChartColors
                }],
                labels: labels
            }
        }

        return {
            datasets: [{
                data: workload.map(e => e.totalHours),
                backgroundColor: ChartColors
            }],
            labels: workload.map(e => e.name)
        }
    }

    onNextPeriod = () => {
        if (this.props.periods[this.state.currentPeriodIndex + 1]) {
            this.setState({ currentPeriodIndex: this.state.currentPeriodIndex + 1 }, this.getWorkload);
        }
    }

    onPrevPeriod = () => {
        if (this.props.periods[this.state.currentPeriodIndex - 1]) {
            this.setState({ currentPeriodIndex: this.state.currentPeriodIndex - 1 }, this.getWorkload);
        }
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
                usePointStyle: true,
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
                                text: `${label}: ${percentage}% `,
                                fillStyle: fill,
                                strokeStyle: stroke,
                                lineWidth: bw,
                                hidden: isNaN(ds.data[i]) || meta.data[i].hidden,
                                index: i
                            };
                        });
                    }

                    return [];
                }
            }
        }
    }

    render() {
        const chartData = this.state.chartData;

        return (
            <div className="workload box">
                <div className="title">
                    <h5>Workload</h5>
                    <div className="milestone">
                        <div className="icon-chevron-down arrow-left" onClick={this.onNextPeriod} />
                        {this.props.periods[this.state.currentPeriodIndex].name}
                        <div className="icon-chevron-down arrow-right" onClick={this.onPrevPeriod} />
                    </div>
                </div>
                {chartData && <Doughnut data={chartData} options={this.chartOptions} />}
            </div>
        );
    }
}