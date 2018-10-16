import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import ReactTooltip from 'react-tooltip';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import CreateSelfPerformance from './CreateSelfPerformance';
import EditSelfPerformance from './EditSelfPerformance';
import WorkLoad from './workload';
import HrmSummary from './HRMSummary';
import avatar1 from '../../../styles/images/avatar.png';
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
            performanceAppraisals: [],
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
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('Self/Performance/Index')),
            axios(resolveClientUrl('Common/GetAllPeriods')),
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, paResponse, nominationResponse, performanceRecordResponse, periodResponse]) => {
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
                performanceAppraisals.forEach(record => record.year = moment(record.period.startDate).year());
                performanceAppraisals = performanceAppraisals.reduce(function (groups, item) {
                    const val = item['year'];
                    groups[val] = groups[val] || [];
                    groups[val].push(item);

                    return groups;
                }, []);

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
        const performanceAppraisals = this.state.performanceAppraisals;
        const performanceRecords = this.state.performanceRecords;
        const nominations = this.state.nominations;
        const avatar = `dist/styles/${avatar1}`;

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page">
                    <div className="general-info">
                        {/* <MemberPerformance memberName="Elton" pcis={this.state.performanceRecords.slice(0, 6).reverse()} />
                        <MemberPerformance memberName="Raymond" pcis={this.state.performanceRecords.slice(0, 5).reverse()} />
                        <MemberPerformance memberName="Tim" pcis={this.state.performanceRecords.slice(0, 3).reverse()} />
                        <MemberPerformance memberName="Olivier" pcis={this.state.performanceRecords.slice(0, 4).reverse()} />
                        {
                            performanceAppraisals.length > 0 &&
                            <PerformanceAppraisalResult performanceAppraisals={performanceAppraisals} />
                        }
                        {
                            nominations.length > 0 &&
                            <NominationResult nominations={nominations} />
                        } */}
                        <div className="user-info">
                            <img src={avatar} alt="avatar" className="avatar"/>
                            <div className="nick-name">Richard</div>
                            <div className="user-name">Trương Văn Thảo</div>
                            <div className="role role-PM">Project Manager</div>
                        </div>
                        <div className="my-pa">
                            <div className="my-pa-year">
                                <div className="year">2018</div>
                                <div className="term"><div>Second Half</div><div className="grade">Exception</div></div>
                                <div className="term"><div>First Half</div><div className="grade">Meet Expectations +</div></div>
                            </div>
                            <div className="my-pa-year">
                                <div className="year">2017</div>
                                <div className="term"><div>Second Half</div><div className="grade">Exceeds Expectations</div></div>
                                <div className="term"><div>First Half</div><div className="grade">Meet Expectations +</div></div>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="warning">
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
                        <PerformanceRecord comment={"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                                                    }
                                           grade={"Meets Expectations +"}  projects={"Alpha, Nexgoal"} nickName={"Richard"} submittedDate={"Aug 01, 2018"}     
                        />
                         <PerformanceRecord comment={"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                                                    }
                                           grade={"Meets Expectations +"}  projects={"Alpha, Nexgoal"}  nickName={"Raymond"} submittedDate={"Aug 01, 2018"}      
                        />
                         <PerformanceRecord comment={"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                                                    }
                                           grade={"Meets Expectations +"}  projects={"Alpha, Nexgoal"}   nickName={"Peter"} submittedDate={"Aug 01, 2018"}     
                        />
                         <PerformanceRecord comment={"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.\
                                                    The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here',\
                                                    marking it look like readable English.many desktop publishing package and web."
                                                    }
                                           grade={"Meets Expectations +"}  projects={"Alpha, Nexgoal"}    nickName={"Ronny"} submittedDate={"Aug 01, 2018"}    
                        />
                    </div>
                    <div className="work-info">
                        <div className="nomination">
                            <h5>Nomination</h5>
                            <div>3rd EOQ 2018 - <span className="status-shortlisted">Shortlisted</span></div>
                            <div>EOY 2016 - <span className="status-nomination">Nominated</span></div>
                            <div>1st EOQ 2015 - <span className="status-winner">Winner</span></div>
                        </div>
                        <div className="workload">
                            <WorkLoad workLoad={this.state.workload} />
                        </div>
                        
                        <HrmSummary />
                    </div>
                    {/* <TeamWorkLoad /> */}

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