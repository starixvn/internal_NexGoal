import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, NominationStatuses, PerformanceRecordStatus } from '../../constants';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import PersonalTimeline from './PersonalTimeline';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import TeamWorkLoad from './TeamWorkLoad';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceAppraisals: [],
            performanceRecords: [],
            nominations: [],
            workLoad: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('Self/Performance/Index')),
            axios(resolveClientUrl('Home/GetWorkLoad'))
        ];

        Promise.all(promises)
            .then(([paResponse, nominationResponse, performanceRecordResponse, workLoadResponse]) => {
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
                    performanceAppraisals: performanceAppraisals,
                    nominations: nominationResponse.data,
                    performanceRecords: performanceRecordResponse.data.performanceRecords,
                    workLoad: workLoadResponse.data
                });
            }).catch(axiosError);
    }

    handleAddNewPerformanceClick = () => {
        window.location = resolveClientUrl('Self/Performance/Create');
    }

    render() {
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.performanceRecords;
        const nominations = this.state.nominations;

        return (
            <div className="row">
                <div className="col-sm-3">
                    <PersonalTimeline />
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
                    <input className="form-control add-new-performance" placeholder="What's on your mind?"
                        onClick={this.handleAddNewPerformanceClick}
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
                    <WorkLoad workLoad={this.state.workLoad} />
                    <HrmSummary />
                </div>
                <TeamWorkLoad />
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('main-container'));