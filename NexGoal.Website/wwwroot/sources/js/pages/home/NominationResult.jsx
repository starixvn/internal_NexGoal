import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { NominationStatuses } from '../../constants';

export default class PerformanceAppraisalResult extends Component {
    static propTypes = {
        nominations: PropTypes.arrayOf(PropTypes.object)
    };

    static defaultProps = {
        nominations: []
    };

    render() {
        const nominations = this.props.nominations;

        return (
            <table className="table table-bordered">
                <thead>
                    <tr><th colSpan="2">Nomination</th></tr>
                </thead>
                <tbody>
                    {
                        nominations.map(nomination => {
                            return (
                                <tr key={`nomination${nomination.eoqyId}`}>
                                    <td>{nomination.period.title}</td>
                                    <td>{NominationStatuses.find(x => x.value === nomination.statusId).text}</td>
                                </tr>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}