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
import CreateSelfPerformance from '../home/CreateSelfPerformance';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberId: document.getElementById('MemberId').value,
            memberInfo: {},
            performanceAppraisals: [],
            displayedPerformanceRecords: [],
            performanceRecords: [],
            performanceRecordIdSelected: null,
            nominations: [],
            periods: [],
            performanceGradeOptions: [],
            projectOptions: [],
            hrmDetails: {},
            hasMore: false
        };
        this.timelineItemPerPage = window.innerWidth > 1600 ? 8 : 4;
        this.itemPerPage = 4;
    }

    componentDidMount() {
        const memberId = this.state.memberId;
        const promises = [
            axios(resolveClientUrl('common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('member/GetMemberNominationResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberInfo'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberPerformanceAppraisalResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/Performance'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('discipline/GetHRMSummary'), { params: { memberId: memberId } })
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, nominationResponse, memberInfoResponse, performanceAppraisalResponse, performanceRecordResponse, hrmResponse]) => {
                let displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage);
                let memberInfo = {
                    nickName: memberInfoResponse.data.nickName,
                    fullName: memberInfoResponse.data.fullName,
                    jobTitle: memberInfoResponse.data.jobTitle,
                    avatar: memberInfoResponse.data.base64Image
                };

                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });

                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });
                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    periods: periodResponse.data,
                    nominations: nominationResponse.data,
                    memberInfo: memberInfo,
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

    render() {
        const nominations = this.state.nominations;
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.displayedPerformanceRecords;
        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <EmployeeInfo employee={this.state.memberInfo} />
                        <PerformanceAppraisalResult performanceAppraisals={performanceAppraisals} />
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <div className="warning marquee">
                                <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                            </div>
                            <PersonalTimeline
                                itemPerPage={this.timelineItemPerPage}
                                onItemClick={this.handlePCIClick}
                                selectedPCIId={this.state.selectedPCIId}
                                ref="timelineRef"
                            />
                        </div>
                        <div className="record-list">
                            <CreateSelfPerformance
                                employee={this.state.memberInfo}
                                performanceGradeOptions={this.state.performanceGradeOptions}
                                projectOptions={this.state.projectOptions}
                                onSubmitSuccess={this.handleCreatePerformanceSuccess}
                                onExpandForm={this.handleExpandForm}
                            />
                            {performanceRecords.length > 0 &&
                                <InfiniteScroll
                                    pageStart={1} initialLoad={true}
                                    loadMore={this.loadPCIs}
                                    hasMore={this.state.hasMore}
                                    threshold={0}
                                    useWindow={true} ref="infiniteScrollRef"
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
                            <WorkLoad periods={this.state.periods} memberId={this.state.memberId} />
                        }
                    </div>

                </div>
            </div>

        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));