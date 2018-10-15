import React, { Component } from "react";
import TimelineNode from './TimelineNode';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            timelineWidth: 0,
            data: [],
            isShowComment: false
        };

        this.timelineWidth = React.createRef();
        this.onClickPrevious = this.onClickPrevious.bind(this);
        this.onClickNext = this.onClickNext.bind(this);
    }

    componentDidMount() {
        axios(resolveClientUrl('Self/Performance/Index')).then((response) => {
            this.setState({
                data: response.data,
                timelineWidth: this.timelineWidth.current.offsetWidth
            });
        }).catch(axiosError);
    }

    calculateNodePosition = (totalNode, index) => {
        let baseLeft = 0;
        let timelineWidth = this.state.timelineWidth;
        let distanceFromRoot = timelineWidth / (totalNode + 1);
        return Math.round(baseLeft + (index + 1) * distanceFromRoot);
    }

    render() {
        const data = this.state.data;
        return (
            <div className="time-line">
                <div className="xaxes" ref={this.timelineWidth} />
                {
                    data.length > 0 &&
                    data.map((pci, index) => {
                        let position = this.calculateNodePosition(data.length, index);
                        return (
                            <TimelineNode data={pci} position={position} />
                        );
                    })
                }

                <div className="previous" onClick={this.onClickPrevious}><span className="button-icon icon-chevron-left" /></div>
                <div className="next" onClick={this.onClickNext}><span className="button-icon icon-chevron-right" /></div>
            </div>
        );
    }
}