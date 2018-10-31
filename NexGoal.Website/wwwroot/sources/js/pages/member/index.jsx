import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import NominationResult from '../../shared/components/NominationResult';
import CreateMemberPerformance from './CreateMemberPerformance';
import MemberWorkload from './MemberWorkload';
import Timeline from './timeline/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberOptions: [],
            performanceGradeOptions: [],
            projectOptions: [],
            memberPerformances: [],
            memberPerformanceAppraisals: [],
            memberNominations: [],
            periods: [],
            displayedMemberPerformances: [],
            currentUser: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src')
            }
        };

        this.itemPerPage = 4;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Common/GetMembers')),
            axios(resolveClientUrl('member/GetMemberNominationResult')),
            axios(resolveClientUrl('member/Performance'))
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, memberResponse, memberNominationResponse, memberPerformanceResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map(performanceGrade => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map(project => {
                    return { value: project.projectId, label: project.name };
                });
                const memberOptions = memberResponse.data.map(member => {
                    return { value: member.employeeId, label: `${member.fullName} (${member.nickName})` };
                });

                let displayedMemberPerformances = memberPerformanceResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    periods: periodResponse.data,
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    memberOptions: memberOptions,
                    memberNominations: memberNominationResponse.data,
                    memberPerformances: memberPerformanceResponse.data,
                    displayedMemberPerformances: displayedMemberPerformances,
                    hasMore: displayedMemberPerformances.length < memberPerformanceResponse.data.length
                });
            }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
        let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                objectId={performanceRecord.performanceRecordId}
                employees={performanceRecord.employees}
                creator={performanceRecord.creator}
                roleId={performanceRecord.roleId}
                statusId={performanceRecord.statusId}
                submittedDate={submittedDate} modifiedDate={modifiedDate}
                isHost={performanceRecord.isHost}
                grade={performanceRecord.grade}
                comment={performanceRecord.comment}
                projects={performanceRecord.projects.map(project => project.name)}
                onEdit={this.handleEditPerformanceRecord}
                isHighlighted={this.state.selectedPCIId === performanceRecord.performanceRecordId}
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
        if (!this.state.displayedMemberPerformances.some(r => r.performanceRecordId === performanceRecordId)) {
            let index = this.state.memberPerformances.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            let page = Math.ceil((index + 1) / this.itemPerPage);

            this.refs.infiniteScrollRef.pageLoaded = page;

            this.loadMoreMemberPerformances(page, () => {
                this.setState({ selectedPCIId: performanceRecordId });
                document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'end', behavior: 'smooth' });
            });
        } else {
            this.setState({ selectedPCIId: performanceRecordId });
            document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'end', behavior: 'smooth' });
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

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <div className="main-layout member-page">
                        <div className="general-info box">
                            <Scrollbars style={{ maxHeight: '100%' }} autoHide={true} >
                            <h5>Member Performance - 2nd 2018</h5>
                                <Timeline onNodeClick={this.handleTimelineNodeClick} />
                            </Scrollbars >
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                                <div className="warning marquee">
                                    <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                                </div>
                            </div>
                            <div className="record-list">
                                <CreateMemberPerformance
                                    currentUser={this.state.currentUser}
                                    memberOptions={this.state.memberOptions}
                                    performanceGradeOptions={this.state.performanceGradeOptions}
                                    projectOptions={this.state.projectOptions}
                                    onSubmitSuccess={this.handleCreateMemberPerformanceSuccess}
                                />
                                <InfiniteScroll
                                    pageStart={1}
                                    initialLoad
                                    loadMore={this.loadMoreMemberPerformances}
                                    hasMore={this.state.hasMore}
                                    threshold={0}
                                    useWindow
                                    ref="infiniteScrollRef"
                                >
                                    {
                                        this.state.displayedMemberPerformances.map(this.renderPerformanceRecord)
                                    }
                                </InfiniteScroll>

                            </div>
                        </div>
                        <div className="work-info">
                            <Scrollbars style={{ maxHeight: '100%' }} autoHide={true} >
                                <NominationResult isSupervisor={true} nominations={this.state.memberNominations} />
                                {this.state.periods.length > 0 && <MemberWorkload periods={this.state.periods} />}
                            </Scrollbars >
                        </div>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));