import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import MemberWorkload from './MemberWorkload';
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
            axios(resolveClientUrl('Common/GetAllPeriods'))

        ];

        Promise.all(promises)
            .then(([memberNominationResponse, periodResponse]) => {
                this.setState({
                    memberNominations: memberNominationResponse.data,
                    periods: periodResponse.data
                });
            }).catch(axiosError);
    }

    render() {
        const memberNominations = this.state.memberNominations;

        return (
            <ToastLayout>
                <div className="row">
                    <div className="main-layout member-page">
                        <div className="general-info">
                            <Timeline />
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                            </div>
                        </div>
                        <div className="work-info">
                            <div className="nomination">
                                <h5>{getResource('MemberNomination')}</h5>
                                {
                                    memberNominations.map(memberNomination => {
                                        return (
                                            <div key={`memberNomination-${memberNomination.employeeId}-${memberNomination.period.nominationPeriodId}`}>
                                                <span className="nick-name">{memberNomination.nickName}</span> - <span>{memberNomination.period.title}</span> - <span className="status-nominated ">{NominationStatuses.find(x => x.value === memberNomination.statusId).text}</span>
                                            </div>
                                        );
                                    })
                                }
                            </div>
                            {this.state.periods.length > 0 && <MemberWorkload periods={this.state.periods} />}
                        </div>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));