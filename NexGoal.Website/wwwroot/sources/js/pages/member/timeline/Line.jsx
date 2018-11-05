import React, { Component } from 'react';

import Node from './Node';
import { resolveClientUrl } from '../../../common-functions';

export default class Timeline extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.timelineWidth !== nextProps.timelineWidth
            || JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

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
        let distanceFromRoot = (this.props.timelineWidth - 80) / (totalNode + 1);
        return Math.round((index + 1) * distanceFromRoot);
    }

    render() {
        const memberPerformance = this.props.data;
        const performanceRecords = memberPerformance.performanceRecords;
        const totalNodes = performanceRecords.length;

        return (
            <div className="member-performance">
                <div className="employee-info">
                    <a href={resolveClientUrl(`member/details/${memberPerformance.employeeId}`)} className="nick-name">{memberPerformance.nickName}</a>
                    {
                        memberPerformance.grade &&
                        <div>PA: {memberPerformance.grade.name}</div>
                    }
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
                                    onClick={this.props.onNodeClick}
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