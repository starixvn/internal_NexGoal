import React, { Component } from 'react';
import isEqual from 'react-fast-compare';

import { calculateTimelineNode } from '../../../common-functions';
import TimelineNode from './TimelineNode';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            totalNodes: calculateTimelineNode(),
            windowWidth: window.innerWidth
        };

        this.timelineWidth = React.createRef();
    }

    componentDidMount() {
        window.addEventListener('resize', this.updateDimensions);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateDimensions);
    }

    updateDimensions = () => {
        this.setState((prevState, prevProps) => {
            const nodes = calculateTimelineNode();
            if (nodes !== prevState.totalNodes) {
                this.props.onPageChange(prevProps.data.pageIndex, nodes);
                return {
                    totalNodes: nodes,
                    windowWidth: window.innerWidth
                };
            }

            return { windowWidth: window.innerWidth };
        });
    };

    onClickPrevious = () => {
        const memberPerformance = this.props.data;
        if (!memberPerformance.prevDisabled) {
            const pageIndex = memberPerformance.pageIndex + 1;
            this.props.onPageChange(pageIndex, this.state.totalNodes);
        }
    };

    onClickNext = () => {
        const memberPerformance = this.props.data;
        if (!memberPerformance.nextDisabled) {
            const pageIndex = memberPerformance.pageIndex - 1;
            this.props.onPageChange(pageIndex, this.state.totalNodes);
        }
    };

    calculateNodePosition = (totalNodes, index) => {
        const baseRight = 20;
        const timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;
        const distanceFromRoot = timelineWidth / (totalNodes + 1);
        return Math.round(baseRight + (index + 1) * distanceFromRoot);
    };

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
                        const position = this.calculateNodePosition(totalNodes, index);
                        return (
                            <TimelineNode key={`timelineNode${record.performanceRecordId}`}
                                data={record}
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
        );
    }
}