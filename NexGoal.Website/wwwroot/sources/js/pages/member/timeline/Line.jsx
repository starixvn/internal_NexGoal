import React, { Component } from "react";
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Node from './Node';

export default class Timeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            timelineWidth: 0,
            memberPerformances: [],
            nextDisabled: true,
            prevDisabled: false,
            pageIndex: 0
        };
    }

    onClickPrevious = () => {
        this.setState(prevState => {
            return { pageIndex: prevState.pageIndex + 1 };
        });
    }

    onClickNext = () => {
        this.setState(prevState => {
            return { pageIndex: prevState.pageIndex - 1 };
        });
    }

    calculateNodePosition = (totalNode, index) => {
        let baseRight = 20;
        let distanceFromRoot = this.props.timelineWidth / (totalNode + 1);
        return Math.round(baseRight + (index + 1) * distanceFromRoot);
    }

    render() {
        const memberPerformance = this.props.data;
        const performanceRecords = memberPerformance.performanceRecords;
        const totalNodes = performanceRecords.length;

        return (
            <div className="member-performance">
                <div className="employee-info">
                    {memberPerformance.nickName}
                    <div>PA:</div>
                </div>
                <div className="timeline">
                    <div className="xaxes" />
                    {
                        totalNodes > 0 &&
                        performanceRecords.map((performanceRecord, index) => {
                            let position = this.calculateNodePosition(totalNodes, index);
                            return (
                                <Node key={`timelineNode${performanceRecord.performanceRecordId}`}
                                    data={performanceRecord}
                                    position={position}
                                    onClick={this.props.onItemClick}
                                />
                            );
                        })
                    }

                    <button className={memberPerformance.prevDisabled ? "previous disabled" : "previous"}
                        onClick={this.onClickPrevious}
                        disabled={memberPerformance.prevDisabled}
                    >
                        <span className="button-icon icon-chevron-left" />
                    </button>
                    <button className={memberPerformance.nextDisabled ? "next disabled" : "next"}
                        onClick={this.onClickNext}
                        disabled={memberPerformance.nextDisabled}
                    >
                        <span className="button-icon icon-chevron-right" />
                    </button>
                </div>
            </div>
        );
    }
}