import React, { Component } from "react";
import TimelineNode from './TimelineNode';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';

const itemPerPage = window.innerWidth > 1440 ? 8 : 5;

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            timelineWidth: 0,
            performanceRecords: [],
            nextDisabled: true,
            prevDisabled: false,
            pageIndex: 1,
            isOverPCI: false
        };

        this.timelineWidth = React.createRef();
        this.onClickPrevious = this.onClickPrevious.bind(this);
        this.onClickNext = this.onClickNext.bind(this);
    }

    componentDidMount() {
        this.getPCI();
        this.setTimelineWidth();
    }

    setTimelineWidth = () => {
        this.setState({ timelineWidth: this.timelineWidth.current.offsetWidth });
    }

    getPCI = () => {
        axios(resolveClientUrl('Self/Performance/GetByEmployee'), {
            params: {
                pageIndex: this.state.pageIndex,
                itemPerPage: itemPerPage
            }
        }).then((response) => {
            this.setState({
                performanceRecords: response.data.performanceRecord,
                timelineWidth: this.timelineWidth.current.offsetWidth,
                prevDisabled: response.data.isOverPCI
            });
        }).catch(axiosError);
    }

    onClickPrevious = () => {
        this.setState({
            pageIndex: this.state.pageIndex + 1,
            nextDisabled: false
        }, () => {
            this.getPCI();
        });
    }

    onClickNext = () => {
        this.setState({
            pageIndex: this.state.pageIndex - 1
        }, () => {
            if (this.state.pageIndex === 1) {
                this.setState({
                    nextDisabled: true
                });
            }
            this.getPCI();
        });
    }

    calculateNodePosition = (totalNode, index) => {
        let baseLeft = 60; // calculate from padding-left
        let timelineWidth = this.state.timelineWidth;
        console.log(timelineWidth);
        let distanceFromRoot = timelineWidth / (totalNode + 1);
        return Math.round(baseLeft + (index + 1) * distanceFromRoot);
    }

    render() {
        const records = this.state.performanceRecords;
        return (
            <div className="time-line">
                <div className="xaxes" ref={this.timelineWidth} />
                {
                    records.length > 0 &&
                    records.map((reccord, index) => {
                        let position = this.calculateNodePosition(records.length, index);
                        return (
                            <TimelineNode key={index} data={reccord} position={position} />
                        );
                    })
                }

                <button className={this.state.prevDisabled ? "previous disabled" : "previous"}
                    onClick={this.onClickPrevious}
                    disabled={this.state.prevDisabled}
                >
                    <span className="button-icon icon-chevron-left" />
                </button>
                <button className={this.state.nextDisabled ? "next disabled" : "next"}
                    onClick={this.onClickNext}
                    disabled={this.state.nextDisabled}
                >
                    <span className="button-icon icon-chevron-right" />
                </button>
            </div>
        );
    }
}