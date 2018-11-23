import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, getNominationClassName, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';
import Multiselect from '../../shared/components/multi-select/index';

export default class NominationResult extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                periods: []
            },
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
                        label: period.shortTitle
                    };
                });

                this.setState({
                    periodOptions: periodOptions,
                    filter: {
                        periods: periodOptions
                    },
                    memberNominations: nominationResponse.data
                });
            }).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.state.periodOptions) !== JSON.stringify(nextState.periodOptions)
            || JSON.stringify(this.state.filter) !== JSON.stringify(nextState.filter)
            || JSON.stringify(this.state.memberNominations) !== JSON.stringify(nextState.memberNominations)) {
            return true;
        }

        return false;
    }

    handlePeriodChange = (selectedOptions) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { periods: selectedOptions })
        });
    }

    groupNomination = () => {
        let memberNominations = this.state.memberNominations;
        let nominations = {};

        memberNominations.forEach(function (nomination) {
            let key = nomination.period.nominationPeriodId;

            if (!nominations[key]) {
                nominations[key] = {
                    period: nomination.period,
                    winners: nomination.statusId === NominationStatuses[2].value ? [nomination.nickName] : [],
                    shortlisteds: nomination.statusId === NominationStatuses[1].value ? [nomination.nickName] : [],
                    nominateds: nomination.statusId === NominationStatuses[0].value ? [nomination.nickName] : []
                };
            } else {
                switch (nomination.statusId) {
                    case NominationStatuses[0].value:
                        nominations[key]['nominateds'].push(nomination.nickName);
                        break;

                    case NominationStatuses[1].value:
                        nominations[key]['shortlisteds'].push(nomination.nickName);
                        break;

                    case NominationStatuses[2].value:
                        nominations[key]['winners'].push(nomination.nickName);
                        break;
                }
            }
        });

        nominations = Object.keys(nominations).map(key => {
            let index = this.state.filter.periods.findIndex(period => period.value === nominations[key].period.nominationPeriodId);

            return {
                period: nominations[key].period,
                winners: nominations[key].winners,
                shortlisteds: nominations[key].shortlisteds,
                nominateds: nominations[key].nominateds,
                isDisplay: index >= 0
            };
        });

        let sortDesc = function (nomination1, nomination2) {
            let date1 = new Date(nomination1.period.endDateForCommittee);
            let date2 = new Date(nomination2.period.endDateForCommittee);

            if (date1 > date2) return -1;
            if (date1 < date2) return 1;

            return 0;
        };

        return nominations.filter(group => group.isDisplay).sort(sortDesc);
    }

    render() {
        let nominations = this.groupNomination();
        let periodOptions = this.state.periodOptions;

        return (
            <Fragment>
                {
                    this.state.memberNominations && this.state.memberNominations.length > 0 &&
                    <div className="nomination box">
                        <h5>{getResource('Nomination')}</h5>
                        {
                            periodOptions && periodOptions.length > 1 &&
                            <div className="ddl-team">
                                <div className="team">{getResource('Period')}</div>
                                <Multiselect className="react-select-container" classNamePrefix="react-select"
                                    value={this.state.filter.periods}
                                    options={periodOptions}
                                    onChange={this.handlePeriodChange}
                                />
                            </div>
                        }
                        {
                            nominations && nominations.map((nomination, index) => {
                                return (
                                    <Fragment key={`nominationResult${index}`}>
                                        <div className="nomination-period-title">{nomination.period.shortTitle}</div>
                                        {
                                            nomination.winners.length > 0 &&
                                            <div>
                                                <span className={getNominationClassName(NominationStatuses[2].value)}>Winner</span>
                                                <span>: {nomination.winners.join(', ')}</span>
                                            </div>
                                        }
                                        {
                                            nomination.shortlisteds.length > 0 &&
                                            <div>
                                                <span className={getNominationClassName(NominationStatuses[1].value)}>Shortlisted</span>
                                                <span>: {nomination.shortlisteds.join(', ')}</span>
                                            </div>
                                        }
                                        {
                                            nomination.nominateds.length > 0 &&
                                            <div>
                                                <span className={getNominationClassName(NominationStatuses[0].value)}>Nominated</span>
                                                <span>: {nomination.nominateds.join(', ')}</span>
                                            </div>
                                        }
                                    </Fragment>
                                );
                            })
                        }
                    </div>
                }
            </Fragment>
        );
    }
}