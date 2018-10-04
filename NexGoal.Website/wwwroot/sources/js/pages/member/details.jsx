import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberId: document.getElementById('MemberId').value,
            performanceAppraisals: [],
            nominations: []
        };
    }

    componentDidMount() {
        const memberId = this.state.memberId;

        const promises = [
            axios(resolveClientUrl('member/GetMemberNominationResult'), { params: { memberId: memberId } }),
            axios(resolveClientUrl('member/GetMemberPerformanceAppraisalResult'), { params: { memberId: memberId } })
        ];

        Promise.all(promises)
            .then(([nominationResponse, performanceAppraisalResponse]) => {
                this.setState({
                    nominations: nominationResponse.data,
                    performanceAppraisals: performanceAppraisalResponse.data
                });
            }).catch(axiosError);
    }

    render() {
        const nominations = this.state.nominations;
        const performanceAppraisals = this.state.performanceAppraisals;

        return (
            <ToastLayout>
                <div className="row">
                    <div className="col-sm-3">
                        <fieldset>
                            <legend>Member PAs</legend>
                            {
                                performanceAppraisals.map(performanceAppraisal => {
                                    return (
                                        <div key={`performanceAppraisal${performanceAppraisal.paRecordId}`}>
                                            {performanceAppraisal.period.name}-
                                            {performanceAppraisal.grade.name}
                                        </div>
                                    );
                                })
                            }
                        </fieldset>
                        <fieldset>
                            <legend>{getResource('MemberNomination')}</legend>
                            {
                                nominations.map(nomination => {
                                    return (
                                        <div key={`nomination${nomination.employeeId}-${nomination.period.nominationPeriodId}`}>
                                            {nomination.period.title}-
                                            {NominationStatuses.find(x => x.value === nomination.statusId).text}
                                        </div>
                                    );
                                })
                            }
                        </fieldset>
                    </div>
                    <div className="col-sm-6">
                        <fieldset>
                            <legend>{getResource('MemberPerformance')}</legend>
                        </fieldset>
                    </div>
                    <div className="col-sm-3">
                        <fieldset>
                            <legend>Workload</legend>
                        </fieldset>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));