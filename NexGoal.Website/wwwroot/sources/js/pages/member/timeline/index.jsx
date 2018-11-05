import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/multi-select/index';
import Line from './Line';

export default class Timeline extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                groups: []
            },
            memberPerformances: [],
            pageSize: window.innerWidth > 1600 ? 4 : 2
        };

        this.timelineWidth = React.createRef();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.groupOptions) !== JSON.stringify(nextProps.groupOptions)
            || JSON.stringify(this.props.currentPerformancePeriod) !== JSON.stringify(nextProps.currentPerformancePeriod)
            || JSON.stringify(this.state.filter) !== JSON.stringify(nextState.filter)
            || this.state.pageSize !== nextState.pageSize
            || JSON.stringify(this.state.memberPerformances) !== JSON.stringify(nextState.memberPerformances)) {
            return true;
        }

        return false;
    }

    componentDidUpdate(prevProps) {
        if (JSON.stringify(this.props.groupOptions) !== JSON.stringify(prevProps.groupOptions)) {
            this.updateFilter('groups', this.props.groupOptions);
        }

        if (JSON.stringify(this.props.currentPerformancePeriod) !== JSON.stringify(prevProps.currentPerformancePeriod)) {
            this.getMemberPerformanceTimeline();
        }
    }

    getMemberPerformanceTimeline = () => {
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

    updateFilter = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
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

    handleGroupChange = (selectedOptions) => {
        this.updateFilter('groups', selectedOptions);
    }

    groupMemberPerformances = (records) => {
        let memberPerformances = records ? records.slice() : [];
        memberPerformances = memberPerformances.reduce(function (groups, item) {
            const val = item['groupName'];
            groups[val] = groups[val] || [];
            groups[val].push(item);

            return groups;
        }, []);

        memberPerformances = Object.keys(memberPerformances).map(key => {
            let index = this.state.filter.groups.findIndex(group => group.label === key);

            return {
                groupName: key,
                isDisplay: index >= 0,
                records: memberPerformances[key]
            };
        });

        return memberPerformances.filter(group => group.isDisplay);
    };

    render() {
        let groupOptions = this.props.groupOptions;
        let currentPerformancePeriod = this.props.currentPerformancePeriod;
        let groupPerformances = this.groupMemberPerformances(this.state.memberPerformances);
        let timelineWidth = this.timelineWidth.current ? this.timelineWidth.current.offsetWidth : 0;

        return (
            <Fragment>
                <h5>Member Performance{currentPerformancePeriod ? ` - ${currentPerformancePeriod.name}` : ''}</h5>
                {
                    groupOptions && groupOptions.length > 1 &&
                    <Fragment>
                        <div>{getResource('Team')}</div>
                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                            value={this.state.filter.groups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </Fragment>
                }
                <div className="member-timeline" ref={this.timelineWidth}>
                    {
                        groupPerformances.map((groupPerformance, index) => {
                            return (
                                <Fragment key={`groupPerformance${index}`}>
                                    <div>{groupPerformance.groupName}</div>
                                    {
                                        groupPerformance.records.map((memberPerformance) => {
                                            return (
                                                <Line key={`memberTimeline${memberPerformance.employeeId}`}
                                                    data={memberPerformance}
                                                    timelineWidth={timelineWidth}
                                                    onPageChange={this.handlePageChange}
                                                    onNodeClick={this.props.onNodeClick}
                                                />
                                            );
                                        })
                                    }
                                </Fragment>
                            );
                        })
                    }
                </div>
            </Fragment>
        );
    }
}