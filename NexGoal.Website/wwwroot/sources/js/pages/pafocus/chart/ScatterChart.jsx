import React, { Component } from 'react';
import { Scatter } from 'react-chartjs-2';

export default class ScatterChart extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.period !== nextProps.period
            || this.props.focusEvaluationLevelOptions !== nextProps.focusEvaluationLevelOptions
            || this.props.pafocuses !== nextProps.pafocuses) {
            return true;
        }

        return false;
    }

    position = {
        None: 0,
        Left: 1,
        Bottom: 2,
        Center: 3,
        Over: 4
    };
    points = [];

    getFocusEvaluationLevel = (value) => {
        let index = this.props.focusEvaluationLevelOptions.findIndex(item => item.value.toString() === value);

        if (index >= 0) {
            return this.props.focusEvaluationLevelOptions[index].name;
        }

        return '-';
    }

    checkExistPoint = (x, y) => {
        var isExisted = false;
        this.points.forEach((point) => {
            if (point.x === x && point.y === y) {
                isExisted = true;
            }
        });

        return isExisted;
    }

    findNumberExist = (point) => {
        let number = 0;
        this.props.pafocuses.forEach((pafocus) => {
            pafocus.members.forEach((member) => {
                if (point.bonusGiven === member.bonusGiven && point.contribution === member.contribution) {
                    number++;
                }
            });
        });

        return number;
    }

    calculateThreshold = (number) => {
        return 1 / number;
    }

    findUniquePoint = (member) => {
        let isExisted = true;
        let step = 0;
        let number = this.findNumberExist(member);
        let threshold = this.calculateThreshold(number);
        let contribution = member.contribution ? parseInt(member.contribution) : null;
        let bonusGiven = member.bonusGiven ? parseInt(member.bonusGiven) : null;

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
    }

    renderReadyPromotion = (value) => {
        let text = value === null ? '-' : value === 'true' ? 'Yes' : 'No';
        let html = text;
        if (value === 'true') {
            html = '<span style="color:#F0931F">' + text + '</span>';
        }

        return html;
    }

    getChartData = () => {
        this.points = [];
        let data = [];
        let colors = [];

        this.props.pafocuses.forEach((pafocus) => {
            pafocus.members.forEach((member) => {
                if (member.bonusGiven !== null && member.bonusGiven !== '') {
                    let color = '#56bde4';
                    if (member.isReadyPromotion === 'true') {
                        color = '#f0931f';
                    }

                    let description = '<span style="color:red">(*)</span>: ' + this.getFocusEvaluationLevel(member.contribution)
                        + '<br> <span style="color:red">(**)</span>: ' + this.getFocusEvaluationLevel(member.bonusGiven)
                        + '<br> Ready for Promotion: ' + this.renderReadyPromotion(member.isReadyPromotion)
                        + '<br> Job Title: ' + member.employee.jobTitle;

                    let point = this.findUniquePoint(member);

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
    }

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
            responsive: false,
            showLine: true,
            tooltips: {
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
                        let titleLines = tooltipModel.title || [];
                        let innerHtml = '';

                        titleLines.forEach(function (title) {
                            innerHtml += title;
                        });

                        tooltipEl.innerHTML = innerHtml;
                    }

                    // `this` will be the overall tooltip
                    let position = this._chart.canvas.getBoundingClientRect();

                    // Display, position, and set styles for font
                    tooltipEl.style.opacity = 1;
                    tooltipEl.style.position = 'absolute';
                    tooltipEl.style.left = position.left + tooltipModel.caretX + 'px';
                    tooltipEl.style.top = position.top + tooltipModel.caretY + 'px';
                    tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                    tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                    tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                    tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
                },
                callbacks: {
                    title: function (tooltipItems, data) {
                        let dataset = data.datasets[tooltipItems[0].datasetIndex];
                        let currentValue = dataset.data[tooltipItems[0].index];

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
        let data = this.getChartData();
        let options = this.getChartOptions();

        return (
            <Scatter
                width={600}
                height={600}
                data={data}
                options={options}
            />
        );
    }
}