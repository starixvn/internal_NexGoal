import React, { Component } from "react";
import TimelineNode from './TimelineNode';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            timelineWidth: 0,
            performanceRecords: [],
            nextDisabled: true,
            prevDisabled: false,
            currentPage: 1,
        };

        this.timelineWidth = React.createRef();
    }

    componentDidMount() {
        this.getPCI();
    }

    getPCI = () => {
        axios(resolveClientUrl('Self/Performance/GetByEmployee'), {
            params: {
                pageIndex: this.state.currentPage,
                itemPerPage: this.props.itemPerPage
            }
        }).then((response) => {
            this.setState({
                performanceRecords: response.data.performanceRecord,
                timelineWidth: this.timelineWidth.current.offsetWidth,
                prevDisabled: response.data.isOverPCI,
                nextDisabled: this.state.currentPage === 1
            });
        }).catch(axiosError);
    }

    onClickPrevious = () => {
        this.setState({
            currentPage: this.state.currentPage + 1,
        }, () => {
            this.getPCI();
        });
    }

    onClickNext = () => {
        this.setState({
            currentPage: this.state.currentPage - 1
        }, () => {
            this.getPCI();
        });
    }

    calculateNodePosition = (totalNode, index) => {
        let baseLeft = 60;
        let timelineWidth = this.state.timelineWidth;
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
                            <TimelineNode key={index} data={reccord} position={position} onClick={this.props.onItemClick} />
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