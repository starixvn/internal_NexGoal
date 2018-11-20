import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';
import { Tabs, Tab } from 'react-bootstrap';

import { axiosError, resolveClientUrl } from '../../common-functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import getResource from '../../resourceHelper';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import NominationResult from '../../shared/components/NominationResult';
import CreateMemberPerformance from './CreateMemberPerformance';
import EditMemberPerformance from './EditMemberPerformance';
import MemberWorkload from './MemberWorkload';
import Timeline from './timeline/index';
import Nomination from './nomination/Nomination';
import MemberPerformanceAppraisal from './performance-appraisal/MemberPerformanceAppraisal';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeOptions: [],
            myGroupOptions: [],
            performanceGradeOptions: [],
            projectOptions: [],
            memberPerformances: [],
            memberPerformanceAppraisals: [],
            memberNominations: [],
            periods: [],
            currentPerformancePeriod: null,
            displayedMemberPerformances: [],
            editMemberPerformance: {
                isShowModal: false,
                performanceRecordId: 0
            },
            currentUser: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src'),
                isNominator: document.getElementById('isNominator').checked,
                isReviewer: document.getElementById('isReviewer').checked
            },
            activeTabKey: 1,
            loadNominationTab: false,
            loadPATab: false
        };

        this.itemPerPage = 5;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Common/GetEmployees')),
            axios(resolveClientUrl('Common/GetMyGroup')),
            axios(resolveClientUrl('Common/GetCurrentPerformancePeriod')),
            axios(resolveClientUrl('member/GetMemberNominationResult')),
            axios(resolveClientUrl('member/Performance'))
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, employeeResponse, myGroupResponse, currentPerformancePeriodResponse,
                memberNominationResponse, memberPerformanceResponse]) => {
                let performanceGradeOptions = gradeResponse.data.map(performanceGrade => {
                    return {
                        value: performanceGrade.gradeId,
                        label: `(${performanceGrade.name}) ${performanceGrade.description}`
                    };
                });
                let projectOptions = projectResponse.data.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                let employeeOptions = employeeResponse.data.map(member => {
                    return { value: member.employeeId, label: `${member.fullName} (${member.nickName})` };
                });

                let myGroupOptions = myGroupResponse.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });

                let displayedMemberPerformances = memberPerformanceResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    periods: periodResponse.data,
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    employeeOptions: employeeOptions,
                    myGroupOptions: myGroupOptions,
                    currentPerformancePeriod: currentPerformancePeriodResponse.data,
                    memberNominations: memberNominationResponse.data,
                    memberPerformances: memberPerformanceResponse.data,
                    displayedMemberPerformances: displayedMemberPerformances,
                    hasMore: displayedMemberPerformances.length < memberPerformanceResponse.data.length
                });
            }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                pageType={2}
                data={performanceRecord}
                onEdit={this.handleEditMemberPerformanceRecord}
                isHighlighted={this.state.performanceRecordIdHighlighted === performanceRecord.performanceRecordId}
            />
        );
    }

    loadMoreMemberPerformances = (page, callback) => {
        let records = this.state.memberPerformances.slice(0, page * this.itemPerPage);
        this.setState({
            displayedMemberPerformances: records,
            hasMore: records.length < this.state.memberPerformances.length
        }, () => {
            if (callback) {
                callback();
            }
        });
    }

    handleTimelineNodeClick = (performanceRecordId) => {
        if (this.state.activeTabKey === 1) {
            if (!this.state.displayedMemberPerformances.some(r => r.performanceRecordId === performanceRecordId)) {
                let index = this.state.memberPerformances.findIndex(function (element) {
                    return element.performanceRecordId === performanceRecordId;
                });

                let page = Math.ceil((index + 1) / this.itemPerPage);

                this.refs.infiniteScrollRef.pageLoaded = page;

                this.loadMoreMemberPerformances(page, () => {
                    this.setState({ performanceRecordIdHighlighted: performanceRecordId }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
                });
            } else {
                this.setState({ performanceRecordIdHighlighted: performanceRecordId }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
            }
        }
    }

    handleCreateMemberPerformanceSuccess = () => {
        axios(resolveClientUrl('member/Performance'))
            .then((response) => {
                let memberPerformances = response.data;

                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    memberPerformances: memberPerformances,
                    displayedMemberPerformances: memberPerformances.slice(0, this.itemPerPage)
                });
            }).catch(axiosError);
    }

    handleSelectTab = (key) => {
        if (key === 2 && !this.state.loadNominationTab) {
            this.setState({ activeTabKey: key, loadNominationTab: true });
        } else if (key === 3 && !this.state.loadPATab) {
            this.setState({ activeTabKey: key, loadPATab: true });
        } else {
            this.setState({ activeTabKey: key });
        }
    }

    handleEditMemberPerformanceRecord = (performanceRecordId) => {
        this.setState({
            editMemberPerformance: {
                isShowModal: true,
                performanceRecordId: performanceRecordId
            }
        });
    }

    handleCancelEditMemberPerformance = () => {
        this.setState({
            editMemberPerformance: {
                isShowModal: false,
                performanceRecordId: 0
            }
        });
    }

    handleEditMemberPerformanceSuccess = () => {
        axios(resolveClientUrl('member/Performance'))
            .then((response) => {
                let memberPerformances = response.data;

                this.refs.memberTimelineRef.getMemberPerformanceTimeline();
                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    memberPerformances: memberPerformances,
                    editMemberPerformance: {
                        isShowModal: false,
                        performanceRecordId: 0
                    },
                    displayedMemberPerformances: memberPerformances.slice(0, this.itemPerPage)
                });
            }).catch(axiosError);
    }

    render() {
        let employeeOptions = this.state.employeeOptions;
        let performanceGradeOptions = this.state.performanceGradeOptions;
        let projectOptions = this.state.projectOptions;
        let currentUser = this.state.currentUser;
        const periodOptions = this.state.periods.map(period => {
            return {
                value: period.periodId,
                label: period.name
            }
        })
        return (
            <ToastLayout>
                <div className="row">
                    <div className="main-layout member-page">
                        <div className="general-info box">
                            <Scrollbars style={{ maxHeight: '100%' }} autoHide>
                                <Timeline ref="memberTimelineRef"
                                    currentPerformancePeriod={this.state.currentPerformancePeriod}
                                    groupOptions={this.state.myGroupOptions}
                                    onNodeClick={this.handleTimelineNodeClick}
                                />
                            </Scrollbars >
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                                <div className="warning marquee">
                                    <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                                </div>
                            </div>
                            <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                                <Tab eventKey={1} title={getResource('MemberPerformance')}>

                                    <div className="record-list">
                                        <Scrollbars style={{ height: '100%' }} autoHide>

                                            <CreateMemberPerformance
                                                currentUser={currentUser}
                                                memberOptions={employeeOptions}
                                                performanceGradeOptions={performanceGradeOptions}
                                                projectOptions={projectOptions}
                                                onSubmitSuccess={this.handleCreateMemberPerformanceSuccess}
                                                isMultiple
                                            />
                                            <InfiniteScroll
                                                pageStart={1}
                                                initialLoad={false}
                                                loadMore={this.loadMoreMemberPerformances}
                                                hasMore={this.state.hasMore}
                                                threshold={0}
                                                useWindow={false}
                                                ref="infiniteScrollRef"
                                            >
                                                {
                                                    this.state.displayedMemberPerformances.map(this.renderPerformanceRecord)
                                                }
                                            </InfiniteScroll>
                                        </Scrollbars>
                                    </div>
                                </Tab>
                                {
                                    this.state.currentUser.isNominator &&
                                    <Tab eventKey={2} title={getResource('Nomination')}>
                                        {
                                            this.state.loadNominationTab &&
                                            <Nomination
                                                currentUser={currentUser}
                                                employeeOptions={employeeOptions}
                                                isEmployeeSelectable
                                            />}
                                    </Tab>
                                }
                                {
                                    this.state.currentUser.isReviewer &&
                                    <Tab eventKey={3} title="Performance Appraisal">
                                        {
                                            this.state.loadPATab && this.state.periods.length > 0 &&
                                            < MemberPerformanceAppraisal
                                                periodOptions={periodOptions}
                                                gradeOptions={this.state.performanceGradeOptions}
                                                groupOptions={this.state.myGroupOptions}
                                            />
                                        }
                                    </Tab>
                                }
                            </Tabs>
                        </div>
                        <div className="work-info">
                            <Scrollbars style={{ maxHeight: '100%' }} autoHide>
                                <NominationResult isSupervisor nominations={this.state.memberNominations} />
                                {this.state.periods.length > 0 && <MemberWorkload periods={this.state.periods} />}
                            </Scrollbars >
                        </div>
                    </div>
                </div>
                {
                    this.state.editMemberPerformance.isShowModal &&
                    <EditMemberPerformance
                        currentUser={currentUser}
                        performanceRecordId={this.state.editMemberPerformance.performanceRecordId}
                        memberOptions={employeeOptions}
                        performanceGradeOptions={performanceGradeOptions}
                        projectOptions={projectOptions}
                        isShowModal={this.state.editMemberPerformance.isShowModal}
                        onCloseModal={this.handleCancelEditMemberPerformance}
                        onSubmitSuccess={this.handleEditMemberPerformanceSuccess}
                    />
                }
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));