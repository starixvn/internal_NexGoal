import React, { Component } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class MemberWorkload extends Component {
    constructor(props) {
        super(props);
        this.state = {
            workloads: null,
            team: '',
            currentPeriodIndex: 0
        };
    }

    componentDidMount() {
        this.getMembersWorkloads();
    }

    getMembersWorkloads = function () {
        axios.get(resolveClientUrl('Home/GetMembersWorkload'), {
            params: {
                periodId: this.props.periods[this.state.currentPeriodIndex].periodId
            }
        }).then(response => {
            this.setState(
                {
                    workloads: this.generateDataSets(response.data.employeeWorkloads),
                    team: response.data.groupName
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

    generateDataSets = (workloads) => {
        let labels = workloads.map((member) => member.nickName);
        let datasets = [];
        let projects = workloads[1].workloads.map((project) => project.name);

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

    render() {
        const workloads = this.state.workloads;
        const options = this.chartOptions;
        const teamName = this.state.team;

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
                {workloads && <HorizontalBar data={workloads} options={options} />}
            </div>
        );
    }
}