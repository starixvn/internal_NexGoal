import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { getNominationClassName } from '../../common-functions';
import { NominationStatuses } from '../../constants';

export default class NominationResult extends Component {
    static propTypes = {
        nominations: PropTypes.arrayOf(PropTypes.object)
    };

    static defaultProps = {
        nominations: []
    };

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.nominations) !== JSON.stringify(nextProps.nominations)) {
            return true;
        }

        return false;
    }

    groupNominationByStatusId = (nominations) => {
        nominations = nominations.reduce(function (groups, item) {
            const val = item['statusId'];
            groups[val] = groups[val] || [];
            groups[val].push(item.period);

            return groups;
        }, []);

        nominations = Object.keys(nominations).map(key => {
            let statusId = parseInt(key);

            return {
                statusId: statusId,
                statusText: NominationStatuses.find(x => x.value === statusId).text,
                periods: nominations[key]
            };
        });

        return nominations;
    }

    render() {
        const nominations = this.groupNominationByStatusId(this.props.nominations);

        return (
            <div className="nomination box">
                <h5>Nomination</h5>
                {
                    nominations && nominations.map((nomination, index) => {
                        return (
                            <div key={`nominationResult${index}`}>
                                <span className={getNominationClassName(nomination.statusId)}>
                                    {nomination.statusText}
                                </span>: <span>{nomination.periods.map(period => period.shortTitle).join(', ')}</span>
                            </div>
                        );
                    })
                }
            </div>
        );
    }
}