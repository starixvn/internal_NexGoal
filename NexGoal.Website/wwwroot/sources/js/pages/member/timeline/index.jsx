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
            pageSize: this.calculatePageSize()
        };

        this.timelineEl = React.createRef();
    }

    componentDidMount() {
        window.addEventListener('resize', this.updateDimensions);
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
            this.getMemberPerformanceTimeline(this.state.pageSize);
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateDimensions);
    }

    calculatePageSize = () => {
        let windowWidth = window.innerWidth;
        let pageSize;

        if (windowWidth > 1600) {
            pageSize = 4;
        } else if (windowWidth >= 1280 && windowWidth <= 1600) {
            pageSize = 3;
        } else {
            pageSize = 2;
        }

        return pageSize;
    }

    updateDimensions = () => {
        let pageSize = this.calculatePageSize();

        this.setState((prevState, prevProps) => {
            if (prevState.pageSize !== pageSize) {
                this.getMemberPerformanceTimeline(pageSize);

                return { pageSize };
            }

            return { pageSize };
        });
    }

    getMemberPerformanceTimeline = (pageSize) => {
        if (this.props.currentPerformancePeriod) {
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    periodId: this.props.currentPerformancePeriod.periodId,
                    pageIndex: 0,
                    pageSize: pageSize
                }
            }).then((response) => {
                response.data.map(memberPerformance => {
                    memberPerformance.pageIndex = 0;
                    memberPerformance.nextDisabled = memberPerformance.pageIndex === 0;
                    memberPerformance.prevDisabled = (memberPerformance.pageIndex + 1) * pageSize >= memberPerformance.totalItems;
                });

                this.setState({
                    memberPerformances: response.data
                });
            }).catch(axiosError);
        }
    }

    updateFilter = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handlePageChange = (employeeId, pageIndex) => {
        let memberPerformances = this.state.memberPerformances.slice();
        let index = memberPerformances.findIndex(memberPerformance => memberPerformance.employeeId === employeeId);

        if (index >= 0 && this.props.currentPerformancePeriod) {
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    periodId: this.props.currentPerformancePeriod.periodId,
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

        memberPerformances = this.state.filter.groups.map(group => {
            return {
                groupId: group.value,
                groupName: group.label,
                records: memberPerformances.filter(record => record.groupName === group.label)
            };
        });

        return memberPerformances;
    };

    renderGroupName = (groupId, groupName) => {
        if (this.props.groupOptions && this.props.groupOptions.length > 1) {
            return (
                <div className="group-name collapsed"
                    data-toggle="collapse"
                    data-target={`#groupTimeline${groupId}`}
                    aria-expanded="false"
                >
                    <span className="icon-minus" /> {groupName}
                </div>
            );
        }

        return <div className="group-name">{groupName}</div>;
    }

    canculateTimelineWidth = () => {
        let windowWidth = window.innerWidth;
        let timelineWidth = this.timelineEl.current ? this.timelineEl.current.offsetWidth : 0;

        if (windowWidth > 1600) {
            timelineWidth -= 112;
        } else {
            timelineWidth -= 102;
        }

        return timelineWidth;
    }

    render() {
        let groupOptions = this.props.groupOptions;
        let currentPerformancePeriod = this.props.currentPerformancePeriod;
        let groupPerformances = this.groupMemberPerformances(this.state.memberPerformances);
        let timelineWidth = this.canculateTimelineWidth();

        return (
            <Fragment>
                <h5 className="title">Member Performance{currentPerformancePeriod ? ` - ${currentPerformancePeriod.name}` : ''}</h5>
                {
                    groupOptions && groupOptions.length > 1 &&
                    <div className="ddl-team">
                        <div className="team">{getResource('Team')}</div>
                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                            value={this.state.filter.groups}
                            options={groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                }
                <div className="member-timeline" ref={this.timelineEl}>
                    {
                        groupPerformances.map((groupPerformance) => {
                            return (
                                <Fragment key={`groupPerformance${groupPerformance.groupId}`}>
                                    {this.renderGroupName(groupPerformance.groupId, groupPerformance.groupName)}
                                    <div id={`groupTimeline${groupPerformance.groupId}`}
                                        className={groupOptions && groupOptions.length > 1 ? 'collapse' : ''}
                                        aria-expanded="false"
                                    >
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
                                    </div>
                                </Fragment>
                            );
                        })
                    }
                </div>
            </Fragment>
        );
    }
}