import React, { Component } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';
import { Scrollbars } from 'react-custom-scrollbars';

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
                    selectedGroups: groupOptions,
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

    generateWorkload = (groupWorkload) => ({
        groupId: groupWorkload.groupId,
        groupName: groupWorkload.groupName,
        chartData: this.generateDataSets(groupWorkload.employeeWorkloads),
        chartOptions: {
            ... this.chartOptions,
            ... {
                title: {
                    display: true,
                    text: groupWorkload.groupName
                }
            }
        }
    })

    generateDataSets = (workloads) => {
        let labels = workloads.map((member) => member.nickName);
        let datasets = [];
        let projects = workloads[0].workloads.map((project) => project.name);

        for (var i = 0; i < projects.length; i++) {
            let dataSet = {
                label: projects[i],
                data: workloads.map((member) => member.workloads[i].totalHours),
                backgroundColor: this.generateColor()
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
            }
        },
        scales: {
            yAxes: [{
                gridLines: {
                    display: false,
                },
                stacked: true
            }]
        },
        legend: {
            display: false
        }
    }

    renderChart = (group) => {
        let workload = this.state.workloads.find(x => x.groupId === group.value);
        return (<HorizontalBar key={workload.groupId} data={workload.chartData} options={workload.chartOptions} />)
    }

    render() {
        return (
            <div className="workload">
                <div className="title">
                    <h5>Workload</h5>
                    <div className="milestone">
                        <div className="icon-chevron-down arrow-left" onClick={this.onNextPeriod} />
                        {this.props.periods[this.state.currentPeriodIndex].name}
                        <div className="icon-chevron-down arrow-right" onClick={this.onPrevPeriod} />
                    </div>
                </div>
                <Multiselect className="react-select-container" classNamePrefix="react-select"
                    value={this.state.selectedGroups}
                    options={this.state.groupOptions}
                    onChange={this.handleGroupChange}
                />

                {this.state.selectedGroups.map(this.renderChart)}
            </div>
        );
    }
}