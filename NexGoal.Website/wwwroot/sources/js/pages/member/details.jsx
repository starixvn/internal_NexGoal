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
            memberNominations: []
        };
    }

    componentDidMount() {
        const memberId = this.state.memberId;

        const promises = [
            axios(resolveClientUrl('member/GetMemberNominationResult'), {
                params: {
                    memberId: memberId
                }
            })
        ];

        Promise.all(promises)
            .then(([memberNominationResponse]) => {
                this.setState({ memberNominations: memberNominationResponse.data });
            }).catch(axiosError);
    }

    render() {
        const memberNominations = this.state.memberNominations;

        return (
            <ToastLayout>
                <div className="row">
                    <div className="col-sm-3">
                        <fieldset>
                            <legend>{getResource('MemberNomination')}</legend>
                            {
                                memberNominations.map(memberNomination => {
                                    return (
                                        <div key={`memberNomination-${memberNomination.employeeId}-${memberNomination.period.nominationPeriodId}`}>
                                            {memberNomination.period.title}-
                                            {NominationStatuses.find(x => x.value === memberNomination.statusId).text}
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