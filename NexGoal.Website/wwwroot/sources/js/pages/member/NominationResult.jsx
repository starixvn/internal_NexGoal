import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, getNominationClassName, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';
import Milestone from '../../shared/components/milestone';

export default class NominationResult extends Component {
    constructor(props) {
        super(props);

        this.state = {
            year: 0,
            yearOptions: [],
            memberNominations: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('member/GetNominationPeriod'), { params: { isCompleted: true } })
            .then(response => {
                let yearOptions = response.data.map(period => period.year).filter((value, index, self) => self.indexOf(value) === index);
                let year = yearOptions.length > 0 ? yearOptions[0] : null;

                this.setState({
                    yearOptions: yearOptions,
                    year: year
                });

                this.getMemberNominations(year);
            }).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.state.yearOptions) !== JSON.stringify(nextState.yearOptions)
            || JSON.stringify(this.state.filter) !== JSON.stringify(nextState.filter)
            || JSON.stringify(this.state.memberNominations) !== JSON.stringify(nextState.memberNominations)) {
            return true;
        }

        return false;
    }

    getMemberNominations = (year) => {
        axios.get(resolveClientUrl('member/GetMemberNominationResult'), { params: { year: year } })
            .then(response => {
                this.setState({
                    memberNominations: response.data
                });
            }).catch(axiosError);
    }

    onYearChange = (year) => {
        this.setState({
            year: year
        }, () => this.getMemberNominations(this.state.year));
    }

    groupMemberNominations = () => {
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
            return {
                period: nominations[key].period,
                winners: nominations[key].winners,
                shortlisteds: nominations[key].shortlisteds,
                nominateds: nominations[key].nominateds
            };
        });

        let sortDesc = function (nomination1, nomination2) {
            let date1 = new Date(nomination1.period.endDateForCommittee);
            let date2 = new Date(nomination2.period.endDateForCommittee);

            if (date1 > date2) return -1;
            if (date1 < date2) return 1;

            return 0;
        };

        return nominations.sort(sortDesc);
    }

    renderMemberNominations = () => {
        let memberNominations = this.groupMemberNominations();

        if (memberNominations && memberNominations.length > 0) {
            return (
                <Fragment>
                    {
                        memberNominations.map((nomination, index) => {
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
                </Fragment>
            );
        }

        return <div>{getResource('NoInformationAvailable')}</div>;
    }

    render() {
        const yearOptions = this.state.yearOptions.map(option => {
            return {
                value: option,
                name: option
            };
        });

        return (
            <Fragment>
                {
                    this.state.yearOptions && this.state.yearOptions.length > 0 &&
                    <div className="nomination box">
                        <div className="title">
                            <h5>{getResource('Nomination')}</h5>
                            <Milestone
                                <div className={`icon-chevron-left arrow-left ${filter.nextDisabled ? 'disabled' : ''}`}
                                onSelectionChange={this.onYearChange}
                                ></div>
                                <div className={`icon-chevron-right arrow-right ${filter.prevDisabled ? 'disabled' : ''}`}
                                ></div>
                        </div>
                        {this.renderMemberNominations()}
                    </div>
                }
            </Fragment>
        );
    }
}