import React, { Component } from 'react';
import axios from 'axios';
import ReactTooltip from 'react-tooltip';
import InfiniteScroll from 'react-infinite-scroller';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, calculateTimelineNode, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
import { PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import Announcement from '../../shared/components/Announcement';
import Marquee from '../../shared/components/Marquee';
import HrmSummary from './hrm/HRMSummary';
import NominationResult from './NominationResult';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';
import PersonalTimeline from './timeline/PersonalTimeline';
import EmployeeInfo from './EmployeeInfo';
import {
    loadMorePerformanceRecord, handleClickEditPerformanceRecord, handleCancelEditPerformanceRecord,
    handleClickEditDraftPerformanceRecord, handleClickDeletePerformanceRecord, handleCancelDeletePerformanceRecord
} from './common-functions';

const queryString = require('query-string');

export default class MyPage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: [],
            displayedPerformanceRecords: [],
            nominations: [],
            performanceAppraisals: [],
            announcements: [],
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

            showAnnouncementModal: false
        };

        this.infiniteScrollRef = React.createRef();
        this.timelineItemPerPage = calculateTimelineNode();
        this.createPerformanceCheckinWrapperRef = React.createRef();
        this.itemPerPage = 10;
        this.performanceGradeOptions = [];
        this.projectOptions = [];
        this.performanceGradeInfos = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjectTags')),
            axios(resolveClientUrl('Self/Performance')),
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('self/Performance/Timeline'), {
                params: {
                    pageIndex: this.state.personalTimeline.pageIndex,
                    pageSize: this.timelineItemPerPage
                }
            }),
            axios(resolveClientUrl('common/GetAnnouncements'))
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, performanceRecordResponse, periodResponse,
                nominationResponse, performanceAppraisalsResponse, personalTimelineResponse, announcementsResponse]) => {
                this.performanceGradeInfos = gradeResponse.data;

                this.performanceGradeOptions = gradeResponse.data.map(performanceGrade => {
                    return {
                        value: performanceGrade.gradeId,
                        label: `(${performanceGrade.name}) ${performanceGrade.description}`
                    };
                });
                this.projectOptions = projectResponse.data.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                const displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecordResponse.data,
                    periods: periodResponse.data,
                    announcements: announcementsResponse.data.length > 0 ? announcementsResponse.data : [getResource('DefaultAnnouncement')],
                    personalTimeline: {
                        records: personalTimelineResponse.data.performanceRecords,
                        pageIndex: 0,
                        nextDisabled: true,
                        prevDisabled: this.timelineItemPerPage >= personalTimelineResponse.data.totalItems
                    },
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

    checkUrlParams = () => {
        const params = queryString.parse(location.search);

        if (params) {
            if (params.recordId && parseInt(params.recordId, 10)) {
                this.handleClickTimelineNode(parseInt(params.recordId, 10));
            } else if (params.action && params.action === 'newrecord') {
                this.createPerformanceCheckinWrapperRef.current.handleOpenForm();
            }
        }
    }

    handleTimelinePageChange = (pageIndex, pageSize) => {
        const size = pageSize ? pageSize : calculateTimelineNode();
        axios.get(resolveClientUrl('Self/Performance/Timeline'), {
            params: {
                pageIndex: pageIndex,
                pageSize: size
            }
        }).then(response => {
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

    handleCreatePerformanceRecordSuccess = performanceRecordIds => {
        axios(resolveClientUrl(`Performance/GetByIds/${performanceRecordIds}`))
            .then(response => {
                const performanceRecords = this.state.performanceRecords.slice();

                if (this.state.personalTimeline.pageIndex === 0) {
                    this.handleTimelinePageChange(0);
                }

                if (performanceRecords.length === 0) {
                    performanceRecords.unshift(...response.data);
                } else {
                    let lastIndex = performanceRecords.map(record => record.isDraft).lastIndexOf(true);
                    lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                    performanceRecords.splice(lastIndex, 0, ...response.data);
                }

                const pageIndex = this.refs.infiniteScrollRef ? this.refs.infiniteScrollRef.pageLoaded : 1;
                const displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });
            }).catch(axiosError);
    }

    handleCreateDraftPerformanceRecordSuccess = performanceRecordId => {
        axios(resolveClientUrl(`Performance/GetDraftById/${performanceRecordId}`))
            .then(response => {
                const performanceRecords = this.state.performanceRecords.slice();

                performanceRecords.unshift(response.data);

                const pageIndex = this.refs.infiniteScrollRef ? this.refs.infiniteScrollRef.pageLoaded : 1;
                const displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });
            }).catch(axiosError);
    }

    handleClickEditPerformanceRecord = performanceRecordId => {
        handleClickEditPerformanceRecord(this, performanceRecordId);
    }

    handleClickEditDraftPerformanceRecord = performanceRecordId => {
        handleClickEditDraftPerformanceRecord(this, performanceRecordId);
    }

    handleCancelEditPerformanceRecord = () => {
        handleCancelEditPerformanceRecord(this);
    }

    handleEditPerformanceRecordSuccess = performanceRecordIds => {
        axios(resolveClientUrl(`Performance/GetByIds/${performanceRecordIds}`))
            .then(response => {
                const performanceRecords = this.state.performanceRecords.slice();
                const currentRecordIndex = performanceRecords.findIndex(record => record.performanceRecordId === this.state.editPerformanceRecord.performanceRecordId);
                const currentRecord = performanceRecords[currentRecordIndex];

                if (currentRecord.isDraft) {
                    performanceRecords.splice(currentRecordIndex, 1);

                    let lastIndex = performanceRecords.map(record => record.isDraft).lastIndexOf(true);
                    lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                    performanceRecords.splice(lastIndex, 0, ...response.data);

                    this.handleTimelinePageChange(this.state.personalTimeline.pageIndex);
                } else {
                    performanceRecords.splice(currentRecordIndex, 1, ...response.data);
                }

                const pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                const displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });

                this.handleCancelEditPerformanceRecord();
            }).catch(axiosError);
    }

    handleEditDraftPerformanceRecordSuccess = performanceRecordId => {
        axios(resolveClientUrl(`Performance/GetDraftById/${performanceRecordId}`))
            .then(response => {
                const performanceRecords = this.state.performanceRecords.slice();
                const currentRecordIndex = performanceRecords.findIndex(record => record.performanceRecordId === this.state.editPerformanceRecord.performanceRecordId);

                performanceRecords.splice(currentRecordIndex, 1, response.data);

                const pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                const displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });

                this.handleCancelEditPerformanceRecord();
            }).catch(axiosError);
    }

    handleClickDeletePerformanceRecord = performanceRecordId => {
        handleClickDeletePerformanceRecord(this, performanceRecordId);
    }

    handleDeletePerformanceRecord = () => {
        const performanceRecordId = this.state.deletePerformanceRecord.performanceRecordId;
        axios({
            url: resolveClientUrl('Performance/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: performanceRecordId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                const performanceRecords = this.state.performanceRecords.slice();
                const index = performanceRecords.findIndex(item => item.performanceRecordId === performanceRecordId);
                if (index >= 0) {
                    performanceRecords.splice(index, 1);
                }

                const pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                const displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });

                this.handleCancelDeletePerformanceRecord();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleCancelDeletePerformanceRecord = () => {
        handleCancelDeletePerformanceRecord(vm);
    }

    loadMorePerformanceRecord = (page, callback) => {
        loadMorePerformanceRecord(this, page, callback);
    }

    renderPerformanceRecord = performanceRecord => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                data={performanceRecord}
                onEdit={this.handleClickEditPerformanceRecord}
                onEditDraft={this.handleClickEditDraftPerformanceRecord}
                onDelete={this.handleClickDeletePerformanceRecord}
                isHighlighted={this.state.performanceRecordIdHighlighted === performanceRecord.performanceRecordId}
            />
        );
    }

    handleClickTimelineNode = performanceRecordId => {
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            const index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            if (index >= 0) {
                const page = Math.ceil((index + 1) / this.itemPerPage);

                this.refs.infiniteScrollRef.pageLoaded = page;

                this.loadMorePerformanceRecord(page, () => {
                    this.setState({ performanceRecordIdHighlighted: performanceRecordId },
                        () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
                });
            }
        } else {
            this.setState({ performanceRecordIdHighlighted: performanceRecordId },
                () => {
                    document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' });
                });
        }
    }

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false })

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true })

    render() {
        const performanceRecords = this.state.displayedPerformanceRecords;
        const currentUserInfo = getCurrentUserInfo();

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <CustomScrollbars>
                            <EmployeeInfo employee={currentUserInfo} />
                            <PerformanceAppraisalResult performanceAppraisals={this.state.performanceAppraisals} />
                        </CustomScrollbars>
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <Marquee messages={this.state.announcements} onClick={this.handleOpenAnnouncementModal} />
                            <PersonalTimeline
                                data={this.state.personalTimeline}
                                onNodeClick={this.handleClickTimelineNode}
                                onPageChange={this.handleTimelinePageChange}
                            />
                        </div>
                        <div className="record-list">
                            <CustomScrollbars>
                                <CreateSelfPerformance ref={this.createPerformanceCheckinWrapperRef}
                                    currentUser={currentUserInfo}
                                    performanceGradeOptions={this.performanceGradeOptions}
                                    projectOptions={this.projectOptions}
                                    onSubmitSuccess={this.handleCreatePerformanceRecordSuccess}
                                    onSubmitDraftSuccess={this.handleCreateDraftPerformanceRecordSuccess}
                                    performanceGradeInfos={this.performanceGradeInfos}
                                />
                                {
                                    performanceRecords.length > 0 &&
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
                            </CustomScrollbars>
                        </div>

                    </div>
                    <div className="work-info">
                        <CustomScrollbars>
                            <div className="work-info-content">
                                {
                                    this.state.nominations.length > 0 &&
                                    <NominationResult nominations={this.state.nominations} />
                                }
                                {
                                    this.state.periods.length > 0 &&
                                    <WorkLoad periods={this.state.periods} afterInitChart={this.checkUrlParams} />
                                }
                                <HrmSummary />
                            </div>
                        </CustomScrollbars >
                    </div>
                    <ReactTooltip id="tooltip" place="bottom" type="info" html className="tooltip" />
                </div>
                {
                    this.state.editPerformanceRecord.isShowModal &&
                    <EditSelfPerformance
                        currentUser={currentUserInfo}
                        performanceRecordId={this.state.editPerformanceRecord.performanceRecordId}
                        isDraft={this.state.editPerformanceRecord.isDraft}
                        performanceGradeOptions={this.performanceGradeOptions}
                        projectOptions={this.projectOptions}
                        showModal={this.state.editPerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelEditPerformanceRecord}
                        onSubmitSuccess={this.handleEditPerformanceRecordSuccess}
                        onSubmitDraftSuccess={this.handleEditDraftPerformanceRecordSuccess}
                        performanceGradeInfos={this.performanceGradeInfos}

                    />
                }
                {
                    this.state.deletePerformanceRecord.isShowModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this record"
                        showModal={this.state.deletePerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelDeletePerformanceRecord}
                        onSubmit={this.handleDeletePerformanceRecord}
                    />
                }
                <Announcement showModal={this.state.showAnnouncementModal}
                    onCloseModal={this.handleCloseAnnouncementModal}
                    announcements={this.state.announcements}
                />

            </div >
        );
    }
}