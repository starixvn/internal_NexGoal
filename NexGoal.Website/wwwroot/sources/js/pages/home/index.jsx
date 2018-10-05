import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, NominationStatuses, PerformanceRecordStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import CreateSelfPerformance from './CreateSelfPerformance';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import TeamWorkLoad from './TeamWorkLoad';
import MemberPerformance from './MemberPerformance';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceAppraisals: [],
            performanceRecords: [],
            nominations: [],
            workload: [],
            performanceGradeOptions: [],
            projectOptions: []
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
                    performanceRecords: performanceRecordResponse.data.performanceRecords,
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
                        <table className="table table-bordered">
                            <thead>
                                <tr><th colSpan="2">My PAs</th></tr>
                            </thead>
                            <tbody>
                                {
                                    performanceAppraisals.map(performanceAppraisal => {
                                        return (
                                            <Fragment key={`year${performanceAppraisal.year}`}>
                                                <tr>
                                                    <td colSpan="2">{performanceAppraisal.year}</td>
                                                </tr>

                                                {
                                                    performanceAppraisal.records.map(record => {
                                                        let grade = record.grade ? record.grade.name : '';
                                                        return (
                                                            <tr key={`performanceAppraisal${record.paRecordId}`}>
                                                                <td>{record.period.name}</td>
                                                                <td>{grade}</td>
                                                            </tr>
                                                        );
                                                    })
                                                }
                                            </Fragment>
                                        );
                                    })
                                }
                            </tbody>
                        </table>
                    }
                    {
                        nominations.length > 0 &&
                        <table className="table table-bordered">
                            <thead>
                                <tr><th colSpan="2">Nomination</th></tr>
                            </thead>
                            <tbody>
                                {
                                    nominations.map(nomination => {
                                        return (
                                            <tr key={`nomination${nomination.eoqyId}`}>
                                                <td>{nomination.period.title}</td>
                                                <td>{NominationStatuses.find(x => x.value === nomination.statusId).text}</td>
                                            </tr>
                                        );
                                    })
                                }
                            </tbody>
                        </table>
                    }
                </div>
                <div className="col-sm-6">
                    <CreateSelfPerformance
                        performanceGradeOptions={this.state.performanceGradeOptions}
                        projectOptions={this.state.projectOptions}
                        handleSuccess={this.handleCreatePerformanceSuccess}
                    />
                    {
                        performanceRecords.length > 0 &&
                        <div className="performance-record">
                            {
                                performanceRecords.map((performanceRecord) => {
                                    let isHost = performanceRecord.statusId === PerformanceRecordStatus.Draft
                                        || performanceRecord.statusId === PerformanceRecordStatus.Submitted;
                                    let editUrl = isHost ? `${resolveClientUrl('Self/Performance/Edit/') + performanceRecord.performanceRecordId}` : null;
                                    let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
                                    let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

                                    return (
                                        <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                                            avatar={performanceRecord.creator.base64Image}
                                            nickName={performanceRecord.creator.nickName}
                                            submittedDate={submittedDate} modifiedDate={modifiedDate}
                                            isHost={isHost}
                                            editUrl={editUrl}
                                            grade={performanceRecord.grade ? performanceRecord.grade.name : ''}
                                            comment={performanceRecord.comment}
                                            projects={performanceRecord.projects.map(project => { return project.name; }).join(', ')}
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
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('main-container'));