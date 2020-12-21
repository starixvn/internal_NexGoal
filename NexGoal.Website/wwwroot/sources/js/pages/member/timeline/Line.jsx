import React, { Component } from 'react';
import isEqual from 'react-fast-compare';

import Node from './Node';
import { resolveClientUrl } from '../../../common-functions';

export default class Line extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    onClickPrevious = () => {
        const memberPerformance = this.props.data;
        if (!memberPerformance.prevDisabled) {
            const pageIndex = memberPerformance.pageIndex + 1;
            this.props.onPageChange(memberPerformance.employeeId, pageIndex);
        }
    };

    onClickNext = () => {
        const memberPerformance = this.props.data;
        if (!memberPerformance.nextDisabled) {
            const pageIndex = memberPerformance.pageIndex - 1;
            this.props.onPageChange(memberPerformance.employeeId, pageIndex);
        }
    };

    calculateNodePosition = (totalNodes, index) => {
        const nodeWidth = (this.props.timelineWidth - 10) / totalNodes;
        const startAt = (nodeWidth - 57) / 2;

        return index * nodeWidth + startAt;
    };

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
                            const position = this.calculateNodePosition(totalNodes, index);

                            return (
                                <Node key={`timelineNode${performanceRecord.performanceRecordId}`}
                                    data={performanceRecord}
                                    position={position}
                                    onClick={this.props.onNodeClick}
                                />
                            );
                        })
                    }

                    <span className={`button-icon icon-chevron-left previous ${memberPerformance.prevDisabled ? 'disabled' : ''}`}
                        onClick={this.onClickPrevious}
                    />
                    <span className={`button-icon icon-chevron-right next ${memberPerformance.nextDisabled ? 'disabled' : ''}`}
                        onClick={this.onClickNext}
                    />
                </div>
            </div>
        );
    }
}