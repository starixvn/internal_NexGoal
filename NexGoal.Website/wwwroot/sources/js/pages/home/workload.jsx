import React, { PureComponent } from 'react';
import axios from 'axios';
import { Doughnut } from 'react-chartjs-2';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { ChartColors } from '../../constants';
import getResource from '../../resourceHelper';
import Milestone from '../../shared/components/milestone';

export default class WorkLoad extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            chartData: null,
            periodId: this.props.periods[0].periodId
        };

        this.chartOptions.afterInit = this.props.afterInitChart;
    }

    componentDidMount() {
        this.getWorkload();
    }

    componentDidUpdate() {
        this.generateChartLegend();
    }

    generateChartLegend = () => {
        if (this.refs.chart) {
            document.getElementById('legend-container').innerHTML = this.refs.chart.chartInstance.generateLegend();
        } else {
            document.getElementById('legend-container').innerHTML = '';
        }
    }

    getWorkload = () => {
        axios.get(resolveClientUrl('Home/GetWorkload'), {
            params: {
                periodId: this.state.periodId,
                memberId: this.props.memberId ? this.props.memberId : null
            }
        }).then(response => this.setState({
            chartData: this.getChartData(response.data)
        }, () => this.chartOptions.afterInit = null
        )).catch(axiosError);
    }

    getChartData = workload => {
        if (workload.length > 5) {
            const labels = workload.slice(0, 5).map(e => e.name);
            labels.push('Others');
            const data = workload.slice(0, 5).map(e => e.totalHours);

            const othersValue = workload.slice(5, workload.length).reduce(function (total, current) {
                return total + current.totalHours;
            }, 0);

            data.push(othersValue);

            return {
                datasets: [{
                    data: data,
                    backgroundColor: ChartColors
                }],
                labels: labels
            };
        }

        return {
            datasets: [{
                data: workload.map(e => e.totalHours),
                backgroundColor: ChartColors
            }],
            labels: workload.map(e => e.name)
        };
    }

    onPeriodChange = periodId => {
        this.setState({
            periodId: periodId
        }, this.getWorkload);
    }

    chartOptions = {
        animation: {
            duration: 500,
            onComplete: function () {
                if (this.options.afterInit) {
                    this.options.afterInit();
                    this.options.afterInit = null;
                }
            }
        },
        cutoutPercentage: 60,
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    const dataset = data.datasets[tooltipItem.datasetIndex];
                    const currentValue = dataset.data[tooltipItem.index];

                    return `${currentValue} hours`;
                },
                title: function (tooltipItem, data) {
                    return data.labels[tooltipItem[0].index];
                }
            }
        },
        legend: {
            position: 'bottom',
            labels: {
                usePointStyle: true,
                generateLabels: function (chart) {
                    const data = chart.data;

                    if (data.labels.length && data.datasets.length) {
                        const meta = chart.getDatasetMeta(0);
                        const ds = data.datasets[0];
                        const total = ds.data.reduce((accumulator, currentValue) => accumulator + currentValue);

                        return data.labels.map(function (label, i) {
                            const percentage = (ds.data[i] / total * 100).toFixed(1);
                            const arc = meta.data[i];
                            const custom = arc && arc.custom || {};
                            const arcOpts = chart.options.elements.arc;
                            const fill = custom.backgroundColor ? custom.backgroundColor : ds.backgroundColor[i] || arcOpts.backgroundColor;
                            const stroke = arcOpts.borderColor;
                            const bw = arcOpts.borderWidth;

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
            },
            display: false
        },
        maintainAspectRatio: false,
        legendCallback: function (chart) {
            const data = chart.data;
            let legends = [];
            if (data.labels.length && data.datasets.length) {
                const meta = chart.getDatasetMeta(0);
                const ds = data.datasets[0];
                const total = ds.data.reduce((accumulator, currentValue) => accumulator + currentValue);

                legends = data.labels.map(function (label, i) {
                    const percentage = (ds.data[i] / total * 100).toFixed(1);
                    const arc = meta.data[i];
                    const custom = arc && arc.custom || {};
                    const arcOpts = chart.options.elements.arc;
                    const fill = custom.backgroundColor ? custom.backgroundColor : ds.backgroundColor[i] || arcOpts.backgroundColor;
                    const stroke = arcOpts.borderColor;

                    return `
                        <div class="legend-item">
                            <span style="background-color: ${fill};border-color: ${stroke};" ></span>
                            ${label}: ${percentage}%
                        </div>
                    `;
                });
            }

            return legends.join('');
        }
    }

    renderWorkloadChart = () => {
        const chartData = this.state.chartData;

        if (chartData !== null && chartData.labels.length > 0) {
            return <Doughnut ref='chart' data={chartData} height={200} options={this.chartOptions} />;
        }

        return <span>{getResource('NoRecordFound')}</span>;
    }

    render() {
        const periodOptions = this.props.periods.map(period => { return { name: period.name, value: period.periodId }; });
        return (
            <div className="workload box">
                <div className="title clearfix">
                    <h5>{getResource('Workload')}</h5>
                    <Milestone
                        options={periodOptions}
                        onChange={this.onPeriodChange}
                    />
                </div>
                <div style={{ position: 'relative' }}>
                    {
                        this.renderWorkloadChart()
                    }
                </div>
                <div id="legend-container" className="legend-container" />
            </div>
        );
    }
}