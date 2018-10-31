import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import InfiniteScroll from 'react-infinite-scroller';
import moment from 'moment';
import EmployeeInfo from '../home/EmployeeInfo';
import PersonalTimeline from '../home/timeline/PersonalTimeline';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import NominationResult from '../../shared/components/NominationResult';
import HrmSummary from '../home/HRMSummary';
import WorkLoad from '../home/workload';
import CreateMemberPerformance from './CreateMemberPerformance';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberInfo: {
                id: document.getElementById('MemberId').value,
                nickName: document.getElementById('member-nickName').value,
                fullName: document.getElementById('member-fullName').value,
                jobTitle: document.getElementById('member-jobTitle').value,
                avatar: document.getElementById('member-avatar').value
            },
            performanceAppraisals: [],
            displayedPerformanceRecords: [],
            performanceRecords: [],
            performanceRecordIdSelected: null,
            nominations: [],
            periods: [],
            memberOptions: [],
            personalTimeline: {
                records: [],
                pageIndex: 0,
                nextDisabled: true,
                prevDisabled: false
            },
            currentUser: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src')
            },
            performanceGradeOptions: [],
            projectOptions: [],
            hrmDetails: {},
            hasMore: false,
            isFormExpanded: false
        };
        this.timelineItemPerPage = window.innerWidth > 1600 ? 8 : 4;
        this.itemPerPage = 4;
    }

    componentDidMount() {
        const memberId = this.state.memberInfo.id;
        const promises = [
            axios(resolveClientUrl('common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Common/GetMembers')),
            axios(resolveClientUrl('member/GetMemberNominationResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberPerformanceAppraisalResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/Performance'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('discipline/GetHRMSummary'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    employeeId: this.state.memberInfo.id,
                    pageIndex: 0,
                    pageSize: this.timelineItemPerPage
                }
            })
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, memberResponse, nominationResponse, performanceAppraisalResponse, performanceRecordResponse, hrmResponse, timelineResponse]) => {
                let displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage);

                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });

                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                const memberOptions = memberResponse.data.map(member => {
                    return { value: member.employeeId, label: `${member.fullName} (${member.nickName})` };
                });

                let timelineInfo = timelineResponse.data[0];
                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    periods: periodResponse.data,
                    nominations: nominationResponse.data,
                    memberOptions: memberOptions,
                    personalTimeline: {
                        records: timelineInfo.performanceRecords,
                        pageIndex: 0,
                        nextDisabled: true,
                        prevDisabled: this.timelineItemPerPage >= timelineInfo.totalItems
                    },
                    performanceAppraisals: performanceAppraisalResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    performanceRecords: performanceRecordResponse.data,
                    hrmDetails: hrmResponse.data,
                    hasMore: displayedPerformanceRecords.length < performanceRecordResponse.data.length
                });
            }).catch(axiosError);
    }

    loadPCIs = (page, callback) => {
        let records = this.state.performanceRecords.slice(0, page * this.itemPerPage);
        this.setState({
            displayedPerformanceRecords: records,
            hasMore: records.length < this.state.performanceRecords.length
        }, () => {
            if (callback)
                callback();
        });
    }

    onTimelinePageChange = (pageIndex) => {
        axios(resolveClientUrl('Member/Performance/Timeline'), {
            params: {
                employeeId: this.state.memberInfo.id,
                pageIndex: pageIndex,
                pageSize: this.timelineItemPerPage
            }
        }).then((response) => {
            let timelineInfo = response.data[0];
            this.setState({
                personalTimeline: {
                    records: timelineInfo.performanceRecords,
                    pageIndex: pageIndex,
                    nextDisabled: pageIndex === 0,
                    prevDisabled: (pageIndex + 1) * this.timelineItemPerPage >= timelineInfo.totalItems
                }
            });
        }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        let isHost = performanceRecord.statusId === PerformanceRecordStatus.Draft
            || performanceRecord.statusId === PerformanceRecordStatus.Submitted;
        let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
        let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                objectId={performanceRecord.performanceRecordId}
                employee={performanceRecord.employee}
                creator={performanceRecord.creator}
                roleId={performanceRecord.roleId}
                statusId={performanceRecord.statusId}
                submittedDate={submittedDate} modifiedDate={modifiedDate}
                isHost={isHost}
                grade={performanceRecord.grade}
                comment={performanceRecord.comment}
                projects={performanceRecord.projects.map(project => project.name)}
                onEdit={this.handleEditPerformanceRecord}
                isHighlighted={this.state.selectedPCIId === performanceRecord.performanceRecordId}
            />
        );
    }

    handleExpandForm = isExpanded => this.setState({ isFormExpanded: isExpanded });

    handlePCIClick = performanceRecordId => {
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            let index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            let page = Math.ceil((index + 1) / this.itemPerPage);

            this.refs.infiniteScrollRef.pageLoaded = page;
            this.loadPCIs(page, () => {
                this.setState({ selectedPCIId: performanceRecordId });
                document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'end', behavior: 'smooth' });
            });
        } else {
            this.setState({ selectedPCIId: performanceRecordId });
            document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'end', behavior: 'smooth' });
        }
    }

    handleCreateMemberPerformanceSuccess = () => {
        axios(resolveClientUrl('member/Performance'), { params: { memberId: this.state.memberInfo.id } })
            .then((response) => {
                let memberPerformances = response.data;
                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    performanceRecords: memberPerformances,
                    displayedMemberPerformances: memberPerformances.slice(0, this.itemPerPage)
                });
            }).catch(axiosError);
    }

    render() {
        const nominations = this.state.nominations;
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.displayedPerformanceRecords;
        const selectedMembes = [{
            value: parseInt(this.state.memberInfo.id),
            label: `${this.state.memberInfo.fullName} (${this.state.memberInfo.nickName})`
        }];
        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <EmployeeInfo employee={this.state.memberInfo} />
                        <PerformanceAppraisalResult
                            performanceAppraisals={performanceAppraisals}
                            title={`${this.state.memberInfo.nickName}'s PAs`}
                        />
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <div className="warning marquee">
                                <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                            </div>
                            <PersonalTimeline
                                itemPerPage={this.timelineItemPerPage}
                                data={this.state.personalTimeline}
                                onPageChange={this.onTimelinePageChange}
                                onItemClick={this.handlePCIClick}
                                selectedPCIId={this.state.selectedPCIId}
                                ref="timelineRef"
                            />
                        </div>
                        <div className="record-list">
                            <CreateMemberPerformance
                                currentUser={this.state.currentUser}
                                memberOptions={this.state.memberOptions}
                                performanceGradeOptions={this.state.performanceGradeOptions}
                                projectOptions={this.state.projectOptions}
                                selectedMembers={selectedMembes}
                                onSubmitSuccess={this.handleCreateMemberPerformanceSuccess}
                            />
                            {performanceRecords.length > 0 &&
                                <InfiniteScroll
                                    pageStart={1}
                                    initialLoad
                                    loadMore={this.loadPCIs}
                                    hasMore={this.state.hasMore}
                                    threshold={0}
                                    useWindow
                                    ref="infiniteScrollRef"
                                >
                                    {
                                        performanceRecords.map(this.renderPerformanceRecord)
                                    }
                                </InfiniteScroll>
                            }
                        </div>
                    </div>
                    <div className="work-info">
                        <NominationResult nominations={nominations} />
                        <HrmSummary hrmDetail={this.state.hrmDetails} />
                        {
                            this.state.periods.length > 0 &&
                            <WorkLoad periods={this.state.periods} memberId={this.state.memberInfo.id} />
                        }
                    </div>

                </div>
            </div>

        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));