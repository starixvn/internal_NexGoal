import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Line from './Line';

export default class Timeline extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberPerformances: []
        };

        this.timelineWidth = React.createRef();
    }

    componentDidMount() {
        this.getMemberPerformanceTimeline();
        window.addEventListener('resize', this.updateDimensions);
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateDimensions);
    }

    updateDimensions = () => {
        this.setState({
            height: window.innerHeight,
            width: window.innerWidth
        });
    }

    getMemberPerformanceTimeline = () => {
        axios(resolveClientUrl('Member/Performance/Timeline'), {
            params: {
                pageIndex: 0,
                pageSize: 4
            }
        }).then((response) => {
            response.data.map(memberPerformance => {
                memberPerformance.pageIndex = 0;
                memberPerformance.pageSize = 4;
                memberPerformance.nextDisabled = memberPerformance.pageIndex === 0;
                memberPerformance.prevDisabled = (memberPerformance.pageIndex + 1) * memberPerformance.pageSize >= memberPerformance.totalItems;
            });

            this.setState({
                memberPerformances: response.data
            });
        }).catch(axiosError);
    }

    render() {
        const memberPerformances = this.state.memberPerformances;
        const timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;

        return (
            <div className="member-timeline" ref={this.timelineWidth}>
                {
                    memberPerformances.map((memberPerformance, index) => {
                        return (
                            <Line key={`memberTimeline${index}`}
                                data={memberPerformance}
                                timelineWidth={timelineWidth}
                            />
                        );
                    })
                }
            </div>
        );
    }
}