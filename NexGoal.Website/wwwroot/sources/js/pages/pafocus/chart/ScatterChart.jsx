import React, { Component } from 'react';
import { Scatter } from 'react-chartjs-2';

import { EmptyPlaceHolder } from '../../../constants';

export default class ScatterChart extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.period !== nextProps.period
            || this.props.focusEvaluationLevelOptions !== nextProps.focusEvaluationLevelOptions
            || this.props.pafocuses !== nextProps.pafocuses;
    }

    position = {
        None: 0,
        Left: 1,
        Bottom: 2,
        Center: 3,
        Over: 4
    };
    points = [];

    getFocusEvaluationLevel = value => {
        const index = this.props.focusEvaluationLevelOptions.findIndex(item => item.value.toString() === value);

        if (index >= 0) {
            return this.props.focusEvaluationLevelOptions[index].name;
        }

        return EmptyPlaceHolder;
    };

    checkExistPoint = (x, y) => {
        let isExisted = false;

        this.points.forEach(point => {
            if (point.x === x && point.y === y) {
                isExisted = true;
            }
        });

        return isExisted;
    };

    findNumberExist = point => {
        let number = 0;
        this.props.pafocuses.forEach(pafocus => {
            pafocus.members.forEach(member => {
                if (point.bonusGiven === member.bonusGiven && point.contribution === member.contribution) {
                    number++;
                }
            });
        });

        return number;
    };

    calculateThreshold = number => 1 / number;

    findUniquePoint = member => {
        let isExisted = true;
        let step = 0;
        const number = this.findNumberExist(member);
        const threshold = this.calculateThreshold(number);
        let contribution = member.contribution ? parseInt(member.contribution, 10) : null;
        let bonusGiven = member.bonusGiven ? parseInt(member.bonusGiven, 10) : null;

        while (isExisted) {
            isExisted = this.checkExistPoint(bonusGiven, contribution);
            if (isExisted) {
                if (step === this.position.Left) {
                    bonusGiven -= threshold;
                } else if (step === this.position.Bottom) {
                    contribution -= threshold;
                } else if (step === this.position.Center || step === this.position.Over) {
                    bonusGiven -= threshold;
                    contribution -= threshold;
                }

                step++;
                if (step > this.position.Over) {
                    step = this.position.None;
                }
            } else {
                this.points.push({ x: bonusGiven, y: contribution });
            }
        }

        return {
            x: bonusGiven,
            y: contribution
        };
    };

    renderReadyPromotion = value => {
        let text = EmptyPlaceHolder;

        if (value) {
            text = value === 'true' ? 'Yes' : 'No';
        }

        let html = text;
        if (value === 'true') {
            html = `<span style='color:#F0931F'>${text}</span>`;
        }

        return html;
    };

    getChartData = () => {
        this.points = [];
        const data = [];
        const colors = [];

        this.props.pafocuses.forEach(pafocus => {
            pafocus.members.forEach(member => {
                if (member.bonusGiven !== null && member.bonusGiven !== '') {
                    let color = '#56bde4';
                    if (member.isReadyPromotion === 'true') {
                        color = '#f0931f';
                    }

                    const description = `<span style="color:red">(*)</span>: ${this.getFocusEvaluationLevel(member.contribution)}
                        <br> <span style="color:red">(**)</span>: ${this.getFocusEvaluationLevel(member.bonusGiven)}
                        <br> Ready for Promotion: ' ${this.renderReadyPromotion(member.isReadyPromotion)}
                        <br> Job Title: ${member.employee.jobTitle}`;

                    const point = this.findUniquePoint(member);

                    colors.push(color);
                    data.push({
                        x: point.x,
                        y: point.y,
                        color: color,
                        name: member.employee.nickName,
                        description: description
                    });
                }
            });
        });

        return {
            datasets: [{
                data: data,
                pointBackgroundColor: colors
            }]
        };
    };

    getChartOptions = () => {
        return {
            layout: {
                padding: {
                    top: 40
                }
            },
            title: {
                display: true,
                text: this.props.period ? `Performance Focus of ${this.props.period.label}` : '',
                fontColor: '#333',
                fontSize: 14,
                padding: 20
            },
            responsive: true,
            showLine: true,
            tooltips: {
                enabled: false,
                custom: function (tooltipModel) {
                    let tooltipEl = document.getElementById('chartjs-tooltip');

                    if (!tooltipEl) {
                        tooltipEl = document.createElement('div');
                        tooltipEl.id = 'chartjs-tooltip';
                        document.body.appendChild(tooltipEl);
                    }

                    if (tooltipModel.opacity === 0) {
                        tooltipEl.style.opacity = 0;
                        return;
                    }

                    tooltipEl.classList.remove('above', 'below', 'no-transform');

                    if (tooltipModel.body) {
                        const titleLines = tooltipModel.title || [];
                        let innerHtml = '';

                        titleLines.forEach(function (title) {
                            innerHtml += title;
                        });

                        tooltipEl.innerHTML = innerHtml;
                    }

                    if (tooltipModel.yAlign) {
                        tooltipEl.classList.add(tooltipModel.yAlign);
                    } else {
                        tooltipEl.classList.add('no-transform');
                    }

                    const position = this._chart.canvas.getBoundingClientRect();
                    const styleLeft = position.left + tooltipModel.caretX;
                    const styleTop = position.top + tooltipModel.caretY;

                    tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                    tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                    tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                    tooltipEl.style.padding = `${tooltipModel.yPadding}px ${tooltipModel.xPadding}px`;
                    tooltipEl.style.opacity = 1;
                    tooltipEl.style.position = 'absolute';
                    tooltipEl.style.left = `${styleLeft}px`;
                    tooltipEl.style.top = `${styleTop}px`;
                },
                callbacks: {
                    title: function (tooltipItems, data) {
                        const dataset = data.datasets[tooltipItems[0].datasetIndex];
                        const currentValue = dataset.data[tooltipItems[0].index];

                        return `<div class="content" style="border: 1px solid ${currentValue.color}"><b>${currentValue.name}</b> <br />${currentValue.description}</div>`;
                    }
                }
            },
            scales: {
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Deserve Highest Reward (Increment And Bonus) (**)'
                    },
                    ticks: {
                        min: 0,
                        max: 5,
                        stepSize: 1
                    }
                }],
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Best Suited to Team Goals (*)'
                    },
                    ticks: {
                        min: 0,
                        max: 5,
                        stepSize: 1
                    }
                }]
            },
            legend: {
                display: false
            }
        };
    };

    render() {
        const data = this.getChartData();
        const options = this.getChartOptions();

        return (
            <div className="chart-pa-focus">
                <Scatter
                    width={600}
                    height={600}
                    data={data}
                    options={options}
                />
            </div>
        );
    }
}