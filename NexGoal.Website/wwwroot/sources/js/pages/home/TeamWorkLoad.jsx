import React, { Component } from 'react';
import axios from 'axios';
import { HorizontalBar } from 'react-chartjs-2';
import 'chartjs-plugin-datalabels';

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class TeamWorkLoad extends Component {
    constructor(props) {
        super(props);
        this.state = {
            workloads: [],
            team: '',
            xAxesMaxValue: 0
        };
    }
    componentDidMount() {
        this.getMembersWorkloads();
    }

    getMembersWorkloads = function () {
        axios.get(resolveClientUrl('Home/GetMembersWorkLoad'))
            .then(response => {
                this.setState(
                    {
                        workloads: this.generateDataSets(response.data.employeeWorkloads),
                        team: response.data.groupName,
                        xAxesMaxValue: this.calculatexAxesMaxValue(response.data.employeeWorkloads)
                    });
            }).catch(axiosError);
    }

    calculatexAxesMaxValue = (workloads) => {
        let teamTotalHours = [];
        for (var i = 0; i < workloads.length; i++) {
            let memberWorkloads = workloads[i].workloads;
            let total = memberWorkloads.reduce((hours, workload, index, memberWorkloads) => {
                return hours += workload.totalHours;
            }, 0);
            teamTotalHours.push(total);
        }

        let maxValue = Math.max(...teamTotalHours);

        return Math.round((maxValue + 50) / 100) * 100;
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

    getChartOptions = () => {
        const xAxesMaxValue = this.state.xAxesMaxValue;
        return {
            tooltips: {
                enable: true,
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
            hover: {
                animationDuration: 0,
                onHover: function (e) {
                    var point = this.getElementAtEvent(e);
                    if (point.length) e.target.style.cursor = 'pointer';
                    else e.target.style.cursor = 'default';
                }
            },
            scales: {
                xAxes: [{
                    ticks: {
                        beginAtZero: true,
                        fontFamily: "'Open Sans Bold', sans-serif",
                        fontSize: 10
                    },
                    scaleLabel: {
                        display: true
                    },
                    gridLines: {
                    },
                    stacked: true
                }],
                yAxes: [{
                    gridLines: {
                        display: false,
                        color: "#fff",
                        zeroLineColor: "#fff",
                        zeroLineWidth: 0
                    },
                    ticks: {
                        fontFamily: "'Open Sans Bold', sans-serif",
                        fontSize: 13,
                        beginAtZero: true
                    },
                    stacked: true,
                    categoryPercentage: 0.8
                }]
            },
            legend: {
                display: false
            },
            plugins: {
                datalabels: {
                    formatter: function (value, data) {
                        let percentage = Math.round(value / xAxesMaxValue * 100);
                        return percentage > 10 ? value + 'h' : '';
                    },
                    fontSize: 10,
                    color: "#fff"
                }
            }
        };
    };

    render() {
        const workloads = this.state.workloads;
        const options = this.getChartOptions();
        const teamName = this.state.team;
        return (
            <div style={{ width: "500px", padding: "20px" }}>
                {
                    workloads &&
                    <div>
                        <p style={{
                            textTransform: "uppercase",
                            fontWeight: "bold",
                            fontSize: "20px",
                            padding: "10px"
                        }}
                        >{teamName}</p>
                        <HorizontalBar data={workloads} options={options} />
                    </div>
                }
            </div>
        );
    }
}