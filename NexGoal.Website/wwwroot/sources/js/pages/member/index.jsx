import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import TeamWorkload from './TeamWorkLoad';
import Timeline from './timeline/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberPerformanceAppraisals: [],
            memberNominations: [],
            periods: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('member/GetMemberNominationResult')),
            axios(resolveClientUrl('Common/GetAllPeriods')),

        ];

        Promise.all(promises)
            .then(([memberNominationResponse, periodResponse]) => {
                this.setState({ memberNominations: memberNominationResponse.data, periods: periodResponse.data });
            }).catch(axiosError);
    }

    render() {
        const memberNominations = this.state.memberNominations;

        return (
            <ToastLayout>
                <div className="row">
                    <div className="col-sm-3">
                        <Timeline />
                    </div>
                    <div className="col-sm-6">
                        <fieldset>
                            <legend>{getResource('MemberPerformance')}</legend>
                        </fieldset>
                    </div>
                    <div className="col-sm-3">
                        <fieldset>
                            <legend>{getResource('MemberNomination')}</legend>
                            {
                                memberNominations.map(memberNomination => {
                                    return (
                                        <div key={`memberNomination-${memberNomination.employeeId}-${memberNomination.period.nominationPeriodId}`}>
                                            {memberNomination.nickName}-
                                            {memberNomination.period.title}-
                                            {NominationStatuses.find(x => x.value === memberNomination.statusId).text}
                                        </div>
                                    );
                                })
                            }
                        </fieldset>
                        <fieldset>
                            <legend>Workload</legend>
                        </fieldset>
                        {this.state.periods.length > 0 && <TeamWorkload periods={this.state.periods} />}
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));