import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import ReactTooltip from 'react-tooltip';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import EmployeeInfo from './EmployeeInfo';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';
import NominationResult from './NominationResult';
import PersonalTimeline from './PersonalTimeline';

var sanitizeHtmlReact = require('sanitize-html-react');
sanitizeHtmlReact.defaults.allowedTags = false;
sanitizeHtmlReact.defaults.allowedAttributes = false;

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: [],
            nominations: [],
            workload: [],
            performanceGradeOptions: [],
            projectOptions: [],
            showEditPerformanceModal: false,
            performanceRecordIdSelected: null,
            periods: []
        };
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

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    performanceRecords: performanceRecordResponse.data,
                    periods: periodResponse.data
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

    render() {
        const performanceRecords = this.state.performanceRecords;

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page">
                    <div className="general-info">
                        <EmployeeInfo />
                        <PerformanceAppraisalResult />
                    </div>
                    <div>
                        <div className="warning">
                            News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed
                        </div>
                        <PersonalTimeline />
                        <CreateSelfPerformance
                            performanceGradeOptions={this.state.performanceGradeOptions}
                            projectOptions={this.state.projectOptions}
                            onSubmitSuccess={this.handleCreatePerformanceSuccess}
                        />
                        {
                            performanceRecords.length > 0 &&
                            <div className="performance-record">
                                {
                                    performanceRecords.map((performanceRecord) => {
                                        let isHost = performanceRecord.statusId === PerformanceRecordStatus.Draft
                                            || performanceRecord.statusId === PerformanceRecordStatus.Submitted;
                                        let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
                                        let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

                                        return (
                                            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                                                objectId={performanceRecord.performanceRecordId}
                                                avatar={performanceRecord.creator.base64Image}
                                                nickName={performanceRecord.creator.nickName}
                                                submittedDate={submittedDate} modifiedDate={modifiedDate}
                                                isHost={isHost}
                                                grade={performanceRecord.grade ? performanceRecord.grade.name : ''}
                                                comment={performanceRecord.comment}
                                                projects={performanceRecord.projects.map(project => { return project.name; }).join(', ')}
                                                onEdit={this.handleEditPerformanceRecord}
                                            />
                                        );
                                    })
                                }
                            </div>
                        }
                        <PerformanceRecord comment="It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                            grade="Meets Expectations +" projects="Alpha, Nexgoal" nickName="Richard" submittedDate="Aug 01, 2018"
                        />
                        <PerformanceRecord comment="It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                            grade="Meets Expectations +" projects="Alpha, Nexgoal" nickName="Raymond" submittedDate="Aug 01, 2018"
                        />
                        <PerformanceRecord comment="It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                            grade="Meets Expectations +" projects={"Alpha, Nexgoal"} nickName="Peter" submittedDate="Aug 01, 2018"
                        />
                        <PerformanceRecord comment="It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                            grade="Meets Expectations +" projects="Alpha, Nexgoal" nickName="Ronny" submittedDate="Aug 01, 2018"
                        />
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