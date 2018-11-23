import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';
import { Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import NominationResult from './NominationResult';
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
            performanceRecords: [],
            displayedPerformanceRecords: [],
            memberPerformanceAppraisals: [],
            periods: [],
            currentPerformancePeriod: null,
            editPerformanceRecord: {
                isShowModal: false,
                performanceRecordId: 0
            },
            deletePerformanceRecord: {
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
            axios(resolveClientUrl('member/Performance'))
        ];

        Promise.all(promises)
            .then(([periodResponse, gradeResponse, projectResponse, employeeResponse, myGroupResponse, currentPerformancePeriodResponse, memberPerformanceResponse]) => {
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

                let displayedPerformanceRecords = memberPerformanceResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    periods: periodResponse.data,
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    employeeOptions: employeeOptions,
                    myGroupOptions: myGroupOptions,
                    currentPerformancePeriod: currentPerformancePeriodResponse.data,
                    performanceRecords: memberPerformanceResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < memberPerformanceResponse.data.length
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

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                pageType={2}
                data={performanceRecord}
                onEdit={this.handleClickEditPerformanceRecord}
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
        if (this.state.activeTabKey === 1) {
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
    }

    handleCreatePerformanceRecordSuccess = () => {
        axios(resolveClientUrl('member/Performance'))
            .then((response) => {
                let performanceRecords = response.data;

                let displayedPerformanceRecords = performanceRecords.slice(0, this.refs.infiniteScrollRef.pageLoaded * this.itemPerPage);

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

    handleEditPerformanceRecordSuccess = () => {
        axios(resolveClientUrl('member/Performance'))
            .then((response) => {
                let performanceRecords = response.data;

                this.refs.memberTimelineRef.getMemberPerformanceTimeline();

                let displayedPerformanceRecords = performanceRecords.slice(0, this.refs.infiniteScrollRef.pageLoaded * this.itemPerPage);

                this.setState({
                    performanceRecords: performanceRecords,
                    editPerformanceRecord: {
                        isShowModal: false,
                        performanceRecordId: 0
                    },
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecords.length
                });
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

    render() {
        let employeeOptions = this.state.employeeOptions;
        let performanceGradeOptions = this.state.performanceGradeOptions;
        let projectOptions = this.state.projectOptions;
        let currentUser = this.state.currentUser;
        const periodOptions = this.state.periods.map(period => {
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
                            <Scrollbars style={{ maxHeight: '100%' }} autoHide>
                                <Timeline ref="memberTimelineRef"
                                    currentPerformancePeriod={this.state.currentPerformancePeriod}
                                    groupOptions={this.state.myGroupOptions}
                                    onNodeClick={this.handleClickTimelineNode}
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
                                                onSubmitSuccess={this.handleCreatePerformanceRecordSuccess}
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
                                    <Tab eventKey={3} title={getResource('PerformanceAppraisal')}>
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
                                <NominationResult />
                                {this.state.periods.length > 0 && <MemberWorkload groupOptions={this.state.myGroupOptions} periods={this.state.periods} />}
                            </Scrollbars >
                        </div>
                    </div>
                </div>
                {
                    this.state.editPerformanceRecord.isShowModal &&
                    <EditMemberPerformance
                        currentUser={currentUser}
                        performanceRecordId={this.state.editPerformanceRecord.performanceRecordId}
                        memberOptions={employeeOptions}
                        performanceGradeOptions={performanceGradeOptions}
                        projectOptions={projectOptions}
                        isShowModal={this.state.editPerformanceRecord.isShowModal}
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
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));