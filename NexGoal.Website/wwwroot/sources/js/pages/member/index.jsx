import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import InfiniteScroll from 'react-infinite-scroller';
import { Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import NominationResult from './NominationResult';
import CreateMemberPerformance from './CreateMemberPerformance';
import EditMemberPerformance from './EditMemberPerformance';
import MemberWorkload from './MemberWorkload';
import Timeline from './timeline/index';
import Nomination from './nomination/Nomination';
import MemberPerformanceAppraisal from './performance-appraisal/MemberPerformanceAppraisal';
import Announcement from '../../shared/components/Announcement';
import Marquee from '../../shared/components/Marquee';

const queryString = require('query-string');

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: [],
            displayedPerformanceRecords: [],
            memberPerformanceAppraisals: [],
            announcements: [],
            currentPerformancePeriod: null,
            editPerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            deletePerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            activeTabKey: 1,
            loadNominationTab: false,
            loadPATab: false,
            showAnnouncementModal: false
        };

        this.infiniteScrollRef = React.createRef();
        this.memberTimelineRef = React.createRef();
        this.createPerformanceCheckinWrapperRef = React.createRef();

        this.currentUser = {
            ...getCurrentUserInfo(),
            roleId: 2,
            isReviewer: document.getElementById('isReviewer').checked
        };
        this.itemPerPage = 10;
        this.memberOptions = [];
        this.myGroupOptions = [];
        this.performanceGradeOptions = [];
        this.projectOptions = [];
        this.periods = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Common/GetMyMember')),
            axios(resolveClientUrl('Common/GetMyGroup')),
            axios(resolveClientUrl('Common/GetCurrentPerformancePeriod')),
            axios(resolveClientUrl('member/Performance')),
            axios(resolveClientUrl('common/GetAnnouncements'))
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, memberResponse, myGroupResponse, currentPerformancePeriodResponse, memberPerformanceResponse, announcementsResponse]) => {
                this.performanceGradeOptions = gradeResponse.data.map(performanceGrade => {
                    return {
                        value: performanceGrade.gradeId,
                        label: `(${performanceGrade.name}) ${performanceGrade.description}`
                    };
                });
                this.projectOptions = projectResponse.data.map(project => {
                    return { value: project.projectId, label: project.name };
                });
                this.memberOptions = memberResponse.data.map(member => {
                    return {
                        value: member.employeeId,
                        label: `${member.nickName} (${member.fullName})`,
                        isProbation: member.isProbation
                    };
                });
                this.myGroupOptions = myGroupResponse.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });
                this.periods = periodResponse.data;

                let displayedPerformanceRecords = memberPerformanceResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    currentPerformancePeriod: currentPerformancePeriodResponse.data,
                    performanceRecords: memberPerformanceResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < memberPerformanceResponse.data.length,
                    announcements: announcementsResponse.data
                }, () => this.checkUrlParams());
            }).catch(axiosError);
    }

    checkUrlParams = () => {
        let params = queryString.parse(location.search);

        if (params) {
            if (params.recordId && parseInt(params.recordId)) {
                this.handleClickTimelineNode(parseInt(params.recordId));
            } else if (params.action && params.action === 'newrecord') {
                this.createPerformanceCheckinWrapperRef.current.handleOpenForm();
            } else if (params.activeTab && parseInt(params.activeTab)) {
                this.handleSelectTab(parseInt(params.activeTab));
            }
        }
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

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                pageType={2}
                data={performanceRecord}
                onEdit={this.handleClickEditPerformanceRecord}
                onEditDraft={this.handleClickEditDraftPerformanceRecord}
                onDelete={this.handleClickDeletePerformanceRecord}
                isHighlighted={this.state.performanceRecordIdHighlighted === performanceRecord.performanceRecordId}
            />
        );
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

    handleClickTimelineNode = (performanceRecordId) => {
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            let index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            if (index >= 0) {
                let page = Math.ceil((index + 1) / this.itemPerPage);

                this.refs.infiniteScrollRef.pageLoaded = page;

                this.loadMorePerformanceRecord(page, () => {
                    this.setState({
                        performanceRecordIdHighlighted: performanceRecordId,
                        activeTabKey: 1
                    }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
                });
            }
        } else {
            this.setState({
                performanceRecordIdHighlighted: performanceRecordId,
                activeTabKey: 1
            }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
        }
    }

    handleCreatePerformanceRecordSuccess = (performanceRecordIds) => {
        axios(resolveClientUrl(`Performance/GetByIds/${performanceRecordIds}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();

                if (performanceRecords.length === 0) {
                    performanceRecords.unshift(...response.data);
                } else {
                    let lastIndex = performanceRecords.map(record => { return record.isDraft; }).lastIndexOf(true);
                    lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                    performanceRecords.splice(lastIndex, 0, ...response.data);
                }

                this.refs.memberTimelineRef.getMemberPerformanceTimeline(this.refs.memberTimelineRef.state.pageSize);

                let pageIndex = this.refs.infiniteScrollRef ? this.refs.infiniteScrollRef.pageLoaded : 1;
                let displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });
            }).catch(axiosError);
    }

    handleCreateDraftPerformanceRecordSuccess = (performanceRecordId) => {
        axios(resolveClientUrl(`Performance/GetDraftById/${performanceRecordId}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();
                performanceRecords.unshift(response.data);
                let pageIndex = this.refs.infiniteScrollRef ? this.refs.infiniteScrollRef.pageLoaded : 1;
                let displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });
            }).catch(axiosError);
    }

    handleClickEditPerformanceRecord = (performanceRecordId) => {
        this.setState({
            editPerformanceRecord: {
                isShowModal: true,
                performanceRecordId: performanceRecordId,
                isDraft: false
            }
        });
    }

    handleClickEditDraftPerformanceRecord = (performanceRecordId) => {
        this.setState({
            editPerformanceRecord: {
                isShowModal: true,
                performanceRecordId: performanceRecordId,
                isDraft: true
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

    handleEditPerformanceRecordSuccess = (performanceRecordIds) => {
        axios(resolveClientUrl(`Performance/GetByIds/${performanceRecordIds}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();
                let currentRecordIndex = performanceRecords.findIndex(record => record.performanceRecordId === this.state.editPerformanceRecord.performanceRecordId);
                let currentRecord = performanceRecords[currentRecordIndex];

                if (currentRecord.isDraft) {
                    performanceRecords.splice(currentRecordIndex, 1);

                    let lastIndex = performanceRecords.map(record => { return record.isDraft; }).lastIndexOf(true);
                    lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                    performanceRecords.splice(lastIndex, 0, ...response.data);

                    this.refs.memberTimelineRef.getMemberPerformanceTimeline(this.refs.memberTimelineRef.state.pageSize);
                } else {
                    performanceRecords.splice(currentRecordIndex, 1, ...response.data);
                }

                let pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                let displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });

                this.handleCancelEditPerformanceRecord();
            }).catch(axiosError);
    }

    handleEditDraftPerformanceRecordSuccess = (performanceRecordId) => {
        axios(resolveClientUrl(`Performance/GetDraftById/${performanceRecordId}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();
                let currentRecordIndex = performanceRecords.findIndex(record => record.performanceRecordId === this.state.editPerformanceRecord.performanceRecordId);

                performanceRecords.splice(currentRecordIndex, 1, response.data);

                let pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                let displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
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
            url: resolveClientUrl('Performance/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: performanceRecordId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
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

                let pageIndex = this.refs.infiniteScrollRef.pageLoaded;
                let displayedPerformanceRecords = performanceRecords.slice(0, pageIndex * this.itemPerPage);

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
        this.setState({
            deletePerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            }
        });
    }

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false })

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true })

    render() {
        let currentUser = this.currentUser;
        let periodOptions = this.periods.map(period => {
            return {
                value: period.periodId,
                label: period.name
            };
        });

        return (
            <ToastLayout>
                <div className="row">
                    <div className="main-layout member-page">
                        <div className="general-info box">
                            <CustomScrollbars>
                                <Timeline ref="memberTimelineRef"
                                    currentPerformancePeriod={this.state.currentPerformancePeriod}
                                    groupOptions={this.myGroupOptions}
                                    onNodeClick={this.handleClickTimelineNode}
                                />
                            </CustomScrollbars >
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                                <Marquee messages={this.state.announcements} onClick={this.handleOpenAnnouncementModal} />
                            </div>
                            <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                                <Tab eventKey={1} title={getResource('Performance')}>

                                    <div className="record-list">
                                        <CustomScrollbars style={{ height: '100%' }} autoHide>
                                            <CreateMemberPerformance ref={this.createPerformanceCheckinWrapperRef}
                                                currentUser={currentUser}
                                                memberOptions={this.memberOptions}
                                                performanceGradeOptions={this.performanceGradeOptions}
                                                projectOptions={this.projectOptions}
                                                onSubmitSuccess={this.handleCreatePerformanceRecordSuccess}
                                                onSubmitDraftSuccess={this.handleCreateDraftPerformanceRecordSuccess}
                                                isMultiple
                                            />
                                            <InfiniteScroll
                                                pageStart={1}
                                                initialLoad={false}
                                                loadMore={this.loadMorePerformanceRecord}
                                                hasMore={this.state.hasMore}
                                                threshold={0}
                                                useWindow={false}
                                                ref="infiniteScrollRef"
                                            >
                                                {
                                                    this.state.displayedPerformanceRecords.map(this.renderPerformanceRecord)
                                                }
                                            </InfiniteScroll>
                                        </CustomScrollbars>
                                    </div>
                                </Tab>
                                <Tab eventKey={2} title={getResource('Nomination')}>
                                    {
                                        this.state.loadNominationTab &&
                                        <Nomination
                                            pageType={2}
                                            currentUser={currentUser}
                                            isEmployeeSelectable
                                        />}
                                </Tab>
                                {
                                    currentUser.isReviewer &&
                                    <Tab eventKey={3} title={getResource('PerformanceAppraisal')}>
                                        {
                                            this.state.loadPATab && periodOptions.length > 0 &&
                                            <MemberPerformanceAppraisal
                                                periodOptions={periodOptions}
                                                gradeOptions={this.performanceGradeOptions}
                                                groupOptions={this.myGroupOptions}
                                            />
                                        }
                                    </Tab>
                                }
                            </Tabs>
                        </div>
                        <div className="work-info">
                            <CustomScrollbars style={{ maxHeight: '100%' }} autoHide>
                                <div className="work-info-content">
                                    <NominationResult />
                                    {
                                        this.periods.length > 0 &&
                                        <MemberWorkload groupOptions={this.myGroupOptions} periods={this.periods} />
                                    }
                                </div>
                            </CustomScrollbars >
                        </div>
                    </div>
                </div>
                {
                    this.state.editPerformanceRecord.isShowModal &&
                    <EditMemberPerformance
                        currentUser={currentUser}
                        performanceRecordId={this.state.editPerformanceRecord.performanceRecordId}
                        isDraft={this.state.editPerformanceRecord.isDraft}
                        memberOptions={this.memberOptions}
                        performanceGradeOptions={this.performanceGradeOptions}
                        projectOptions={this.projectOptions}
                        isShowModal={this.state.editPerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelEditPerformanceRecord}
                        onSubmitSuccess={this.handleEditPerformanceRecordSuccess}
                        onSubmitDraftSuccess={this.handleEditDraftPerformanceRecordSuccess}
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
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));