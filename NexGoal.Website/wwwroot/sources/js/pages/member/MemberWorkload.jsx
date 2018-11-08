import React, { Component } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';

import { axiosError, resolveClientUrl } from '../../common-functions';
import Multiselect from '../../shared/components/multi-select/index';

export default class MemberWorkload extends Component {
    constructor(props) {
        super(props);
        this.state = {
            workloads: [],
            groupOptions: [],
            selectedGroups: [],
            currentPeriodIndex: 0
        };
    }

    componentDidMount() {
        this.getMembersWorkloads();
    }

    getMembersWorkloads = () => {
        axios.get(resolveClientUrl('Home/GetMembersWorkload'), {
            params: {
                periodId: this.props.periods[this.state.currentPeriodIndex].periodId
            }
        }).then(response => {
            let workloads = response.data.map(this.generateWorkload);
            let groupOptions = response.data.map(x => ({ value: x.groupId, label: x.groupName }));

            this.setState(
                {
                    workloads: workloads,
                    groupOptions: groupOptions,
                    selectedGroups: groupOptions
                });
        }).catch(axiosError);
    }

    onNextPeriod = () => {
        if (this.props.periods[this.state.currentPeriodIndex + 1]) {
            this.setState({ currentPeriodIndex: this.state.currentPeriodIndex + 1 }, this.getMembersWorkloads);
        }
    }

    onPrevPeriod = () => {
        if (this.props.periods[this.state.currentPeriodIndex - 1]) {
            this.setState({ currentPeriodIndex: this.state.currentPeriodIndex - 1 }, this.getMembersWorkloads);
        }
    }

    generateWorkload = (groupWorkload) => {
        var totalHours = 0;
        groupWorkload.employeeWorkloads.forEach(e => {
            totalHours += e.workloads.reduce((total, current) => total + current.totalHours, 0)
        })

        return {
            groupId: groupWorkload.groupId,
            groupName: groupWorkload.groupName,
            chartData: this.generateDataSets(groupWorkload.employeeWorkloads),
            chartOptions: {
                ... this.chartOptions,
                ... {
                    title: {
                        display: true,
                        text: `${groupWorkload.groupName} (${totalHours} hours)`
                    }
                }
            }
        }
    }

    generateDataSets = (workloads) => {
        let labels = workloads.map((member) => member.nickName);
        let datasets = [];
        let projects = workloads[0].workloads.map((project) => project.name);

        for (var i = 0; i < projects.length; i++) {
            let dataSet = {
                label: projects[i],
                data: workloads.map((member) => member.workloads[i].totalHours),
                backgroundColor: "#F3AC16"
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

    generateColor = () => {
        let hex = "0123456789ABCDEF",
            color = "#";
        for (var i = 0; i < 6; i++) {
            color += hex[Math.floor(Math.random() * 16)];
        }

        return color;
    }

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
                barThickness: 20
            }],
        },
        legend: {
            display: false
        }
    }

    renderChart = (group) => {
        let workload = this.state.workloads.find(x => x.groupId === group.value);
        let basicHeight = 100;

        if (workload.chartData.labels.length > 2) {
            basicHeight += (workload.chartData.labels.length - 2) * 20
        }

        return (
            <HorizontalBar height={basicHeight} key={workload.groupId} data={workload.chartData} options={workload.chartOptions} />
        );
    }

    render() {
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
                {this.state.groupOptions.length > 0 &&
                    <div>
                        <div className="team">Team</div>
                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                            value={this.state.selectedGroups}
                            options={this.state.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                }

                {this.state.selectedGroups.map(this.renderChart)}
            </div>
        );
    }
}