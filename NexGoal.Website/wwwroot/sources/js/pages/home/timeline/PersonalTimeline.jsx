import React, { Component } from "react";
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import TimelineNode from './TimelineNode';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            timelineWidth: 0,
            performanceRecords: [],
            nextDisabled: true,
            prevDisabled: false,
            pageIndex: 0
        };

        this.timelineWidth = React.createRef();
    }

    componentDidMount() {
        this.getPCI();
    }

    getPCI = () => {
        axios.get(resolveClientUrl('Self/Performance/GetByEmployee'), {
            params: {
                pageIndex: this.state.pageIndex,
                pageSize: this.props.itemPerPage
            }
        }).then((response) => {
            this.setState({
                performanceRecords: response.data.performanceRecord,
                timelineWidth: this.timelineWidth.current.offsetWidth,
                prevDisabled: response.data.isOverPCI,
                nextDisabled: this.state.pageIndex === 0
            });
        }).catch(axiosError);
    }

    onClickPrevious = () => {
        this.setState(prevState => {
            return { pageIndex: prevState.pageIndex + 1 };
        }, () => {
            this.getPCI();
        });
    }

    onClickNext = () => {
        this.setState(prevState => {
            return { pageIndex: prevState.pageIndex - 1 };
        }, () => {
            this.getPCI();
        });
    }

    calculateNodePosition = (totalNode, index) => {
        let baseRight = 20;
        let timelineWidth = this.state.timelineWidth;
        let distanceFromRoot = timelineWidth / (totalNode + 1);
        return Math.round(baseRight + (index + 1) * distanceFromRoot);
    }

    render() {
        const records = this.state.performanceRecords;

        return (
            <div className="time-line">
                <div className="xaxes" ref={this.timelineWidth} />
                {
                    records.length > 0 &&
                    records.map((record, index) => {
                        let position = this.calculateNodePosition(records.length, index);
                        return (
                            <TimelineNode key={`timeline${index}`} data={record} position={position} onClick={this.props.onItemClick} />
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