import React, { Component } from 'react';
import { NominationStatuses } from '../../constants';

export default class NominationResult extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.nominations) !== JSON.stringify(nextProps.nominations)) {
            return true;
        }
        return false;
    }

    getCssClass = (statusId) => {
        let cssClass = '';

        switch (statusId) {
            case NominationStatuses[0].value:
                cssClass = 'status-nomination';
                break;

            case NominationStatuses[1].value:
                cssClass = 'status-shortlisted';
                break;

            case NominationStatuses[2].value:
                cssClass = 'status-winner';
                break;
        }

        return cssClass;
    }

    render() {
        const nominations = this.props.nominations;

        return (
            <div className="nomination">
                <h5>Nomination</h5>
                {
                    nominations && nominations.map(nomination => {
                        return (
                            <div key={`nomination${nomination.eoqyId}`}>
                                {nomination.period.title} - <span className={this.getCssClass(nomination.statusId)}>{NominationStatuses.find(x => x.value === nomination.statusId).text}</span>
                            </div>
                        );
                    })
                }
            </div>
        );
    }
}