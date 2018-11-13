import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { getNominationClassName } from '../../common-functions';
import { NominationStatuses } from '../../constants';

export default class NominationResult extends Component {
    static propTypes = {
        isSupervisor: PropTypes.bool,
        nominations: PropTypes.arrayOf(PropTypes.object)
    };

    static defaultProps = {
        isSupervisor: false,
        nominations: []
    };

    shouldComponentUpdate(nextProps) {
        if (this.props.isSupervisor !== nextProps.isSupervisor
            || JSON.stringify(this.props.nominations) !== JSON.stringify(nextProps.nominations)) {
            return true;
        }
        return false;
    }

    render() {
        const nominations = this.props.nominations;

        return (
            <div className="nomination box">
                <h5>Nomination</h5>
                {
                    nominations && nominations.map((nomination, index) => {
                        return (
                            <div key={`nominationResult${index}`}>
                                {
                                    this.props.isSupervisor &&
                                    <span className="nick-name">{nomination.nickName} - </span>
                                }
                                {nomination.period.title} - <span className={getNominationClassName(nomination.statusId)}>
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