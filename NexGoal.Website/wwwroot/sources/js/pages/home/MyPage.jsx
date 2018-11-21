import React, { Component } from 'react';
import axios from 'axios';
import ReactTooltip from 'react-tooltip';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, calculateTimelineNode } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
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
            editPerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            deletePerformanceRecord: {
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

        this.timelineItemPerPage = calculateTimelineNode();
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
                        label: `(${performanceGrade.name}) ${performanceGrade.description}`
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
        let size = pageSize ? pageSize : calculateTimelineNode();
        axios.get(resolveClientUrl('Self/Performance/Timeline'), {
            params: {
                pageIndex: pageIndex,
                pageSize: size
            }
        }).then((response) => {
            this.setState({
                personalTimeline: {
                    records: response.data.performanceRecords,
                    pageIndex: pageIndex,
                    nextDisabled: pageIndex === 0,
                    prevDisabled: (pageIndex + 1) * size >= response.data.totalItems
                }
            });
        }).catch(axiosError);
    };

    handleCreatePerformanceRecordSuccess = () => {
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

    handleClickEditPerformanceRecord = (performanceRecordId) => {
        this.setState({
            editPerformanceRecord: {
                isShowModal: true,
                performanceRecordId: performanceRecordId
            }
        });
    }

    handleCancelEditPerformanceRecord = () => {
        this.setState({
            editPerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            }
        });
    }

    handleEditPerformanceRecordSuccess = () => {
        axios(resolveClientUrl(`Self/Performance`))
            .then((response) => {
                let performanceRecords = response.data;

                this.onTimelinePageChange(0);
                this.refs.infiniteScrollRef.pageLoaded = 1;

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: performanceRecords.slice(0, this.itemPerPage)
                });

                this.handleCancelEditPerformanceRecord();
            }).catch(axiosError);
    }

    handleClickDeletePerformanceRecord = (performanceRecordId) => {
        this.setState({
            deletePerformanceRecord: {
                isShowModal: true,
                performanceRecordId: performanceRecordId
            }
        });
    }

    handleDeletePerformanceRecord = () => {
        let performanceRecordId = this.state.deletePerformanceRecord.performanceRecordId;
        axios({
            url: resolveClientUrl('Self/Performance/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: performanceRecordId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                let performanceRecords = this.state.performanceRecords.slice();
                let index = performanceRecords.findIndex(item => item.performanceRecordId === performanceRecordId);
                if (index >= 0) {
                    performanceRecords.splice(index, 1);
                }

                let displayedPerformanceRecords = this.state.displayedPerformanceRecords.slice();
                index = displayedPerformanceRecords.findIndex(item => item.performanceRecordId === performanceRecordId);
                if (index >= 0) {
                    displayedPerformanceRecords.splice(index, 1);
                }

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords
                });

                this.handleCancelDeletePerformanceRecord();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleCancelDeletePerformanceRecord = () => {
        this.setState({
            deletePerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            }
        });
    }

    loadMorePerformanceRecord = (page, callback) => {
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
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                data={performanceRecord}
                onEdit={this.handleClickEditPerformanceRecord}
                onDelete={this.handleClickDeletePerformanceRecord}
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

            this.loadMorePerformanceRecord(page, () => {
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
                                    onSubmitSuccess={this.handleCreatePerformanceRecordSuccess}
                                />
                                {performanceRecords.length > 0 &&
                                    <InfiniteScroll initialLoad={false}
                                        pageStart={1}
                                        loadMore={this.loadMorePerformanceRecord}
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
                        {
                            this.state.nominations.length > 0 &&
                            <NominationResult nominations={this.state.nominations} />
                        }
                        <HrmSummary hrmDetail={this.state.hrmDetail} />
                        {
                            this.state.periods.length > 0 &&
                            <WorkLoad periods={this.state.periods} />
                        }
                    </div>
                    <ReactTooltip id="tooltip" place="bottom" type="info" html={true} className="tooltip" />
                </div>
                {
                    this.state.editPerformanceRecord.isShowModal &&
                    <EditSelfPerformance
                        currentUser={currentUser}
                        performanceRecordId={this.state.editPerformanceRecord.performanceRecordId}
                        performanceGradeOptions={this.state.performanceGradeOptions}
                        projectOptions={this.state.projectOptions}
                        showModal={this.state.editPerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelEditPerformanceRecord}
                        onSubmitSuccess={this.handleEditPerformanceRecordSuccess}
                    />
                }
                {
                    this.state.deletePerformanceRecord.isShowModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this performance"
                        showModal={this.state.deletePerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelDeletePerformanceRecord}
                        onSubmit={this.handleDeletePerformanceRecord}
                    />
                }
            </div >
        );
    }
}