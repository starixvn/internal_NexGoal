import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Multiselect from '../../shared/components/multi-select/index';
import Milestone from '../../shared/components/milestone';

export default class MemberWorkload extends Component {
    constructor(props) {
        super(props);

        this.state = {
            workloads: [],
            selectedGroups: this.props.groupOptions,
            periodId: this.props.periods[0].periodId
        };
    }

    componentDidMount() {
        this.getMembersWorkloads();
    }

    getMembersWorkloads = () => {
        axios.get(resolveClientUrl('Member/GetMembersWorkload'), {
            params: {
                periodId: this.state.periodId
            }
        }).then(response => {
            let workloads = response.data.map(this.generateWorkload);

            this.setState(
                {
                    workloads: workloads
                });
        }).catch(axiosError);
    }

    onPeriodChange = (periodId) => {
        this.setState({
            periodId: periodId
        }, this.getMembersWorkloads);
    }

    generateWorkload = (groupWorkload) => {
        var totalHours = 0;
        groupWorkload.employeeWorkloads.forEach(e => {
            totalHours += e.workloads.reduce((total, current) => total + current.totalHours, 0);
        });

        return {
            groupId: groupWorkload.groupId,
            groupName: groupWorkload.groupName,
            title: `${groupWorkload.groupName} (${totalHours.toFixed(2)} hours)`,
            chartData: this.generateDataSets(groupWorkload.employeeWorkloads),
            chartOptions: this.chartOptions
        };
    }

    generateDataSets = (workloads) => {
        let labels = workloads.map((member) => member.nickName);
        let datasets = [];
        let projects = workloads[0].workloads.map((project) => project.name);

        for (var i = 0; i < projects.length; i++) {
            let dataSet = {
                label: projects[i],
                data: workloads.map((member) => member.workloads[i].totalHours),
                backgroundColor: "#2748A5"
            };

            datasets.push(dataSet);
        }

        return {
            labels,
            datasets
        };
    }

    handleGroupChange = (selectedOptions) => this.setState({
        selectedGroups: selectedOptions
    })

    chartOptions = {
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var dataset = data.datasets[tooltipItem.datasetIndex];
                    var currentValue = dataset.data[tooltipItem.index];
                    return currentValue > 0 ? `${dataset.label}: ${currentValue}h` : ``;
                },
                title: function (tooltipItem, data) {
                    return data.labels[tooltipItem[0].index] > 0 ? data.labels[tooltipItem[0].index] : ``;
                }
            },
            enabled: false,
            custom: function (tooltipModel) {
                // Tooltip Element
                var tooltipEl = document.getElementById('chartjs-tooltip');

                // Create element on first render
                if (!tooltipEl) {
                    tooltipEl = document.createElement('div');
                    tooltipEl.id = 'chartjs-tooltip';
                    tooltipEl.innerHTML = "<table></table>";
                    document.body.appendChild(tooltipEl);
                }

                // Hide if no tooltip
                if (tooltipModel.opacity === 0) {
                    tooltipEl.style.opacity = 0;
                    return;
                }

                // Set caret Position
                tooltipEl.classList.remove('above', 'below', 'no-transform');
                if (tooltipModel.yAlign) {
                    tooltipEl.classList.add(tooltipModel.yAlign);
                } else {
                    tooltipEl.classList.add('no-transform');
                }

                function getBody(bodyItem) {
                    return bodyItem.lines;
                }

                // Set Text
                if (tooltipModel.body) {
                    var titleLines = tooltipModel.title || [];
                    var bodyLines = tooltipModel.body.map(getBody);

                    var innerHtml = '<thead>';

                    titleLines.forEach(function (title) {
                        innerHtml += '<tr><th>' + title + '</th></tr>';
                    });
                    innerHtml += '</thead><tbody>';
                    bodyLines.forEach(function (body, i) {
                        if (body.length > 0) {
                            var colors = tooltipModel.labelColors[i];
                            var style = 'background:' + colors.backgroundColor;
                            style += '; border: 2px solid ' + colors.borderColor;
                            var span = '<span class="chartjs-tooltip-key" style="' + style + '"></span>';
                            innerHtml += '<tr><td>' + body + '</td></tr>';
                        }
                    });
                    innerHtml += '</tbody>';

                    var tableRoot = tooltipEl.querySelector('table');
                    tableRoot.innerHTML = innerHtml;
                }

                var position = this._chart.canvas.getBoundingClientRect();

                // Display, position, and set styles for font
                tooltipEl.style.opacity = 1;
                tooltipEl.style.position = 'absolute';
                tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX + 'px';
                tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
                tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
                tooltipEl.style.pointerEvents = 'none';
            }
        },
        scales: {
            yAxes: [{
                gridLines: {
                    display: false
                },
                stacked: true,
                barThickness: 20,
                afterFit: function (scaleInstance) {
                    scaleInstance.width = 60;
                }
            }],
            xAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        },
        legend: {
            display: false
        },
        maintainAspectRatio: false
    }

    renderChart = (workload, i) => {
        if (this.state.selectedGroups.some(x => x.value === workload.groupId)) {
            let basicHeight = 140;

            if (workload.chartData.labels.length > 2) {
                basicHeight += (workload.chartData.labels.length - 2) * 20;
            }

            return (
                <Fragment key={workload.groupId} >
                    <div className="group-name collapsed"
                        data-toggle="collapse"
                        data-target={`#chart-${workload.groupId}`}
                        aria-expanded="false">
                        <span className="icon-minus" /> {workload.title}
                    </div>
                    <div id={`chart-${workload.groupId}`} className="collapse" aria-expanded="false">
                        <div style={{ position: 'relative', height: `${basicHeight}px`, width: '100%' }}>
                            <HorizontalBar data={workload.chartData} options={workload.chartOptions} />
                        </div>
                    </div>
                </Fragment >
            );
        }

        return '';
    }

    render() {
        const selections = this.props.periods.map(period => { return { name: period.name, value: period.periodId }; });

        return (
            <div className="workload box">
                <div className="title">
                    <h5>{getResource('Workload')}</h5>
                    <Milestone
                        selections={selections}
                        onSelectionChange={this.onPeriodChange}
                    />
                </div>
                {this.props.groupOptions.length > 1 &&
                    <div>
                        <div className="team">{getResource('Team')}</div>
                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                            value={this.state.selectedGroups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                }
                <div id="chart-wrapper" >
                    {this.state.workloads.map(this.renderChart)}
                </div>
            </div>
        );
    }
}