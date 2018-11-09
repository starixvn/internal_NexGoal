import React, { Component } from "react";
import TimelineNode from './TimelineNode';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            totalNodes: 8,
            windowWidth: window.innerWidth
        };

        this.timelineWidth = React.createRef();
        this.updateDimensions = this.updateDimensions.bind(this);
    }

    componentDidMount() {
        this.updateDimensions();
        const pageIndex = this.props.data.pageIndex;
        this.props.onPageChange(pageIndex, this.state.totalNodes);
        window.addEventListener("resize", this.updateDimensions);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)
            || nextState.totalNode !== this.state.totalNodes
            || nextState.windowWidth !== this.state.windowWidth) {
            return true;
        }

        return false;
    }

    componentWillUnmount() {
        window.removeEventListener("resize", this.updateDimensions);
    }

    updateDimensions() {
        this.setState((prevState, prevProps) => {
            let nodes = this.calculateNodes();
            if (nodes !== prevState.totalNodes) {
                this.props.onPageChange(prevProps.data.pageIndex, nodes);
                return {
                    totalNodes: nodes,
                    windowWidth: window.innerWidth
                };
            }

            return { windowWidth: window.innerWidth };
        });
    }

    calculateNodes = () => {
        const width = window.innerWidth;
        if (width > 1024 && width < 1366) return 3;
        if (width >= 1366 && width <= 1440) return 5;
        return 8;
    }

    onClickPrevious = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex + 1;
        this.props.onPageChange(pageIndex, this.state.totalNodes);
    }

    onClickNext = () => {
        const memberPerformance = this.props.data;
        const pageIndex = memberPerformance.pageIndex - 1;
        this.props.onPageChange(pageIndex, this.state.totalNodes);
    }

    calculateNodePosition = (totalNodes, index) => {
        let baseRight = 20;
        let timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;
        let distanceFromRoot = timelineWidth / (totalNodes + 1);
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
        );
    }
}