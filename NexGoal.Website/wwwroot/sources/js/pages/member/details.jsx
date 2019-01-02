import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { Tabs, Tab } from 'react-bootstrap';
import InfiniteScroll from 'react-infinite-scroller';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, calculateTimelineNode, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import HrmSummary from '../home/hrm/HRMSummary';
import EmployeeInfo from '../home/EmployeeInfo';
import PersonalTimeline from '../home/timeline/PersonalTimeline';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import NominationResult from '../home/NominationResult';
import WorkLoad from '../home/workload';
import CreateMemberPerformance from './CreateMemberPerformance';
import EditMemberPerformance from './EditMemberPerformance';
import Nomination from './nomination/Nomination';
import Announcement from '../../shared/components/Announcement';
import Marquee from '../../shared/components/Marquee';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberInfo: {
                id: parseInt(document.getElementById('MemberId').value),
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
            personalTimeline: {
                records: [],
                pageIndex: 0,
                nextDisabled: true,
                prevDisabled: false
            },
            currentUserInfo: {
                ...getCurrentUserInfo(),
                roleId: 0
            },
            editPerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            deletePerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            performanceGradeOptions: [],
            projectOptions: [],
            announcements: [],
            hrmDetails: {},
            hasMore: false,
            activeTabKey: 1,
            loadNominationTab: false
        };

        this.infiniteScrollRef = React.createRef();
        this.timelineItemPerPage = calculateTimelineNode();
        this.itemPerPage = 10;
    }

    componentDidMount() {
        const memberId = this.state.memberInfo.id;
        const promises = [
            axios(resolveClientUrl('common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('member/GetRoleAccordingToMember'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberNominationResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberPerformanceAppraisalResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/Performance'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('common/GetHRMSummary'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('Member/Performance/Timeline'), {
                params: {
                    employeeId: this.state.memberInfo.id,
                    pageIndex: 0,
                    pageSize: this.timelineItemPerPage
                }
            }),
            axios(resolveClientUrl('common/GetAnnouncements'))
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, roleResponse,
                nominationResponse, performanceAppraisalResponse, performanceRecordResponse, hrmResponse, timelineResponse, announcementResponse]) => {
                let displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage);

                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: `(${performanceGrade.name}) ${performanceGrade.description}`
                    };
                });

                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                let timelineInfo = timelineResponse.data[0];
                let currentUserInfo = Object.assign({}, this.state.currentUserInfo, { roleId: roleResponse.data });

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    periods: periodResponse.data,
                    nominations: nominationResponse.data,
                    personalTimeline: {
                        records: timelineInfo.performanceRecords,
                        pageIndex: 0,
                        nextDisabled: true,
                        prevDisabled: this.timelineItemPerPage >= timelineInfo.totalItems
                    },
                    currentUserInfo: currentUserInfo,
                    performanceAppraisals: performanceAppraisalResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    performanceRecords: performanceRecordResponse.data,
                    announcements: announcementResponse.data,
                    hrmDetails: hrmResponse.data,
                    hasMore: displayedPerformanceRecords.length < performanceRecordResponse.data.length
                });
            }).catch(axiosError);
    }

    handleSelectTab = (key) => {
        if (key === 2 && !this.state.loadNominationTab) {
            this.setState({ activeTabKey: key, loadNominationTab: true });
        } else {
            this.setState({ activeTabKey: key });
        }
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

    handleTimelinePageChange = (pageIndex, pageSize) => {
        let size = pageSize ? pageSize : calculateTimelineNode();
        axios(resolveClientUrl('Member/Performance/Timeline'), {
            params: {
                employeeId: this.state.memberInfo.id,
                pageIndex: pageIndex,
                pageSize: size
            }
        }).then((response) => {
            let timelineInfo = response.data[0];
            this.setState({
                personalTimeline: {
                    records: timelineInfo.performanceRecords,
                    pageIndex: pageIndex,
                    nextDisabled: pageIndex === 0,
                    prevDisabled: (pageIndex + 1) * size >= timelineInfo.totalItems
                }
            });
        }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                data={performanceRecord}
                pageType={3}
                onEdit={this.handleClickEditPerformanceRecord}
                onDelete={this.handleClickDeletePerformanceRecord}
                isHighlighted={this.state.selectedPCIId === performanceRecord.performanceRecordId}
            />
        );
    }

    handleClickTimelineNode = performanceRecordId => {
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            let index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            let page = Math.ceil((index + 1) / this.itemPerPage);

            this.refs.infiniteScrollRef.pageLoaded = page;
            this.loadMorePerformanceRecord(page, () => {
                this.setState({
                    selectedPCIId: performanceRecordId
                }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
            });
        } else {
            this.setState({
                selectedPCIId: performanceRecordId
            }, () => document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'center', behavior: 'smooth' }));
        }
    }

    handleCreatePerformanceRecordSuccess = (performanceRecordIds, statusId) => {
        axios(resolveClientUrl(`Member/Performance/GetByIds/${performanceRecordIds}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();

                if (statusId === PerformanceRecordStatus.Draft) {
                    performanceRecords.unshift(...response.data);
                } else {
                    if (this.state.personalTimeline.pageIndex === 0) {
                        this.handleTimelinePageChange(0);
                    }

                    if (performanceRecords.length === 0) {
                        performanceRecords.unshift(...response.data);
                    } else {
                        let lastIndex = performanceRecords.map(record => { return record.statusId; }).lastIndexOf(PerformanceRecordStatus.Draft);
                        lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                        performanceRecords.splice(lastIndex, 0, ...response.data);
                    }
                }

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

    handleEditPerformanceRecordSuccess = (performanceRecordIds, statusId) => {
        axios(resolveClientUrl(`Member/Performance/GetByIds/${performanceRecordIds}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();
                let currentRecordIndex = performanceRecords.findIndex(record => record.performanceRecordId === this.state.editPerformanceRecord.performanceRecordId);
                let currentRecord = performanceRecords[currentRecordIndex];

                if (currentRecord.statusId === PerformanceRecordStatus.Draft) {
                    if (statusId === PerformanceRecordStatus.Draft) {
                        performanceRecords.splice(currentRecordIndex, 1, ...response.data);
                    } else {
                        performanceRecords.splice(currentRecordIndex, 1);

                        let lastIndex = performanceRecords.map(record => { return record.statusId; }).lastIndexOf(PerformanceRecordStatus.Draft);
                        lastIndex = lastIndex >= 0 ? lastIndex + 1 : 0;

                        let records = response.data.filter(record => record.employees[0].employeeId === this.state.memberInfo.id);
                        performanceRecords.splice(lastIndex, 0, ...records);

                        this.handleTimelinePageChange(this.state.personalTimeline.pageIndex);
                    }
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
            url: resolveClientUrl('Member/Performance/Delete'),
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
        const nominations = this.state.nominations;
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.displayedPerformanceRecords;

        return (
            <ToastLayout>
                <div className="row">
                    <div className="my-page member-detail main-layout">
                        <div className="general-info">
                            <EmployeeInfo employee={this.state.memberInfo} />
                            <PerformanceAppraisalResult
                                performanceAppraisals={performanceAppraisals}
                                title={`${this.state.memberInfo.nickName}'s Performance Appraisal`}
                            />
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                                <Marquee messages={this.state.announcements} onClick={this.handleOpenAnnouncementModal} />
                            </div>
                            <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab}>
                                <Tab eventKey={1} title="Performance">
                                    <PersonalTimeline
                                        itemPerPage={this.timelineItemPerPage}
                                        data={this.state.personalTimeline}
                                        onPageChange={this.handleTimelinePageChange}
                                        onNodeClick={this.handleClickTimelineNode}
                                        selectedPCIId={this.state.selectedPCIId}
                                        ref="timelineRef"
                                    />
                                    <div className="record-list">
                                        <CustomScrollbars>
                                            <CreateMemberPerformance
                                                currentUser={this.state.currentUserInfo}
                                                performanceGradeOptions={this.state.performanceGradeOptions}
                                                projectOptions={this.state.projectOptions}
                                                selectedMemberId={this.state.memberInfo.id}
                                                onSubmitSuccess={this.handleCreatePerformanceRecordSuccess}
                                                isMultiple={false}
                                            />
                                            {
                                                performanceRecords.length > 0 &&
                                                <InfiniteScroll
                                                    pageStart={1}
                                                    initialLoad={false}
                                                    loadMore={this.loadMorePerformanceRecord}
                                                    hasMore={this.state.hasMore}
                                                    threshold={0}
                                                    useWindow={false}
                                                    ref="infiniteScrollRef"
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
                                </Tab>
                                <Tab eventKey={2} title="Nomination">
                                    {
                                        this.state.loadNominationTab &&
                                        <Nomination
                                            currentUser={this.state.currentUserInfo}
                                            isMemberDetail
                                            nomineeId={this.state.memberInfo.id}
                                        />
                                    }
                                </Tab>
                            </Tabs>
                        </div>
                        <div className="work-info">
                            {
                                nominations.length > 0 &&
                                <NominationResult nominations={nominations} />
                            }
                            {
                                this.state.periods.length > 0 &&
                                <WorkLoad periods={this.state.periods} memberId={this.state.memberInfo.id} />
                            }
                            <HrmSummary hrmDetail={this.state.hrmDetails} />
                        </div>

                    </div>
                </div>
                {
                    this.state.editPerformanceRecord.isShowModal &&
                    <EditMemberPerformance
                        currentUser={this.state.currentUserInfo}
                        performanceRecordId={this.state.editPerformanceRecord.performanceRecordId}
                        performanceGradeOptions={this.state.performanceGradeOptions}
                        projectOptions={this.state.projectOptions}
                        isShowModal={this.state.editPerformanceRecord.isShowModal}
                        onCloseModal={this.handleCancelEditPerformanceRecord}
                        onSubmitSuccess={this.handleEditPerformanceRecordSuccess}
                        isDisabledAddMember
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

                <Announcement showModal={this.state.showAnnouncementModal} onCloseModal={this.handleCloseAnnouncementModal} announcements={this.state.announcements} />
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));