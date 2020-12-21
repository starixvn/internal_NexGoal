import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Multiselect from '../../shared/components/multi-select/multi-select';
import Milestone from '../../shared/components/milestone';

export default class MemberWorkload extends Component {
    constructor(props) {
        super(props);

        this.state = {
            workloads: null,
            selectedGroups: this.props.groupOptions,
            periodId: this.props.periods[0].periodId
        };

        this.chartOptions.afterInit = this.props.afterInitChart;
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
            const workloads = response.data.map(this.generateWorkload);

            this.setState({ workloads: workloads }, () => this.chartOptions.afterInit = null);
        }).catch(axiosError);
    };

    onPeriodChange = periodId => {
        this.setState({
            periodId: periodId
        }, this.getMembersWorkloads);
    };

    generateWorkload = groupWorkload => {
        let totalHours = 0;
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
    };

    generateDataSets = employeeWorkloads => {
        const labels = [];
        const dataset = {
            label: 'workload',
            backgroundColor: '#2748A5',
            data: [],
            workloadDetails: []
        };
        const datasets = [dataset];

        employeeWorkloads.forEach(employee => {
            labels.push(employee.nickName);
            dataset.data.push(employee.workloads.reduce((total, workload) => total + workload.totalHours, 0));
            dataset.workloadDetails.push(employee.workloads.map(w => ({
                project: w.name,
                totalHours: w.totalHours
            })));
        });

        return {
            labels,
            datasets
        };
    };

    handleGroupChange = selectedOptions => this.setState({
        selectedGroups: selectedOptions
    });

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
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    const dataset = data.datasets[tooltipItem.datasetIndex];
                    return dataset.workloadDetails[tooltipItem.index];
                }
            },
            enabled: false,
            custom: function (tooltipModel) {
                // Tooltip Element
                let tooltipEl = document.getElementById('chartjs-tooltip');

                // Create element on first render
                if (!tooltipEl) {
                    tooltipEl = document.createElement('div');
                    tooltipEl.id = 'chartjs-tooltip';
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

                // Set Text
                if (tooltipModel.body) {
                    const bodyLines = tooltipModel.body.map(bodyItem => bodyItem.lines);
                    let innerHtml = '';
                    bodyLines.forEach(function (body) {
                        body.forEach(b => innerHtml += '<p>' + `${b.project}: ${b.totalHours}h` + '</p>');
                    });

                    tooltipEl.innerHTML = innerHtml;
                }

                const position = this._chart.canvas.getBoundingClientRect();

                // Display, position, and set styles for font
                const styleLeft = position.left + window.pageXOffset + 100;
                const styleTop = position.top + window.pageYOffset + tooltipModel.caretY;

                tooltipEl.style.opacity = 1;
                tooltipEl.style.position = 'absolute';
                tooltipEl.style.left =  `${styleLeft}px`;
                tooltipEl.style.top =  `${styleTop}px`;
                tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                tooltipEl.style.padding = `${tooltipModel.yPadding}px ${tooltipModel.xPadding}px`;
                tooltipEl.style.pointerEvents = 'none';
            }
        },
        scales: {
            yAxes: [{
                gridLines: {
                    display: false
                },
                barThickness: 20,
                afterFit: function (scaleInstance) {
                    scaleInstance.width = 80;
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
    };

    renderGroupName = (groupId, groupName) => {
        if (this.props.groupOptions.length > 1) {
            return (
                <div className="group-name collapsed"
                    data-toggle="collapse"
                    data-target={`#chart-${groupId}`}
                    aria-expanded="false"
                >
                    <span className="icon-minus" /> {groupName}
                </div>
            );
        }

        return <div className="group-name">{groupName}</div>;
    };

    renderChart = workload => {
        if (this.state.selectedGroups.some(x => x.value === workload.groupId)) {
            let basicHeight = 140;

            if (workload.chartData.labels.length > 2) {
                basicHeight += (workload.chartData.labels.length - 2) * 20;
            }

            return (
                <Fragment key={workload.groupId} >
                    {this.renderGroupName(workload.groupId, workload.title)}
                    <div id={`chart-${workload.groupId}`}
                        className={this.props.groupOptions.length > 1 ? 'collapse' : ''}
                        aria-expanded="false"
                    >
                        <div style={{ position: 'relative', height: `${basicHeight}px`, width: '100%' }}>
                            <HorizontalBar data={workload.chartData} options={workload.chartOptions} />
                        </div>
                    </div>
                </Fragment >
            );
        }

        return '';
    };

    renderTeamWorkload = () => {
        if (this.state.workloads.length > 0) {
            return this.state.workloads.map(this.renderChart);
        } else {
            return <span>{getResource('NoRecordFound')}</span>;
        }
    };

    render() {
        const periodOptions = this.props.periods.map(period => {
          return { name: period.name, value: period.periodId };
        });

        return (
            <div className="workload box">
                <div className="title clearfix">
                    <h5>{getResource('Workload')}</h5>
                    <Milestone
                        options={periodOptions}
                        onChange={this.onPeriodChange}
                    />
                </div>
                {
                    this.props.groupOptions.length > 1 &&
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
                    {this.state.workloads &&
                        this.renderTeamWorkload()
                    }
                </div>
            </div>
        );
    }
}