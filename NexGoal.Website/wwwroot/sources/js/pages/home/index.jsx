import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import ReactTooltip from 'react-tooltip';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';
import NominationResult from './NominationResult';
import PersonalTimeline from './PersonalTimeline';
import EmployeeInfo from './EmployeeInfo';

var sanitizeHtmlReact = require('sanitize-html-react');
sanitizeHtmlReact.defaults.allowedTags = false;
sanitizeHtmlReact.defaults.allowedAttributes = false;

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: [],
            displayedPerformanceRecords: [],
            nominations: [],
            workload: [],
            performanceGradeOptions: [],
            projectOptions: [],
            showEditPerformanceModal: false,
            performanceRecordIdSelected: null,
            periods: [],
            employee: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src')
            },
            isFormExpanded: false,
        };

        this.infiniteScrollRef = React.createRef();
        this.itemPerPage = 4;//window.innerWidth > 1600 ? 8 : 4;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('Self/Performance/Index')),
            axios(resolveClientUrl('Common/GetAllPeriods'))
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, performanceRecordResponse, periodResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                let displayedPerformanceRecords = performanceRecordResponse.data.slice(0, this.itemPerPage)

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    performanceRecords: performanceRecordResponse.data,
                    periods: periodResponse.data,
                    displayedPerformanceRecords: displayedPerformanceRecords,
                    hasMore: displayedPerformanceRecords.length < performanceRecordResponse.data.length
                });
            }).catch(axiosError);
    }

    componentDidUpdate() {
        ReactTooltip.rebuild();
    }

    handleCreatePerformanceSuccess = (response) => {
        axios(resolveClientUrl(`Self/Performance/GetById/${response.objectId}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();
                performanceRecords.unshift(response.data);

                this.setState({
                    performanceRecords: performanceRecords
                });
            }).catch(axiosError);
    }

    handleEditPerformanceRecord = (performanceRecordId) => {
        this.setState({
            showEditPerformanceModal: true,
            performanceRecordIdSelected: performanceRecordId
        });
    }

    handleCloseEditPerformanceModal = () => {
        this.setState({
            showEditPerformanceModal: false,
            performanceRecordIdSelected: null
        });
    }

    handleEditPerformanceSuccess = (performanceRecordId) => {
        axios(resolveClientUrl(`Self/Performance/GetById/${performanceRecordId}`))
            .then((response) => {
                let performanceRecords = this.state.performanceRecords.slice();

                let index = performanceRecords.map(record => { return record.performanceRecordId; }).indexOf(performanceRecordId);
                if (index >= 0) {
                    performanceRecords[index] = response.data;
                }

                this.setState({
                    performanceRecords: performanceRecords,
                    showEditPerformanceModal: false,
                    performanceRecordIdSelected: null
                });
            }).catch(axiosError);
    }

    loadPCIs = (page, callback) => {
        debugger;
        let records = this.state.performanceRecords.slice(0, page * this.itemPerPage);
        this.setState({ displayedPerformanceRecords: records, hasMore: records.length < this.state.performanceRecords.length }, () => {
            debugger;

            if (callback)
                callback();
        });
    }

    handleExpandForm = isExpanded => this.setState({ isFormExpanded: isExpanded });

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
                submittedDate={submittedDate} modifiedDate={modifiedDate}
                isHost={isHost}
                grade={performanceRecord.grade}
                comment={performanceRecord.comment}
                projects={performanceRecord.projects.map(project => { return project.name; })}
                onEdit={this.handleEditPerformanceRecord}
            />
        );
    }

    handlePCIClick = performanceRecordId => {
        debugger;
        if (!this.state.displayedPerformanceRecords.some(r => r.performanceRecordId === performanceRecordId)) {
            this.selectedPCIId = performanceRecordId;
            let index = this.state.performanceRecords.findIndex(function (element) {
                return element.performanceRecordId === performanceRecordId;
            });

            let page = Math.ceil(index + 1 / this.itemPerPage);

            this.infiniteScrollRef.current.pageLoaded = page;
            this.loadPCIs(page, () => {
                document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'nearest', behavior: 'smooth' });
            });
        } else {
            document.getElementById(`record-${performanceRecordId}`).scrollIntoView({ block: 'nearest', behavior: 'smooth' });
        }
    }

    render() {
        const performanceRecords = this.state.displayedPerformanceRecords;
        const avatar = `dist/styles/${avatar1}`;

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page">
                    <div className="general-info">
                        <EmployeeInfo employee={this.state.employee} />
                        <PerformanceAppraisalResult />
                    </div>
                    <div className="main-my-page">
                        <div className="fixed-section">
                            <div className="warning">
                                News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed
                            </div>
                            <PersonalTimeline itemPerPage={this.itemPerPage} onItemClick={this.handlePCIClick} />
                            <CreateSelfPerformance
                                employee={this.state.employee}
                                performanceGradeOptions={this.state.performanceGradeOptions}
                                projectOptions={this.state.projectOptions}
                                onSubmitSuccess={this.handleCreatePerformanceSuccess}
                                onExpandForm={this.handleExpandForm}
                            />
                        </div>
                        <div className={this.state.isFormExpanded ? 'record-list record-list-collapse' : 'record-list record-list-expand'}>
                            {/* <Scrollbars style={{ height: '500px' }} autoHide={true} className="scrollbar" > */}
                            {performanceRecords.length > 0 &&
                                <InfiniteScroll
                                    pageStart={1} initialLoad={true}
                                    loadMore={this.loadPCIs}
                                    hasMore={this.state.hasMore}
                                    threshold={0}
                                    useWindow={true} ref={r => this.infiniteScrollRef = r} >
                                    {
                                        performanceRecords.map(this.renderPerformanceRecord)
                                    }
                                </InfiniteScroll>
                            }
                        <PerformanceRecord comment="It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                            {/* </Scrollbars > */}

                        </div>
                    </div>
                    <div className="work-info">
                        <NominationResult />
                        {this.state.periods.length > 0 && <WorkLoad periods={this.state.periods} />}

                        <HrmSummary />
                    </div>

                    {
                        this.state.showEditPerformanceModal &&
                        <EditSelfPerformance
                            performanceRecordId={this.state.performanceRecordIdSelected}
                            performanceGradeOptions={this.state.performanceGradeOptions}
                            projectOptions={this.state.projectOptions}
                            showModal={this.state.showEditPerformanceModal}
                            onCloseModal={this.handleCloseEditPerformanceModal}
                            onSubmitSuccess={this.handleEditPerformanceSuccess}
                        />
                    }
                    <ReactTooltip id="tooltip" place="bottom" type="info" html={true} className="tooltip" />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('main-container'));