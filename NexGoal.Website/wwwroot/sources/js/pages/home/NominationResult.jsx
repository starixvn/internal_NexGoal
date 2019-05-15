import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { getNominationClassName } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import getResource from '../../resourceHelper';

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

    groupNominationByStatusId = nominations => {
        const nominationReduces = nominations.reduce(function (groups, item) {
            const val = item['statusId'];
            groups[val] = groups[val] || [];
            groups[val].push(item.period);

            return groups;
        }, []);

        const nominationReults = Object.keys(nominationReduces).map(key => {
            const statusId = parseInt(key, 10);

            return {
                statusId: statusId,
                statusText: NominationStatuses.find(x => x.value === statusId).text,
                periods: nominationReduces[key]
            };
        });

        return nominationReults;
    }

    render() {
        const nominations = this.groupNominationByStatusId(this.props.nominations);

        return (
            <div className="nomination box">
                <h5>{getResource('Achievements')}</h5>
                {
                    nominations && nominations.map((nomination, index) => {
                        return (
                            <div className="result-item" key={`nominationResult${index}`}>
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