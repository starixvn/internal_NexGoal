import React, { Component, Fragment } from 'react';
import axios from 'axios';
import isEqual from 'react-fast-compare';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/multi-select/multi-select';
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
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    componentDidUpdate(prevProps) {
        if (!isEqual(this.props.groupOptions, prevProps.groupOptions)) {
            this.updateFilter('groups', this.props.groupOptions);
        }

        if (!isEqual(this.props.currentPerformancePeriod, prevProps.currentPerformancePeriod)) {
            this.getMemberPerformanceTimeline(this.state.pageSize);
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateDimensions);
    }

    calculatePageSize = () => {
        const windowWidth = window.innerWidth;
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
        const pageSize = this.calculatePageSize();

        this.setState(prevState => {
            if (prevState.pageSize !== pageSize) {
                this.getMemberPerformanceTimeline(pageSize);

                return { pageSize };
            }

            return { pageSize };
        });
    }

    getMemberPerformanceTimeline = pageSize => {
        if (this.props.currentPerformancePeriod) {
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    periodId: this.props.currentPerformancePeriod.periodId,
                    pageIndex: 0,
                    pageSize: pageSize
                }
            }).then(response => {
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
        const memberPerformances = this.state.memberPerformances.slice();
        const index = memberPerformances.findIndex(memberPerformance => memberPerformance.employeeId === employeeId);

        if (index >= 0 && this.props.currentPerformancePeriod) {
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    periodId: this.props.currentPerformancePeriod.periodId,
                    employeeId: employeeId,
                    pageIndex: pageIndex,
                    pageSize: this.state.pageSize
                }
            }).then(response => {
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

    handleGroupChange = selectedOptions => {
        this.updateFilter('groups', selectedOptions);
    }

    groupMemberPerformances = records => {
        let memberPerformances = records ? records.slice() : [];

        memberPerformances = memberPerformances.reduce(function (groups, item) {
            const val = item['groupName'];
            groups[val] = groups[val] || [];
            groups[val].push(item);

            return groups;
        }, []);

        memberPerformances = Object.keys(memberPerformances).map(key => {
            const index = this.state.filter.groups.findIndex(g => g.label === key);
            const group = this.props.groupOptions.filter(x => x.label === key)[0];

            return {
                groupId: group ? group.value : '',
                groupName: key,
                isDisplay: index >= 0,
                records: memberPerformances[key]
            };
        });

        return memberPerformances.filter(group => group.isDisplay);
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

    calculateTimelineWidth = () => {
        const windowWidth = window.innerWidth;
        let timelineWidth = this.timelineEl.current ? this.timelineEl.current.offsetWidth : 0;

        if (windowWidth > 1600) {
            timelineWidth -= 122;
        } else {
            timelineWidth -= 112;
        }

        return timelineWidth;
    }

    render() {
        const { groupOptions, currentPerformancePeriod } = this.props;
        const groupPerformances = this.groupMemberPerformances(this.state.memberPerformances);
        const timelineWidth = this.calculateTimelineWidth();

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
                        groupPerformances.map(groupPerformance => {
                            return (
                                <Fragment key={`groupPerformance${groupPerformance.groupId}`}>
                                    {this.renderGroupName(groupPerformance.groupId, groupPerformance.groupName)}
                                    <div id={`groupTimeline${groupPerformance.groupId}`}
                                        className={groupOptions && groupOptions.length > 1 ? 'collapse' : ''}
                                        aria-expanded="false"
                                    >
                                        {
                                            groupPerformance.records.map(memberPerformance => {
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