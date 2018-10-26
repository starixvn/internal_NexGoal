import React, { Component } from "react";

import Node from './Node';

export default class Timeline extends Component {
    onClickPrevious = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex + 1;

        this.props.onPageChange(memberPerformance.employeeId, pageIndex);
    }

    onClickNext = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex - 1;

        this.props.onPageChange(memberPerformance.employeeId, pageIndex);
    }

    calculateNodePosition = (totalNode, index) => {
        let distanceFromRoot = (this.props.timelineWidth - 70) / (totalNode + 1);
        return Math.round((index + 1) * distanceFromRoot);
    }

    render() {
        const memberPerformance = this.props.data;
        const performanceRecords = memberPerformance.performanceRecords;
        const totalNodes = performanceRecords.length;

        return (
            <div className="member-performance">
                <div className="employee-info">
                    <a href="#" className="nick-name">{memberPerformance.nickName}</a>
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
                                    onClick={this.props.onSelectNode}
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