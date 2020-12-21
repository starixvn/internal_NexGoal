import React, { Component } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import moment from 'moment';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PerformanceProcess } from '../../../constants';
import Timeline from '../../member/timeline/index';

import ToastLayout from '../../../shared/layouts/ToastLayout';
import CustomScrollbars from '../../../shared/components/CustomScrollbars';
import NexPanel from '../../../shared/components/NexPanel';
import ProbationProcess from '../ProbationProcess';
import HRM from '../HRM';
import EvaluatorForm from './EvaluatorForm';
import ReviewerForm from './ReviewerForm';
import Description from '../Description';

export default class Evaluation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationer: null,
            evaluationDetails: [],
            hrm: null
        };

        this.probationerInfo = {
            employeeId: parseInt(document.getElementById('probationer-id').value, 10),
            nickName: document.getElementById('probationer-nickName').value,
            fullName: document.getElementById('probationer-fullName').value,
            jobTitle: document.getElementById('probationer-jobTitle').value,
            groupName: document.getElementById('probationer-groupname').value,
            avatar: document.getElementById('probationer-avatar').value,
            joinedDate: moment(document.getElementById('probationer-joineddate').value)
        };

        this.myGroupOptions = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Probation/GetEvaluationDetail'), {
                params: {
                    employeeId: this.probationerInfo.employeeId
                }
            }),
            axios(resolveClientUrl('Probation/GetEvaluation'), {
                params: {
                    employeeId: this.probationerInfo.employeeId
                }
            }),
            axios(resolveClientUrl('Probation/GetHRM'), {
                params: {
                    employeeId: this.probationerInfo.employeeId
                }
            }),
            axios(resolveClientUrl('Common/GetMyGroup')),
            axios(resolveClientUrl('Common/GetCurrentPerformancePeriod'))
        ];

        Promise.all(promises)
            .then(([evaluationDetailResponse, probationerResponse, hrmResponse, myGroupResponse, currentPerformancePeriodResponse]) => {
                this.myGroupOptions = myGroupResponse.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });
                const probationer = probationerResponse.data;

                probationer.settings.forEach(setting => {
                    setting.fromDate = moment(setting.fromDate);
                    setting.toDate = moment(setting.toDate);
                });

                this.setState({
                    evaluationDetails: evaluationDetailResponse.data,
                    probationer: probationer,
                    currentPerformancePeriod: currentPerformancePeriodResponse.data,
                    hrm: hrmResponse.data,
                    processId: this.getProcessId(probationer)
                });
            }).catch(axiosError);
    }

    getProcessId = probationer => {
        const processIds = probationer.processIds;

        if (processIds.length === 1) {
            return processIds[0];
        }

        const reviewerProcess = probationer.settings.find(x => x.processId === PerformanceProcess.Reviewer);

        if (moment().isBefore(reviewerProcess.fromDate) && processIds.some(p => p === PerformanceProcess.Evaluator)) {
            return PerformanceProcess.Evaluator;
        }

        if (moment().isSameOrAfter(reviewerProcess.fromDate) && processIds.some(p => p === PerformanceProcess.Reviewer)) {
            return PerformanceProcess.Reviewer;
        }

        return null;
    };

    render() {
        return (
            <div className="row">
                <div className="main-layout">
                    <div className="general-info member-timeline-wrapper box">
                        <CustomScrollbars>
                            <Timeline ref="memberTimelineRef"
                                currentPerformancePeriod={this.state.currentPerformancePeriod}
                                groupOptions={this.myGroupOptions}
                                onNodeClick={this.handleClickTimelineNode}
                            />
                        </CustomScrollbars>
                    </div>
                    {this.state.probationer &&
                        <div className="probation-content">
                            <NexPanel title="PERFORMANCE REVIEW: SKILLS AND ABILITIE">
                                <Description employeeInfo={this.probationerInfo} />
                                <ProbationProcess processes={this.state.probationer.settings} />
                            </NexPanel>

                            <NexPanel title="HRM System" enable={false}>
                                <HRM hrm={this.state.hrm} />
                            </NexPanel>
                            {this.state.processId === PerformanceProcess.Evaluator &&
                                <EvaluatorForm probationer={this.state.probationer} evaluationDetails={this.state.evaluationDetails} />
                            }

                            {this.state.processId === PerformanceProcess.Reviewer &&
                                <ReviewerForm probationer={this.state.probationer} evaluationDetails={this.state.evaluationDetails} />
                            }
                        </div>
                    }

                </div>
                <ToastLayout />
            </div >
        );
    }
}

ReactDOM.render(<Evaluation />, document.getElementById('evaluation-wrapper'));