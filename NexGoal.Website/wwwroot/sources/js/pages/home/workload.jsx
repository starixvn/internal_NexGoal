import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import { Pie } from 'react-chartjs-2';
import 'chartjs-plugin-datalabels';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

export default class WorkLoad extends Component {
    constructor(props) {
        super(props);

        this.state = {
            workLoad: []
        }
    }

    componentDidMount() {
        this.getWorkLoad();
    }

    getWorkLoad = () => {
        axios.get(resolveClientUrl('Home/GetWorkLoad'))
            .then(response => {
                this.setState({ workLoad: response.data });
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
        const data = {
            datasets: [{
                data: this.state.workLoad.map(e => e.totalHours),
                backgroundColor: this.generateColors(this.state.workLoad.length),
            }],

            labels: this.state.workLoad.map(e => e.name)
        };

        const options = {
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
        return (
            <div style={{ width: '500px', height: '400px' }} >
                <Pie data={data} options={options} />
            </div>
        );
    }
}

ReactDOM.render(<WorkLoad />, document.getElementById("workload-wrapper"));