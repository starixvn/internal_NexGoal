import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import TeamWorkLoad from './TeamWorkLoad';
import MemberPerformance from './MemberPerformance';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';
import NominationResult from './NominationResult';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceAppraisals: [],
            performanceRecords: [],
            nominations: [],
            workload: [],
            performanceGradeOptions: [],
            projectOptions: [],
            showEditPerformanceModal: false,
            performanceRecordSelected: null
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('Self/Performance/Index')),
            axios(resolveClientUrl('Home/GetWorkLoad'))
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, paResponse, nominationResponse, performanceRecordResponse, workloadResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                let performanceAppraisals = paResponse.data;
                performanceAppraisals.map(record => record.year = moment(record.period.startDate).year());
                performanceAppraisals = performanceAppraisals.reduce(function (groups, item) {
                    const val = item['year'];
                    groups[val] = groups[val] || [];
                    groups[val].push(item);

                    return groups;
                }, {});

                performanceAppraisals = Object.keys(performanceAppraisals).map(key => {
                    return {
                        year: key,
                        records: performanceAppraisals[key]
                    };
                });

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions,
                    performanceAppraisals: performanceAppraisals,
                    nominations: nominationResponse.data,
                    performanceRecords: performanceRecordResponse.data,
                    workload: workloadResponse.data
                });
            }).catch(axiosError);
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
        let index = this.state.performanceRecords.map(record => { return record.performanceRecordId; }).indexOf(performanceRecordId);

        if (index >= 0) {
            this.setState({
                showEditPerformanceModal: true,
                performanceRecordSelected: { ...this.state.performanceRecords[index] }
            });
        }
    }

    handleCloseEditPerformanceModal = () => {
        this.setState({
            showEditPerformanceModal: false,
            performanceRecordSelected: null
        });
    }

    handleEditPerformanceSuccess = (performanceRecord) => {
        this.setState({
            showEditPerformanceModal: false,
            performanceRecordSelected: null
        });
    }

    render() {
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.performanceRecords;
        const nominations = this.state.nominations;

        return (
            <div className="row">
                <ToastLayout />
                <div className="col-sm-3">
                    <MemberPerformance memberName="Elton" pcis={this.state.performanceRecords.slice(0).reverse()} />
                    <MemberPerformance memberName="Raymond" pcis={this.state.performanceRecords.slice(0).reverse()} />
                    <MemberPerformance memberName="Tim" pcis={this.state.performanceRecords.slice(0).reverse()} />
                    <MemberPerformance memberName="Olivier" pcis={this.state.performanceRecords.slice(0).reverse()} />
                    {
                        performanceAppraisals.length > 0 &&
                        <PerformanceAppraisalResult performanceAppraisals={performanceAppraisals} />
                    }
                    {
                        nominations.length > 0 &&
                        <NominationResult nominations={nominations} />
                    }
                </div>
                <div className="col-sm-6">
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
                </div>
                <div className="col-sm-3">
                    <WorkLoad workLoad={this.state.workload} />
                    <HrmSummary />
                </div>
                <TeamWorkLoad />

                {
                    this.state.showEditPerformanceModal &&
                    <EditSelfPerformance
                        performanceRecord={this.state.performanceRecordSelected}
                        performanceGradeOptions={this.state.performanceGradeOptions}
                        projectOptions={this.state.projectOptions}
                        showModal={this.state.showEditPerformanceModal}
                        onCloseModal={this.handleCloseEditPerformanceModal}
                        onSubmitSuccess={this.handleEditPerformanceSuccess}
                    />
                }
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('main-container'));