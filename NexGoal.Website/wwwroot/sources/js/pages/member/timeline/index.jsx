import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Line from './Line';

export default class Timeline extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberPerformances: [],
            pageSize: window.innerWidth > 1600 ? 4 : 2
        };

        this.timelineWidth = React.createRef();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.currentPerformancePeriod) !== JSON.stringify(nextProps.currentPerformancePeriod)
            || this.state.pageSize !== nextState.pageSize
            || JSON.stringify(this.state.memberPerformances) !== JSON.stringify(nextState.memberPerformances)) {
            return true;
        }

        return false;
    }

    componentDidUpdate(prevProps) {
        if (JSON.stringify(this.props.currentPerformancePeriod) !== JSON.stringify(prevProps.currentPerformancePeriod)) {
            this.getMemberPerformanceTimeline();
        }
    }

    getMemberPerformanceTimeline = () => {
        console.log(this.props.currentPerformancePeriod);
        axios(resolveClientUrl('Member/Performance/Timeline'), {
            params: {
                periodId: this.props.currentPerformancePeriod ? this.props.currentPerformancePeriod.periodId : null,
                pageIndex: 0,
                pageSize: this.state.pageSize
            }
        }).then((response) => {
            response.data.map(memberPerformance => {
                memberPerformance.pageIndex = 0;
                memberPerformance.nextDisabled = memberPerformance.pageIndex === 0;
                memberPerformance.prevDisabled = (memberPerformance.pageIndex + 1) * this.state.pageSize >= memberPerformance.totalItems;
            });

            this.setState({
                memberPerformances: response.data
            });
        }).catch(axiosError);
    }

    handlePageChange = (employeeId, pageIndex) => {
        let memberPerformances = this.state.memberPerformances.slice();
        let index = memberPerformances.findIndex(memberPerformance => memberPerformance.employeeId === employeeId);

        if (index >= 0) {
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    employeeId: employeeId,
                    pageIndex: pageIndex,
                    pageSize: this.state.pageSize
                }
            }).then((response) => {
                response.data[0].pageIndex = pageIndex;
                response.data[0].nextDisabled = pageIndex === 0;
                response.data[0].prevDisabled = (pageIndex + 1) * this.state.pageSize >= response.data[0].totalItems;

                memberPerformances[index] = response.data[0];

                this.setState({
                    memberPerformances: memberPerformances
                });
            }).catch(axiosError);
        }
    }

    render() {
        let currentPerformancePeriod = this.props.currentPerformancePeriod;
        const memberPerformances = this.state.memberPerformances;
        const timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;

        return (
            <Fragment>
                <h5>Member Performance{currentPerformancePeriod ? ` - ${currentPerformancePeriod.name}` : ''}</h5>
                <div className="member-timeline" ref={this.timelineWidth}>
                    {
                        memberPerformances.map((memberPerformance, index) => {
                            return (
                                <Line key={`memberTimeline${index}`}
                                    data={memberPerformance}
                                    timelineWidth={timelineWidth}
                                    onPageChange={this.handlePageChange}
                                    onNodeClick={this.props.onNodeClick}
                                />
                            );
                        })
                    }
                </div>
            </Fragment>
        );
    }
}