import React, { Component } from "react";
import TimelineNode from './TimelineNode';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.timelineWidth = React.createRef();
    }

    onClickPrevious = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex + 1;
        this.props.onPageChange(pageIndex);
    }

    onClickNext = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex - 1;
        this.props.onPageChange(pageIndex);
    }

    calculateNodePosition = (totalNode, index) => {
        let baseRight = 20;
        let timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;
        let distanceFromRoot = timelineWidth / (totalNode + 1);
        return Math.round(baseRight + (index + 1) * distanceFromRoot);
    }

    render() {
        const memberPerformance = this.props.data;
        const records = memberPerformance.records;
        const totalNodes = records.length;

        return (
            <div className="timeline">
                <div className="xaxes" ref={this.timelineWidth} />
                {
                    totalNodes > 0 &&
                    records.map((record, index) => {
                        let position = this.calculateNodePosition(totalNodes, index);
                        return (
                            <TimelineNode key={`timelineNode${record.performanceRecordId}`}
                                data={record}
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
        );
    }
}