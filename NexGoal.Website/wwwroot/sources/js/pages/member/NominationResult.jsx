import React, { Component } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { getNominationClassName } from '../../common-functions';
import { NominationStatuses } from '../../constants';

export default class NominationResult extends Component {
    constructor(props) {
        super(props);

        this.state = {
            periodOptions: [],
            memberNominations: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('member/GetNominationPeriod'), { params: { isCompleted: true } }),
            axios(resolveClientUrl('member/GetMemberNominationResult'))
        ];

        Promise.all(promises)
            .then(([periodResponse, nominationResponse]) => {
                let periodOptions = periodResponse.data.map(period => {
                    return {
                        value: period.nominationPeriodId,
                        label: period.title
                    };
                });

                this.setState({
                    periodOptions: periodOptions,
                    memberNominations: nominationResponse.data
                });
            }).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.state.periodOptions) !== JSON.stringify(nextState.periodOptions)
            || JSON.stringify(this.state.memberNominations) !== JSON.stringify(nextState.memberNominations)) {
            return true;
        }

        return false;
    }

    render() {
        const memberNominations = this.state.memberNominations;

        return (
            <div className="nomination box">
                <h5>Nomination</h5>
                {
                    memberNominations && memberNominations.map((nomination, index) => {
                        return (
                            <div key={`nominationResult${index}`}>
                                <span className="nick-name">{nomination.nickName}</span>
                                <span> - </span>
                                {nomination.period.shortTitle}
                                <span> - </span>
                                <span className={getNominationClassName(nomination.statusId)}>
                                    {NominationStatuses.find(x => x.value === nomination.statusId).text}
                                </span>
                            </div>
                        );
                    })
                }
            </div>
        );
    }
}