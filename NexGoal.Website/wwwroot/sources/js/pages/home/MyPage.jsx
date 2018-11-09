import React, { Component } from 'react';
import axios from 'axios';
import moment from 'moment';
import ReactTooltip from 'react-tooltip';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import NominationResult from '../../shared/components/NominationResult';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';
import PersonalTimeline from './timeline/PersonalTimeline';
import EmployeeInfo from './EmployeeInfo';

export default class MyPage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: [],
            displayedPerformanceRecords: [],
            nominations: [],
            performanceGradeOptions: [],
            projectOptions: [],
            performanceAppraisals: [],
            editPerformance: {
                isShowModal: false,
                performanceRecordId: 0
            },
            periods: [],
            personalTimeline: {
                records: [],
                pageIndex: 0,
                nextDisabled: false,
                prevDisabled: false
            },
            hrmDetail: {},
            currentUser: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src')
            }
        };

        this.timelineItemPerPage = window.innerWidth > 1600 ? 8 : 4;
        this.itemPerPage = 5;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Self/Performance')),
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('common/GetHRMSummary')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('self/Performance/Timeline'), {
                params: {
                    pageIndex: this.state.personalTimeline.pageIndex,
                    pageSize: this.timelineItemPerPage
                }
            })
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, performanceRecordResponse, periodResponse, hrmDetailResponse, nominationResponse, performanceAppraisalsResponse, personalTimelineResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                let displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    performanceRecords: performanceRecordResponse.data,
                    periods: periodResponse.data,
                    personalTimeline: {
                        records: personalTimelineResponse.data.performanceRecords,
                        pageIndex: 0,
                        nextDisabled: true,
                        prevDisabled: this.timelineItemPerPage >= personalTimelineResponse.data.totalItems
                    },
                    hrmDetail: hrmDetailResponse.data,
                    nominations: nominationResponse.data,
                    performanceAppraisals: performanceAppraisalsResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecordResponse.data.length
                });
            }).catch(axiosError);
    }

    componentDidUpdate() {
        ReactTooltip.rebuild();
    }

    onTimelinePageChange = (pageIndex, pageSize) => {
        axios.get(resolveClientUrl('Self/Performance/Timeline'), {
            params: {
                pageIndex: pageIndex,
                pageSize: pageSize
            }
        }).then((response) => {
            this.setState({
                personalTimeline: {
                    records: response.data.performanceRecords,
                    pageIndex: pageIndex,
                    nextDisabled: pageIndex === 0,
                    prevDisabled: (pageIndex + 1) * pageSize >= response.data.totalItems
                }
            });
        }).catch(axiosError);
    };

    handleCreatePerformanceSuccess = () => {
        axios(resolveClientUrl('Self/Performance'))
            .then((response) => {
                let performanceRecords = response.data;

                if (this.state.personalTimeline.pageIndex === 0) {
                    this.onTimelinePageChange(0);
                }

                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: performanceRecords.slice(0, this.itemPerPage)
                });
            }).catch(axiosError);
    }

    handleEditPerformanceRecord = (performanceRecordId) => {
        this.setState({
            editPerformance: {
                isShowModal: true,
                performanceRecordId: performanceRecordId
            }
        });
    }

    handleCancelEditPerformance = () => {
        this.setState({
            editPerformance: {
                isShowModal: false,
                performanceRecordId: 0
            }
        });
    }

    handleEditPerformanceSuccess = () => {
        axios(resolveClientUrl(`Self/Performance`))
            .then((response) => {
                let performanceRecords = response.data;

                this.onTimelinePageChange(0);
                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    performanceRecords: performanceRecords,
                    editPerformance: {
                        isShowModal: false,
                        performanceRecordId: 0
                    },
                    displayedPerformanceRecords: performanceRecords.slice(0, this.itemPerPage)
                });
            }).catch(axiosError);
    }

    loadMorePerformances = (page, callback) => {
        let records = this.state.performanceRecords.slice(0, page * this.itemPerPage);
        this.setState({
            displayedPerformanceRecords: records,
            hasMore: records.length < this.state.performanceRecords.length
        }, () => {
            if (callback) {
                callback();
            }
        });
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
                isHighlighted={this.state.performanceRecordIdHighlighted === performanceRecord.performanceRecordId}
            />
        );
    }

    handleTimelineNodeClick = performanceRecordId => {
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            let index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            let page = Math.ceil((index + 1) / this.itemPerPage);

            this.refs.infiniteScrollRef.pageLoaded = page;

            this.loadMorePerformances(page, () => {
                this.setState({ performanceRecordIdHighlighted: performanceRecordId }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
            });
        } else {
            this.setState({ performanceRecordIdHighlighted: performanceRecordId }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
        }
    }

    render() {
        let performanceRecords = this.state.displayedPerformanceRecords;
        let currentUser = this.state.currentUser;

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <EmployeeInfo employee={currentUser} />
                        <PerformanceAppraisalResult performanceAppraisals={this.state.performanceAppraisals} />
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <div className="warning marquee">
                                <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                            </div>
                            <PersonalTimeline
                                data={this.state.personalTimeline}
                                onNodeClick={this.handleTimelineNodeClick}
                                onPageChange={this.onTimelinePageChange}
                            />
                        </div>
                        <div className="record-list">
                            <Scrollbars style={{ height: '100%' }} autoHide={true}>
                                <CreateSelfPerformance
                                    currentUser={currentUser}
                                    performanceGradeOptions={this.state.performanceGradeOptions}
                                    projectOptions={this.state.projectOptions}
                                    onSubmitSuccess={this.handleCreatePerformanceSuccess}
                                />
                                {performanceRecords.length > 0 &&
                                    <InfiniteScroll initialLoad={false}
                                        pageStart={1}
                                        loadMore={this.loadMorePerformances}
                                        hasMore={this.state.hasMore}
                                        threshold={0}
                                        ref="infiniteScrollRef"
                                        useWindow={false}
                                    >
                                        <div className="pci-list">
                                            {
                                                performanceRecords.map(this.renderPerformanceRecord)
                                            }
                                        </div>

                                    </InfiniteScroll>
                                }
                            </Scrollbars>
                        </div>

                    </div>
                    <div className="work-info">
                        <NominationResult nominations={this.state.nominations} />
                        <HrmSummary hrmDetail={this.state.hrmDetail} />
                        {
                            this.state.periods.length > 0 &&
                            <WorkLoad periods={this.state.periods} />
                        }
                    </div>
                    {
                        this.state.editPerformance.isShowModal &&
                        <EditSelfPerformance
                            currentUser={currentUser}
                            performanceRecordId={this.state.editPerformance.performanceRecordId}
                            performanceGradeOptions={this.state.performanceGradeOptions}
                            projectOptions={this.state.projectOptions}
                            showModal={this.state.editPerformance.isShowModal}
                            onCloseModal={this.handleCancelEditPerformance}
                            onSubmitSuccess={this.handleEditPerformanceSuccess}
                        />
                    }
                    <ReactTooltip id="tooltip" place="bottom" type="info" html={true} className="tooltip" />
                </div>
            </div >
        );
    }
}